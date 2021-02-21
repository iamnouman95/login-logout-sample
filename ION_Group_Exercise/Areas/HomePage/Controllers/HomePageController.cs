using Models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace LoginLogoutSample.Areas.HomePage.Controllers
{
    public class HomePageController : Controller
    {
        /// <summary>
        /// The method Login is the 
        /// landing page of the web application,
        /// which has Layout.cshtml integrated
        /// </summary>
        /// <param name="res"></param>
        /// <returns>Return a view</returns>
        public ActionResult Login(ApiResponse res = null)
        {
            //If session has already values it means the user is signed in hense always 
            //return the Frame
            if (Session["LoggedInUser"] != null)
            {
                return RedirectToAction("Frame");
            }

            return View(res);
        }

        /// <summary>
        /// The method Logout clears session memory
        /// and redirect user to login page
        /// </summary>
        /// <returns>Returns a view</returns>
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// The Frame method is only shown to authorized users
        /// after Login is successfull
        /// </summary>
        /// <returns>Return a view</returns>
        public ActionResult Frame()
        {
            if (Session["LoggedInUser"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


        /// <summary>
        /// CheckLogin is a method to authenticate the user
        /// credentials after receiving response from web api
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// The function return either a view and/or 
        /// Json message in case of exceptions
        /// </returns>
        [HttpPost]
        public object CheckLogin(LoginModel model)
        {
            ApiResponse res = null, errResult;
            try
            {
                var api = ConfigurationManager.AppSettings["apiPath"].ToString();
                var url = "https://" + api + "/api/Account/";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

                    //Post call to web api
                    var responseTask = client.PostAsync("CheckLogin", stringContent);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();

                        var response = readTask.Result;
                        res = JsonConvert.DeserializeObject<ApiResponse>(response);
                    }
                }

                //If the response from web api is a success i.e. Login ok
                if (res.success == true)
                {
                    Session.Add("LoggedInUser", model.Username);
                    return RedirectToAction("Frame");
                }
                else
                {
                    return Json(res);
                }
            }
            catch (WebException exc) //If any type of network exception occurs then return proper Jsonmessage which is handled in Login.js
            {
                errResult = new ApiResponse
                {
                    success = false,
                    errorMessage = "Network Error: " + exc.Message +
                                  "\nStatus code: " + exc.Status
                };
                return Json(errResult);
            }
            catch (Exception ex)
            {
                errResult = new ApiResponse
                {
                    success = false,
                    errorMessage = ex.Message
                };
                return Json(errResult);
            }
        }
    }
}