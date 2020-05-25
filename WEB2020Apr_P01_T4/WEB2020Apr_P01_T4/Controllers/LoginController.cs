using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WEB2020Apr_P01_T4.Controllers
{
    public class LoginController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //Staff Login credentails
        [HttpPost]
        public ActionResult Stafflogin(IFormCollection formData)
        {
            //Email address converted to lowercase
            string loginID = formData["txtLoginID"].ToString().ToLower();
            string password = formData["txtPassword"].ToString();

            if (loginID == "s1234567@ica.com" && password == "p@55Staff")
            {
                //Store Login ID in session with the key "LoginID"
                HttpContext.Session.SetString("LoginID", loginID);

                //Store the user role "Staff" as a string in session with the key "Role"
                HttpContext.Session.SetString("Role", "Staff");

                //Redirect use to the Staff Main 
                return RedirectToAction("StaffMain");
            }
            else
            {
                //Store an error message in TempData for display at the index view
                TempData["Message"] = "Invalid Login Credentials!";

                //Redirect user back to the index view through an action
                return RedirectToAction("Index");
            }
        }

        public ActionResult StaffMain()
        {
            //Stop accessing the action if not logged in
            //or account not in the  "Staff" Role
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Staff"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public ActionResult Logout()
        {
            //Clear all key-values pairs stored in session state
            HttpContext.Session.Clear();
            //Call the Index action of home Controller
            return RedirectToAction("Index");
        }

        //Customer Login Credentials        
    }
}
