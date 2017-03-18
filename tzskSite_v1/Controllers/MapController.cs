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
            if (Session["id"] == null)
                return RedirectToAction("Message", "Msg", null);

            if (lon == null || lat == null || address == null )           
                return RedirectToAction("Index", "Home", null);

            //TODO: FixMe если отправлять пост запрос с левыми  данными (не через кнопку) будет отображаться
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