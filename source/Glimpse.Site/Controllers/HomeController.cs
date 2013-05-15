﻿using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Glimpse.Site.Controllers
{
    public partial class HomeController : Controller
    {
        public virtual ActionResult Index()
        {
            return View(MVC.Home.Views.Index, MVC.Shared.Views._Home);
        }

        [OutputCache(Duration = 3600)] // Cache for 1 hour
        public async virtual Task<ActionResult> BuildStatus()
        {
            var httpClient = new HttpClient();

            var response = await httpClient.GetStringAsync("http://teamcity.codebetter.com/app/rest/builds/buildType:%28id:bt428%29?guest=1");
            var xml = XElement.Parse(response);

            DateTimeOffset date;
            DateTimeOffset.TryParseExact(xml.Element("startDate").Value, "yyyyMMddTHHmmsszz00", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

            return Json(
                new
                {
                    id = xml.Attribute("id").Value, 
                    status = xml.Attribute("status").Value.ToLower(), 
                    link = xml.Attribute("webUrl").Value,
                    date = date.DateTime.ToShortDateString(),
                    time = date.DateTime.ToShortTimeString()
                }, 
                JsonRequestBehavior.AllowGet);
        }
    }
}