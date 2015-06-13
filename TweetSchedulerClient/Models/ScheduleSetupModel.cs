using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TweetSchedulerClient
{
    public class ScheduleSetupModel
    {
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }

        public string Hours { get; set; }
        public string Minutes { get; set; }
        public string AmPm { get; set; }

        public List<TimeModel> Times { get; set; }

        public string Tweets { get; set; }

        public ScheduleSetupModel()
        {
            Times = new List<TimeModel>();
            Hours = "12";
            Minutes = "00";
            AmPm = "AM";
        }
    }
}