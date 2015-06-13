using Microsoft.Azure;
using Microsoft.WindowsAzure.Management.Scheduler;
using Microsoft.WindowsAzure.Management.Scheduler.Models;
using Microsoft.WindowsAzure.Scheduler;
using Microsoft.WindowsAzure.Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AzureApiCaller
{
    class Program
    {
        //Code taken from http://fabriccontroller.net/blog/posts/a-complete-overview-to-get-started-with-the-windows-azure-scheduler/
        public static CertificateCloudCredentials FromPublishSettingsFile(string path, string subscriptionName)
        {
            var profile = XDocument.Load(path);
            var subscriptionId = profile.Descendants("Subscription")
                .First(element => element.Attribute("Name").Value == subscriptionName)
                .Attribute("Id").Value;
            var certificate = new X509Certificate2(
                Convert.FromBase64String(profile.Descendants("PublishProfile").Descendants("Subscription").Single().Attribute("ManagementCertificate").Value));
            return new CertificateCloudCredentials(subscriptionId, certificate);
        }

        static void Main(string[] args)
        {
            //Create a new one from Azure...
            var credentials = FromPublishSettingsFile(@"<Your Publish Settings File>", "<Your Azure Subscription Name>");

            SetupCloudServiceAndJobCollection(credentials);

            var schedulerClient = new SchedulerClient("anurag-cs", "MyProgrammaticJobCollection", credentials);
            schedulerClient.Jobs.Create(new Microsoft.WindowsAzure.Scheduler.Models.JobCreateParameters
            {
                Action = new JobAction
                {
                    Type = JobActionType.Http,
                    Request = new JobHttpRequest
                    {
                        Method = "GET",
                        Uri = new Uri("http://thecatapi.com/api/images/get")
                    }
                },
                Recurrence = new JobRecurrence
                {
                    Frequency = JobRecurrenceFrequency.Week,
                    Schedule = new JobRecurrenceSchedule()
                    {
                        Days = new List<JobScheduleDay>() { JobScheduleDay.Saturday },
                        Hours = new List<int>() { 20 },
                        Minutes = new List<int>() { 45, 46, 47, 48, 49, 50, 51, 52 }
                    }
                },
                StartTime = DateTime.UtcNow
            });

        }

        private static void SetupCloudServiceAndJobCollection(CertificateCloudCredentials credentials)
        {
            var cloudServiceClient = new CloudServiceManagementClient(credentials);

            if (cloudServiceClient.CloudServices.List().Any(c => c.Name == "anurag-cs"))
            {
                cloudServiceClient.CloudServices.Delete("anurag-cs");
            }

            var result = cloudServiceClient.CloudServices.Create("anurag-cs", new CloudServiceCreateParameters()
            {
                Description = "anurag-cs",
                GeoRegion = "east us",
                Label = "anurag-cs"
            });

            var schedulerServiceClient = new SchedulerManagementClient(credentials);

            if (!schedulerServiceClient.JobCollections.CheckNameAvailability("anurag-cs", "MyProgrammaticJobCollection").IsAvailable)
            {
                schedulerServiceClient.JobCollections.Delete("anurag-cs", "MyProgrammaticJobCollection");
            }

            schedulerServiceClient.JobCollections.Create("anurag-cs", "MyProgrammaticJobCollection", new JobCollectionCreateParameters
            {
                Label = "MyProgrammaticJobCollection",
            });
        }
    }
}
