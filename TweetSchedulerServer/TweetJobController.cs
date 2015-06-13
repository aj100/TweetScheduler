using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;

namespace TweetSchedulerServer
{
    public class TweetJobController : ApiController
    {
        [HttpGet]
        public string Get()
        {
            return "It still works!";
        }

        [HttpPost]
        public string ExecuteTweetSchedule(int id)
        {
            try
            {
                //Get the list of tweets
                var tweets = GetTweets(id);

                if (tweets.Count > 0)
                {
                    var topTweet = tweets.First();

                    //Tweet out the top tweet
                    Tweet(topTweet.TweetText);

                    //Delete the top tweet
                    DeleteTweet(topTweet.Id);
                }

                return "Success";
            }
            catch (Exception ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);

                while(ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(ex.StackTrace);
                }

                return sb.ToString();
            }
        }

        private List<TweetRecord> GetTweets(int batchId)
        {
            using (var conn = new SqlConnection(TweetServicesResources.connString))
            {
                conn.Open();

                var cmdString = string.Format("SELECT Id, TweetString FROM Tweets WHERE BatchId = {0} ORDER BY Id", batchId);

                using (var command = new SqlCommand(cmdString, conn))
                {
                    var reader = command.ExecuteReader();
                    List<TweetRecord> tweetRecords = new List<TweetRecord>();
                    while (reader.Read())
                    {
                        tweetRecords.Add(new TweetRecord
                        {
                            Id = (int)reader["Id"],
                            TweetText = (string)reader["TweetString"]
                        });
                    }

                    return tweetRecords;
                }

            }
        }

        private void Tweet(string tweetText)
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = TweetServicesResources.consumerKey,
                    ConsumerSecret = TweetServicesResources.consumerSecret,
                    AccessToken = TweetServicesResources.accessToken,
                    AccessTokenSecret = TweetServicesResources.accessTokenSecret
                }
            };

            var twitterCtx = new TwitterContext(auth);

            twitterCtx.TweetAsync(tweetText).Wait();
        }

        private void DeleteTweet(int id)
        {
            using (var conn = new SqlConnection(TweetServicesResources.connString))
            {
                conn.Open();

                var cmdString = string.Format("DELETE Tweets WHERE Id = {0}", id);

                using (var command = new SqlCommand(cmdString, conn))
                {
                    command.ExecuteNonQuery();
                }

            }
        }
    }
}