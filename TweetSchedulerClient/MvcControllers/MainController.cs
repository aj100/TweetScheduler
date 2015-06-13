using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TweetSchedulerClient.Helpers;

namespace TweetSchedulerClient
{
    public class MainController : Controller
    {
        [HttpGet]
        public ViewResult Main(ScheduleSetupModel model = null)
        {
            if(model == null)
                model = new ScheduleSetupModel();
                        
            return View("/Main.cshtml", model);
        }

        [HttpPost, HttpParamAction]
        public ViewResult AddTime(ScheduleSetupModel model)
        {
            model.Times.Add(new TimeModel(model.Hours, model.Minutes, model.AmPm));

            return View("/Main.cshtml", model);
        }

        [HttpPost, HttpParamAction]
        public ViewResult AddSchedule(ScheduleSetupModel model)
        {
            if(model.Times.Count == 0)
            {
                model.Times.Add(new TimeModel(model.Hours, model.Minutes, model.AmPm));
            }

            var dir = HttpContext.Server.MapPath("~");
            AzureHelpers.AddNewScheduledJob(dir, model);

            return View("/Main.cshtml", model);
        }

        public string Success()
        {
            return "Success";
        }
    }
}