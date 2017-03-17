using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tzskSitedb.lib.Entity;

namespace tzskSite.Controllers
{
    public class HomeController : Controller
    {
        tzskSiteEntities dbContext;

        public HomeController()
        {
            dbContext = new tzskSiteEntities();
        }
       
        public ActionResult Index()
        {
            DateTime dt = DateTime.Now;
            int id = 0;
            if (Session["id"] != null)
                id = (int)Session["id"];
            
            var tbUser = dbContext.tbUsers.FirstOrDefault(l => (l.cIsAdmin == true) && (l.id == id) );
            if( tbUser != null)
                return View( dbContext.tbUserData.Where(l => l.cDate >= dt.Date).OrderBy(l => l.cDate).ToList() );
            else 
            {
                tbUser = dbContext.tbUsers.FirstOrDefault( l => (l.cIsAdmin == false) && (l.id == id) );              
                if (tbUser != null)
                    return View( dbContext.tbUserData.Where(l => (l.cUserId == tbUser.id) && (l.cDate >= dt.Date)).OrderBy(l => l.cDate).ToList() );
                return View();
            }
        }
    }
}