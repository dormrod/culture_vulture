using System;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Globalization;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Deserializers;
using System.Threading.Tasks;
using AppKit;

namespace CultureVulture
{
    public class GoodreadsHandler
    {
		
        //Client and API Keys
        private RestClient Client = new RestClient("https://www.goodreads.com/");
        private string DeveloperKey;
		private string DeveloperSecret;
        private string OAuthToken;
        private string OAuthTokenSecret;
        private string AccessToken;
        private string AccessTokenSecret;
        private string UserID;
        public string UserName="";
        

        public GoodreadsHandler()
        {
            //Construct loading developer keys

            //##### Developer keys and secret here #####
            DeveloperKey = "XXXXXX"; 
            DeveloperSecret = "XXXXXX"; 
            Client.Authenticator = OAuth1Authenticator.ForRequestToken(DeveloperKey, DeveloperSecret);
        }

        public string GenerateAuthURL()
        {
            //Generate URL for OAuth1 flow

			//Get OAuth tokens
            Client.Authenticator = OAuth1Authenticator.ForRequestToken(DeveloperKey, DeveloperSecret);
            var oauthRequestToken = new RestRequest("oauth/request_token");
            var response = Client.Execute(oauthRequestToken);
            var qs = HttpUtility.ParseQueryString(response.Content);
            OAuthToken = qs["oauth_token"];
            OAuthTokenSecret = qs["oauth_token_secret"];

            //Construct url for user to login to
            var oauthAuthorise = new RestRequest("oauth/authorize");
            oauthAuthorise.AddParameter("oauth_token", OAuthToken);
            oauthAuthorise.AddParameter("oauth_token_secret", OAuthTokenSecret);
            response = Client.Execute(oauthAuthorise);
            var url = Client.BuildUri(oauthAuthorise).ToString();

            return url;
        }

        public bool VerifyAuth()
        {
            //Check authentication ok

            try
            {
                Client.Authenticator = OAuth1Authenticator.ForAccessToken(DeveloperKey, DeveloperSecret, OAuthToken, OAuthTokenSecret);
                var oauthAccessToken = new RestRequest("oauth/access_token");
                var response = Client.Execute(oauthAccessToken);
                if ((int)response.StatusCode != 200) return false;
                var qs = HttpUtility.ParseQueryString(response.Content);
                AccessToken = qs["oauth_token"];
                AccessTokenSecret = qs["oauth_token_secret"];

                //Get user information
                Client.Authenticator = OAuth1Authenticator.ForProtectedResource(DeveloperKey, DeveloperSecret, AccessToken, AccessTokenSecret);
                var authUser = new RestRequest("api/auth_user", DataFormat.Xml);
                var authResponse = ExecuteGetRequest<AuthResponse>(authUser);
                UserID = authResponse.Data.user.id;
                UserName = authResponse.Data.user.name;
            }
            catch
            {
                return false;
			}

            return true;
        }

        public List<MediaModel> Pull(string shelf)
        {
            //Get all user books from goodreads account

            //Loop through record pages and extract books
            var books = new List<MediaModel>();
            int page = 1;
            string endRecord, totalRecord;

            //Loop until pagination complete
            do
            {
                //Get list of user reviwers
                var request = new RestRequest(string.Format("review/list/{0}.xml", UserID), DataFormat.Xml);
                request.AddParameter("v", 2);
                request.AddParameter("id", UserID);
                request.AddParameter("shelf", shelf);
                request.AddParameter("page", page);
                request.AddParameter("per_page", 200);
                request.AddParameter("key", DeveloperKey);
                var response = ExecuteGetRequest<ShelfResponse>(request);
                
                //Loop over each review and convert to media
				foreach (Review review in response.Data.reviews.reviews)
                {

                    //Extract book information
                    string bookTitle = review.book.titleWithoutSeries;
                    int bookRating = review.rating;
                    string bookAuthor = "";
                    string bookDate = "";
                    foreach (ReviewAuthor reviewAuthor in review.book.authors)
                    {
                        bookAuthor += reviewAuthor.name;
                    }
                    try
                    {
                        string dateTmp = "";
                        for (int i = 0; i < review.readAt.Length; ++i)
                        {
                            if (i < 20 || i > 25) dateTmp += review.readAt[i];
                        }
                        DateTime dt = DateTime.ParseExact(dateTmp, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);
                        bookDate = dt.ToString("dd-MM-yyyy");
                    }
                    catch
                    {
                        bookDate = "";
                    }
                    string GoodreadsBookID = review.book.id;
                    string GoodreadsReviewID = review.id;
                    string BookStatus="";
                    if (shelf == "read") BookStatus = "Completed";
                    else if (shelf == "currently-reading") BookStatus = "In Progress";
                    else if (shelf == "to-read") BookStatus = "Wish List";

                    var book = new MediaModel("Book",bookTitle,bookAuthor,"",bookDate,bookRating,BookStatus,GoodreadsBookID,GoodreadsReviewID);
                    books.Add(book);
                }
                endRecord = response.Data.reviews.end;
                totalRecord = response.Data.reviews.end;
                ++page;
            } while (endRecord != totalRecord);

            return books;
        }

