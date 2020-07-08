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
            //Get developer keys and secret
            //DeveloperKey = Environment.GetEnvironmentVariable("goodreads_key");
            //DeveloperSecret = Environment.GetEnvironmentVariable("goodreads_secret");
			DeveloperKey = "FZCOErQOPJ5lfAzWOUNUA";
			DeveloperSecret = "naiIe3c4Kcf7wqYENsM27kkrxDjOKxJxgocUzGueJo";
            Console.WriteLine(DeveloperKey);
            Console.WriteLine(DeveloperSecret);
            Client.Authenticator = OAuth1Authenticator.ForRequestToken(DeveloperKey, DeveloperSecret);
        }

        public string GenerateAuthURL()
        {

			//Get OAuth tokens
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

        public List<MediaModel> Pull(NSProgressIndicator Progress)
        {
            //Get all user books from goodreads account

            //Loop through record pages and extract books
            var books = new List<MediaModel>();
            int page = 1,counter=1,counterMax;
            string endRecord, totalRecord;
            do
            {
				Progress.DoubleValue = 10;
                var request = new RestRequest(string.Format("review/list/{0}.xml", UserID), DataFormat.Xml);
                request.AddParameter("v", 2);
                request.AddParameter("id", UserID);
                request.AddParameter("shelf", "read");
                request.AddParameter("page", page);
                request.AddParameter("per_page", 200);
                request.AddParameter("key", DeveloperKey);
                var response = ExecuteGetRequest<ShelfResponse>(request);
                counterMax = Convert.ToInt32(response.Data.reviews.end);

                foreach (Review review in response.Data.reviews.reviews)
                {

                    string BookTitle = review.book.titleWithoutSeries;
                    int BookRating = review.rating;
                    string BookAuthor = "";
                    string BookDate = "";
                    foreach (ReviewAuthor reviewAuthor in review.book.authors)
                    {
                        BookAuthor += reviewAuthor.name;
                    }
                    try
                    {
                        string dateTmp = "";
                        for (int i = 0; i < review.readAt.Length; ++i)
                        {
                            if (i < 20 || i > 25) dateTmp += review.readAt[i];
                        }
                        DateTime dt = DateTime.ParseExact(dateTmp, "ddd MMM dd HH:mm:ss yyyy", CultureInfo.InvariantCulture);
                        BookDate = dt.ToString("yyyy-MM-dd");
                    }
                    catch
                    {
                        BookDate = "";
                    }
                    string GoodreadsID = review.book.id;

                    var book = new MediaModel("Book",BookTitle,BookAuthor,"",BookDate,BookRating,GoodreadsID);
                    books.Add(book);
                    Progress.DoubleValue = 10 + 40 * counter / counterMax;
                    ++counter;
                }
                endRecord = response.Data.reviews.end;
                totalRecord = response.Data.reviews.end;
                ++page;
            } while (endRecord != totalRecord);

            return books;
        }


        public IRestResponse<T> ExecuteGetRequest<T>(RestRequest request)
        {
            request.XmlSerializer = new RestSharp.Serializers.DotNetXmlSerializer();
            var response = Client.Execute<T>(request);
            return response;
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


}
