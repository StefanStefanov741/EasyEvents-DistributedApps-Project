using ApplicationService.DTOs;
using MVC.ViewModels;
using System;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class UserController : Controller
    {

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                using (SoapService.Service1Client service = new SoapService.Service1Client())
                {
                    if (service.GetUserByEmail(model.email) != null)
                    {
                        //retrun error if user is already registered
                        ModelState.AddModelError("RegistrationError", "Email already registered!");
                        return View();
                    }
                    if (service.GetUserByUsername(model.username) != null) {
                        //retrun error if user is already registered
                        ModelState.AddModelError("RegistrationError", "Username taken!");
                        return View();
                    }
                    if (service.GetUserByDisplayName(model.displayName) != null)
                    {
                        //retrun error if user is already registered
                        ModelState.AddModelError("RegistrationError", "Display name taken!");
                        return View();
                    }
                    UserDTO userDto = new UserDTO
                    {
                        username = model.username,
                        password = model.password,
                        email = model.email,
                        displayName = model.displayName,
                        gender = model.gender,
                        birthday=model.birthday
                        
                    };
                    service.PostUser(userDto);
                    return RedirectToAction("Index","Home");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (SoapService.Service1Client service = new SoapService.Service1Client()) {
                UserDTO tryUser = service.TryLogin(model.username, model.password);
                if (tryUser == null)
                {
                    ModelState.AddModelError("AuthenticationFailed", "Wrong username or password !");
                    return View(model);
                }
            }
            //log in the user
            return RedirectToAction("Index", "Home");
        }

    }//end of class
}//end of namespace