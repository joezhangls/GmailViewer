using GmailViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GmailViewer.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Mseeages value = new Mseeages();
            value.getlist();
            return View(value.ListMinimal);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}