        public (string bookID, string reviewID) PushNew(MediaModel book)
        {
            //Push new book to goodreads and return IDs to update local record
			
            //Search goodreads for book id and take top one
            var request = new RestRequest("search/index.xml", DataFormat.Xml);
            request.AddParameter("q", string.Format("{0} {1}", book.Creator, book.Title));
            request.AddParameter("page", 1);
            request.AddParameter("key", DeveloperKey);
            request.XmlSerializer = new RestSharp.Serializers.DotNetXmlSerializer();
            var response = Client.Execute<CatalogueSearch>(request);
            var deserialiser = new CustomXmlDeserialiser(); //Use custom deserialiser to handle non-escapted characters
            CatalogueSearch data = deserialiser.DeserializeRegEx<CatalogueSearch>(response);
            var bookID = data.search.results.work[0].bestBook.id;

            //Add book
            string shelf="";
            if (book.Status == "Completed") shelf = "read";
            else if (book.Status == "In Progress") shelf = "currently-reading";
            else if (book.Status == "Wish List") shelf = "to-read";
            var postRequest = new RestRequest("review.xml", Method.POST);
            postRequest.AddParameter("book_id", bookID);
            if (book.Rating != 0) postRequest.AddParameter("review[rating]", book.Rating);
            //if (book.Date != "") postRequest.AddParameter("review[read_at]", book.Date);
            postRequest.AddParameter("shelf", shelf);
            var postResponse = Client.Execute<Review>(postRequest);
            string reviewID = postResponse.Data.id;
            return (bookID,reviewID);
        }
        
		public void PushExisting(MediaModel book)
        {
            //Push existing book to goodreads by editing review

            //Edit book review
            string shelf = "";
            if (book.Status == "Completed") shelf = "read";
            else if (book.Status == "In Progress") shelf = "currently-reading";
            else if (book.Status == "Wish List") shelf = "to-read";
            var putRequest = new RestRequest(string.Format("review/{0}.xml", book.GoodreadsReviewID), Method.POST);
            putRequest.AddParameter("id", book.GoodreadsReviewID);
            putRequest.AddParameter("review[rating]", book.Rating);
            //if (book.Date != "") putRequest.AddParameter("review[read_at]", book.Date);
            putRequest.AddParameter("shelf", shelf);
            var putResponse = Client.Execute(putRequest);

            //Edit shelf
            var postRequest = new RestRequest("shelf/add_to_shelf.xml", Method.POST);
            postRequest.AddParameter("book_id", book.GoodreadsBookID);
            postRequest.AddParameter("name", shelf);
            var postResponse = Client.Execute(postRequest);
        }

        public IRestResponse<T> ExecuteGetRequest<T>(RestRequest request)
        {
            request.XmlSerializer = new RestSharp.Serializers.DotNetXmlSerializer();
            var response = Client.Execute<T>(request);
            return response;
        }
    }

    //#### CUSTOM DESEREALISER AS TO REMOVE ESCAPE CHARACTERS ####
    class CustomXmlDeserialiser : DotNetXmlDeserializer
    {
        public T DeserializeRegEx<T>(IRestResponse response)
        {
            string pattern = @"&#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|7F|8[0-46-9A-F]9[0-9A-F])"; // XML 1.0
            //string pattern = @"#x((10?|[2-F])FFF[EF]|FDD[0-9A-F]|[19][0-9A-F]|7F|8[0-46-9A-F]|0?[1-8BCEF])"; // XML 1.1
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (regex.IsMatch(response.Content))
            {
                response.Content = regex.Replace(response.Content, String.Empty);
            }
            response.Content = response.Content.Replace("&;", string.Empty);

            return base.Deserialize<T>(response);
        }
    }

    //#### AUTH XML DESEREALISATION ####
    [XmlRoot("GoodreadsResponse")]
    public class AuthResponse
    {
        [XmlElement("user")]
        public AuthUser user { get; set; }
    }

    public class AuthUser
    {
        [XmlElement("id")]
        public string id { get; set; }
        [XmlElement("name")]
        public string name { get; set; }
    }

    //#### SHELF XML DESEREALISATION ####
    [XmlRoot("GoodreadsResponse")]
    public class ShelfResponse
    {
        [XmlElement("reviews")]
        public Reviews reviews { get; set; }
    }

    public class Reviews
    {
        [XmlElement("start")]
        public string start { get; set; }
        [XmlElement("end")]
        public string end { get; set; }
        [XmlElement("total")]
        public string total { get; set; }
        [XmlElement("review")]
        public List<Review> reviews { get; set; }
    }

    public class Review
    {
        [XmlElement]
        public string id { get; set; }
        [XmlElement("book")]
        public ReviewBook book { get; set; }
        [XmlElement("rating")]
        public int rating { get; set; }
        [DeserializeAs(Name = "readat")]
        public string readAt { get; set; }
    }

    public class ReviewBook
    {
        [XmlElement("id")]
        public string id { get; set; }
        [DeserializeAs(Name = "titlewithoutseries")]
        public string titleWithoutSeries { get; set; }
        [XmlElement("authors")]
        public List<ReviewAuthor> authors { get; set; }
    }

    public class ReviewAuthor
    {
        [XmlElement("author")]
        public string name { get; set; }
    }

    //#### SEARCH XML DESEREALISATION ####
    [XmlRoot("GoodreadsResponse")]
    public class CatalogueSearch
    {
        [XmlElement("search")]
        public SearchBook search { get; set; }
    }

    public class SearchBook
    {
        [XmlElement("results")]
        public SearchResults results { get; set; }
    }

    public class SearchResults
    {
        [XmlElement("work")]
        public List<SearchWork> work { get; set; }
    }

    public class SearchWork
    {
        [XmlElement("best_book")]
        public SearchBookMatch bestBook { get; set; }
    }

    public class SearchBookMatch
    {
        [XmlElement("id")]
        public string id { get; set; }
    }


}
