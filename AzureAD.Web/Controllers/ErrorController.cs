using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureAD.Web.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return Content("There was an error with that request");
        }

		public ActionResult AuthFailure(string message)
		{
			return Content("There was an error authenticating your account for the following reason: " + message);
		}
	}
}