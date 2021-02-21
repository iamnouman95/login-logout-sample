using Models;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace WebAPI.Controllers
{
    public class AccountController : ApiController
    {
        //As login data is hard coded within the server 
        //i.e. why Dictionary is efficient way of fetching users
        public Dictionary<string, string> Users;

        public AccountController()
        {
            AddUsers();
        }

        [HttpPost]
        public ApiResponse Login( LoginModel model)
        {
            return CheckLogin(model);
        }

        private ApiResponse CheckLogin( LoginModel model)
        {
            try
            {
                ApiResponse result;
                if (Users.ContainsKey(model.Username))
                {
                    var password = Users[model.Username];
                    if (password == model.Password)
                    {
                        result = new ApiResponse
                        {
                            success = true,
                        };
                    }
                    else
                    {
                        result = new ApiResponse
                        {
                            success = false,
                            errorMessage = "Password do not match"
                        };
                    }
                }
                else
                {
                    result = new ApiResponse
                    {
                        success = false,
                        errorMessage = "Username not found"
                    };
                }
                
                return result;
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    success = false,
                    errorMessage = "Contact Admin" + " - " + ex.Message
                };
            }
        }
        private void AddUsers()
        {
            //Hardcoded users -> as required by exercise document
            Users = new Dictionary<string, string>();
            Users.Add("nouman", "1234");
            Users.Add("micheal", "1234");
            Users.Add("guest", "1234");
        }
    }
}
