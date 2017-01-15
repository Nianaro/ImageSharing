using ImageSharingApp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageSharingApp.Controllers
{
    [HandleError() ]
    [Culture]
    public class StaticPagesController : Controller
    {
        //
        // GET: /StaticPages/

        public ActionResult About()
        {
            return View();
        }

    }
}
