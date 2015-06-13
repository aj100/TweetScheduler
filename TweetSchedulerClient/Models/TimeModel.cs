using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TweetSchedulerClient
{
    public class TimeModel
    {
        public string Hour {get; set;}
        public string Minute { get; set; }
        public string AmPm { get; set; }

        public TimeModel()
        {
            Hour = "12";
            Minute = "00";
            AmPm = "AM";
        }

        public TimeModel(string hour, string minute, string amPm)
        {
            Hour = hour;
            Minute = minute;
            AmPm = amPm;
        }
    }
}