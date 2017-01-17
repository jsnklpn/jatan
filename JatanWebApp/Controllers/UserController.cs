using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using JatanWebApp.Helpers;
using JatanWebApp.Models.DAL;
using JatanWebApp.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace JatanWebApp.Controllers
{
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        // GET: /User/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /User/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "You have been locked out.");
                    return View(model);
                //case SignInStatus.RequiresVerification:
                    //return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        // POST: /User/Logoff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: User/Settings
        [Authorize]
        public ActionResult Settings()
        {
            var vm = new UserSettingsViewModel();

            using (var db = new JatanDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                if (user != null)
                {
                    var userImage = user.UserImage;
                    if (userImage != null)
                    {
                        vm.UserImageName = userImage.UserFileName;
                        vm.UserImagePath = userImage.ImagePath;
                    }
                }
                
            }

            return View(vm);
        }

        // POST: User/Settings
        [Authorize]
        [HttpPost]
        public ActionResult Settings(UserSettingsViewModel viewModel, HttpPostedFileBase avatarFile)
        {
            try
            {
                if (avatarFile != null)
                {
                    if (avatarFile.ContentLength == 0)
                        throw new Exception("The file is empty.");
                    if (avatarFile.ContentLength > 2000000)
                        throw new Exception("The filesize exceeds 2MB.");

                    var avatarPhysicalPath = HttpContext.Server.MapPath(@"~/Content/Images/avatars/");
                    if (!Directory.Exists(avatarPhysicalPath))
                        Directory.CreateDirectory(avatarPhysicalPath);

                    string physicalFileName = string.Format("{0}{1}.{2}", User.Identity.Name, DateTime.UtcNow.Ticks, "jpg");
                    string physicalFilePath = Path.Combine(avatarPhysicalPath, physicalFileName);
                    string userFileName = avatarFile.FileName;

                    System.Drawing.Bitmap resizedImage = null;
                    try
                    {
                        resizedImage = ImageHelper.ResizeImage(avatarFile.InputStream, 128, 128);
                    }
                    catch
                    {
                        throw new Exception("The image type is not supported.");
                    }

                    resizedImage.Save(Path.GetFullPath(physicalFilePath), ImageFormat.Jpeg);

                    using (var db = new JatanDbContext())
                    {
                        var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                        if (user != null)
                        {
                            var newUserImage = new UserImage()
                            {
                                ImagePath = @"/Content/Images/avatars/" + physicalFileName,
                                UserFileName = userFileName
                            };
                            db.UserImages.Add(newUserImage);
                            db.SaveChanges();
                            user.UserImageId = newUserImage.Id;
                            db.SaveChanges();

                            viewModel.UserImageName = newUserImage.UserFileName;
                            viewModel.UserImagePath = newUserImage.ImagePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "Failed to upload image. " + ex.Message;
                return View(viewModel);
            }

            TempData["success"] = "Settings saved successfully.";
            return View(viewModel);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}