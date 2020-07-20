using MVC5_Online_shop.Models.Data;
using MVC5_Online_shop.Models.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC5_Online_shop.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            //confirm if user is authorized
            string userName = User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
                return RedirectToAction("user-profile");

            //return view()
            return View();
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            //check model for validy
            if (!ModelState.IsValid)
                return View("CreateAccount", model);

            //check pass
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password do not match!");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                //check name for unique
                if (db.Users.Any(x => x.UserName.Equals(model.UserName)))
                {
                    ModelState.AddModelError("", $"Username {model.UserName} is taken.");
                    model.UserName = "";
                    return View("CreateAccount", model);
                }

                //create usertDTO
                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    UserName = model.UserName,
                    Password = model.Password
                };

                //add data to userDTO model
                db.Users.Add(userDTO);

                //save data
                db.SaveChanges();

                //add role
                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }

            //write msg to tempdata
            TempData["SM"] = "Your are now registered and can login";

            //redirect user
            return RedirectToAction("Login");
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            //check model for validity
            if (!ModelState.IsValid)
                return View(model);

            //check user for validity
            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.UserName.Equals(model.Username) && x.Password.Equals(model.Password)))
                    isValid = true;
                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }

        }

        //GET: /account/logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult UserNavPartial()
        {
            //get name of user
            string userName = User.Identity.Name;

            //declare model
            UserNavPertialVM model;

            using (Db db = new Db())
            {
                //get user
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                //fill model from context(dto)
                model = new UserNavPertialVM()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };
            }

            //return partial view from model
            return PartialView(model);
        }

        // GET: account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        public ActionResult UserProfile()
        {
            //get user name
            string userName = User.Identity.Name;

            //declare model
            UserProfileVM model;

            using (Db db = new Db())
            {
                //get user
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);

                //init model with data
                model = new UserProfileVM(dto);
            }

            //return model to view
            return View("UserProfile", model);
        }

        // POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        public ActionResult UserProfile(UserProfileVM model)
        {
            bool userNameIsChanged = false;
            //check model for validity
            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            //check password(if user change it)
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Password don't match");
                    return View("UserProfile", model);
                }
            }
            using (Db db = new Db())
            {
                //get username
                string userName = User.Identity.Name;
                
                //check if username is changed
                if(userName != model.UserName)
                {
                    userName = model.UserName;
                    userNameIsChanged = true;
                }

                //check name for unique
                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.UserName == userName))
                {
                    ModelState.AddModelError("", $"Username {model.UserName} already exists.");
                    model.UserName = "";
                    return View("UserProfile", model);
                }

                //change dto model context
                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.UserName = model.UserName;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }

                //save changes
                db.SaveChanges();
            }

            //save message to tempdata
            TempData["SM"] = "You have edited your profile";

            if (!userNameIsChanged)
                //return view with model
                return View("UserProfile", model);
            else
                return RedirectToAction("Logout");
        }
    }
}