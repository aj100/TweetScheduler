using Microsoft.Azure;
using Microsoft.WindowsAzure.Management.Scheduler;
using Microsoft.WindowsAzure.Management.Scheduler.Models;
using Microsoft.WindowsAzure.Scheduler;
using Microsoft.WindowsAzure.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Xml.Linq;

namespace TweetSchedulerClient.Helpers
{
    public static class AzureHelpers
    {
        //Code from http://fabriccontroller.net/blog/posts/a-complete-overview-to-get-started-with-the-windows-azure-scheduler/
        private static CertificateCloudCredentials FromPublishSettingsFile(string directory)
        {
            var profile = XDocument.Load(Path.Combine(directory,  "<Your Settings File>"));
            var subscriptionId = profile.Descendants("Subscription")
                .First(element => element.Attribute("Name").Value == "<Your Subscription Name>")
                .Attribute("Id").Value;
            var certificate = new X509Certificate2(
                Convert.FromBase64String(profile.Descendants("PublishProfile").Descendants("Subscription").Single().Attribute("ManagementCertificate").Value));
            return new CertificateCloudCredentials(subscriptionId, certificate);
        }

        private static void CreateOrVerifyJobCollection(CertificateCloudCredentials credentials)
        {
            var cloudServiceClient = new CloudServiceManagementClient(credentials);

            if (!cloudServiceClient.CloudServices.List().Any(c => c.Name == "tweet-sched-cs"))
            {
                var result = cloudServiceClient.CloudServices.Create("tweet-sched-cs", new CloudServiceCreateParameters()
                {
                    Description = "My Tweet scheduling cloud service",
                    GeoRegion = "east us",
                    Label = "tweet-sched-cs"
                });
            }

            var schedulerServiceClient = new SchedulerManagementClient(credentials);

            if (schedulerServiceClient.JobCollections.CheckNameAvailability("tweet-sched-cs", "TweetJobCollection").IsAvailable)
            {
                schedulerServiceClient.JobCollections.Create("tweet-sched-cs", "TweetJobCollection", new JobCollectionCreateParameters
                {
                    Label = "Tweet Job Collection"
                });
            }
        }

        private static void CreateJob(CertificateCloudCredentials credentials, ScheduleSetupModel model)
        {
            var dayValues = new List<JobScheduleDay>();
            var properties = model.GetType().GetProperties();

            //Getting days of the week using reflection
            //doing this to avoid having seven if statements
            foreach(var day in Enum.GetNames(typeof(JobScheduleDay)))
            {
                var dayProp = properties.FirstOrDefault(p => p.Name == day);
                if(dayProp != null)
                {
                    if((bool)dayProp.GetValue(model))
                    {
                        dayValues.Add((JobScheduleDay)Enum.Parse(typeof(JobScheduleDay), day));
                    }
                }
            }

            Func<TimeModel, int> fixHour = t =>
            {
                var hour = int.Parse(t.Hour);

                if (t.AmPm == "AM" && t.Hour == "12")
                    return 0;
                if (t.AmPm == "PM" && t.Hour != "12")
                    return hour + 12;
                
                return hour;
            };


            var times = model.Times.Select(t => new DateTime(2015, 5, 5, 
                fixHour(t),
                int.Parse(t.Minute),
                0));

            times = times.Select(t => t.AddHours(4));

            var hours = times.Select(t => t.Hour).ToList();
            var minutes = times.Select(t => t.Minute).ToList();

            var schedulerClient = new SchedulerClient("tweet-sched-cs", "TweetJobCollection", credentials);

            var batchId = InsertTweetBatch(model.Tweets);
            
            schedulerClient.Jobs.Create(new Microsoft.WindowsAzure.Scheduler.Models.JobCreateParameters
            {
                Action = new JobAction
                {
                    Type = JobActionType.Http,
                    Request = new JobHttpRequest
                    {
                        Method = "POST",
                        Uri = new Uri("http://tweetschedbackend.azurewebsites.net/api/TweetJob/" + batchId)
                    }
                },
                Recurrence = new JobRecurrence
                {
                    Frequency = JobRecurrenceFrequency.Week,
                    Schedule = new JobRecurrenceSchedule()
                    {
                        Days = dayValues,
                        Hours = hours,
                        Minutes = minutes
                    }
                },
                StartTime = DateTime.UtcNow
            });
        }

        private static int InsertTweetBatch(string tweetsString)
        {
            var tweets = tweetsString.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (tweets.Length == 0)
                return 0;

            int id = 0;

            using (var conn = new SqlConnection(CoolStuff.connString))
            {
                conn.Open();

                var groupSql = string.Format("INSERT TweetScheduleGroup(Comments) VALUES('{0}') SELECT MAX(Id) FROM TweetScheduleGroup", 
                    DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
                using(var command = new SqlCommand(groupSql, conn))
                {
                    id = (int)command.ExecuteScalar();
                }

                foreach (var tweet in tweets)
                {
                    var tweetSql = string.Format("INSERT Tweets(BatchId, TweetString) VALUES({0}, '{1}')", id, tweet);
                    using (var command = new SqlCommand(tweetSql, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            return id;
        }

        public static void AddNewScheduledJob(string serverDirectory, ScheduleSetupModel model)
        {
            if (string.IsNullOrEmpty(model.Tweets))
                return;

            var credentials = FromPublishSettingsFile(serverDirectory);
            CreateOrVerifyJobCollection(credentials);
            CreateJob(credentials, model);
        }
    }
}