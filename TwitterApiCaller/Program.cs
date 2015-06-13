using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterApiCaller
{
    class Program
    {
        static void Main(string[] args)
        {
            //var auth = new PinAuthorizer()
            //{
            //    CredentialStore = new InMemoryCredentialStore
            //    {
            //        ConsumerKey = Properties.Resources.consumerKey,
            //        ConsumerSecret = Properties.Resources.consumerSecret,
            //    },
            //    GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
            //    GetPin = () =>
            //    {
            //        Console.WriteLine(
            //            "\nAfter authorizing this application, Twitter " +
            //            "will give you a 7-digit PIN Number.\n");
            //        Console.Write("Enter the PIN number here: ");
            //        return Console.ReadLine();
            //    }
            //};

            //auth.AuthorizeAsync().Wait();

            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = Properties.Resources.consumerKey,
                    ConsumerSecret = Properties.Resources.consumerSecret,
                    AccessToken = Properties.Resources.accessToken2,
                    AccessTokenSecret = Properties.Resources.accessTokenSecret2
                    //AccessToken = Properties.Resources.accessToken,
                    //AccessTokenSecret = Properties.Resources.accessTokenSecret
                }
            };
            

            var twitterCtx = new TwitterContext(auth);

            var searchResults = SearchTwitter(twitterCtx);
            
            
        
        }

        static void CreateTweet(TwitterContext twitterCtx)
        {
            var tweetResult = twitterCtx.TweetAsync("Testing out Linq2Twitter!").Result;
        }

        static List<Search> SearchTwitter(TwitterContext twitterCtx)
        {
            var results = twitterCtx.Search
                .Where(s => s.Type == SearchType.Search && s.Query == "\"LINQ to Twitter\" OR Linq2Twitter OR LinqToTwitter").ToList();

            return results;

            //if (searchResponse != null && searchResponse.Statuses != null)
            //    searchResponse.Statuses.ForEach(tweet =>
            //        Console.WriteLine(
            //            "\n  User: {0} ({1})\n  Tweet: {2}",
            //            tweet.User.ScreenNameResponse,
            //            tweet.User.UserIDResponse,
            //            tweet.Text));
        }

    }
}
