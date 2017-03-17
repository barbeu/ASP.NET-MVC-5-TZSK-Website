using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tzskSitedb.lib.Entity;

namespace tzskSite_v1.Controllers
{
    public class MapController : Controller
    {
        [HttpPost]
        public ActionResult Map(string lon, string lat, string address)
        {
            coordinates coord = new coordinates();
            coord.lon = lon;
            coord.lat = lat;
            coord.address = address;

            return View(coord);
        }

        public struct coordinates
        {
            public string lon;
            public string lat;
            public string address;
        }
    }
}