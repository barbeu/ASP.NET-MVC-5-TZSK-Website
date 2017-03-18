using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tzskSite_v1.Models;

namespace tzskSite_v1.Controllers
{
    public class MsgController : Controller
    {
        public ActionResult Message(MsgViewModel model)
        {
            if (model.Message != null)
                return View(model);
            else
                return RedirectToAction("Index", "Home", null);
        }
        
        public ActionResult AuthError()
        {
            if(Session["id"] == null) 
                return View();
            else
                return RedirectToAction("Index", "Home", null);
        }
    }
}