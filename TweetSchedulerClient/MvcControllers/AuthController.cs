//See here for full example: https://linqtotwitter.codeplex.com/wikipage?title=Implementing%20OAuth%20for%20ASP.NET%20MVC&referringTitle=Learning%20to%20use%20OAuth
//Removed LinqToTwitter reference
//using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TweetSchedulerClient
{
    public class AuthController : Controller
    {
        public void BeginAuth()
        {
            //var auth = new MvcAuthorizer
            //{
            //    CredentialStore = new SessionStateCredentialStore
            //    {
            //        ConsumerKey = CoolStuff.consumerKey,
            //        ConsumerSecret = CoolStuff.consumerSecret
            //    }
            //};

            //string twitterCallbackUrl = Request.Url.ToString().ToLowerInvariant().Replace("begin", "complete");
            //return await auth.BeginAuthorizationAsync(new Uri(twitterCallbackUrl)); 
        }

        public ActionResult CompleteAuth()
        {
            //var auth = new MvcAuthorizer
            //{
            //    CredentialStore = new SessionStateCredentialStore()
            //};

            //await auth.CompleteAuthorizeAsync(Request.Url);

            //var credentials = auth.CredentialStore;
            //string oauthToken = credentials.OAuthToken;
            //string oauthTokenSecret = credentials.OAuthTokenSecret;
            //string screenName = credentials.ScreenName;
            //ulong userID = credentials.UserID;

            //var commandStr = "INSERT UserAuth(UserId, ScreenName, OauthToken, OauthTokenSecret) " +
            //    string.Format("VALUES ({0}, '{1}', '{2}', '{3}')", userID, screenName, oauthToken, oauthTokenSecret);

            //using(var conn = new SqlConnection(CoolStuff.connString))
            //using(var command = new SqlCommand(commandStr, conn))
            //{
            //    conn.Open();
            //    command.ExecuteNonQuery();
            //}
            
            return RedirectToAction("Success", "Main");
        }
    }
}