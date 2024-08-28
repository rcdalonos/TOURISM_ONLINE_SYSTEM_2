using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1;


using System.Globalization;

using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebApplication1.Models;

using System.Data.SqlClient;
using System.Configuration;

using System.Web.Security;
using WebApplication1.DAL;

using System.IO;
using System.Net.Mail;

using WebApplication1.MyDataSetTableAdapters;

using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Web.Helpers;

using Microsoft.AspNet.Identity.EntityFramework;

//using Microsoft.AspNetCore;
using SendGrid;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;

namespace WebApplication1.Controllers
{
    public class UserRegsConController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private ONLINE_DATABASE_SYSTEMEntities db = new ONLINE_DATABASE_SYSTEMEntities();
        
        public static string GenerateEmailToken()
        {
            // generate an email verification token for the user
            using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
            {
                byte[] data = new byte[16];
                provider.GetBytes(data);
                return Convert.ToBase64String(data);
            }
        }

        //public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(Microsoft.AspNet.Identity.IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        // GET: UserRegsCon
        public ActionResult Index()
        {
            return View(db.UserRegs.ToList());
        }

        // GET: UserRegsCon/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserReg userReg = db.UserRegs.Find(id);
            if (userReg == null)
            {
                return HttpNotFound();
            }
            return View(userReg);
        }

        // GET: UserRegsCon/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserRegsCon/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public  Task<ActionResult> Create([Bind(Include = "Userid,UserName,Email,UserPassword,UserRePassword,Firstname,Lastname,Gender,Age,ContactNumber,Address,imgname,imgext,imgpath,AccountRole,EmailConfirmed")] UserReg userReg)
        public ActionResult Create (UserReg user)
        {
           
            if (ModelState.IsValid)
            {
                ONLINE_DATABASE_SYSTEMEntities _ONLINE_DATABASE_SYSTEMEntities = new ONLINE_DATABASE_SYSTEMEntities();
                _ONLINE_DATABASE_SYSTEMEntities.UserRegs.Add(user);
                _ONLINE_DATABASE_SYSTEMEntities.SaveChanges();
                string message = string.Empty;
                switch (user.Userid)
                {
                    case -1:
                        ViewData["UserAccountNotAvailable"] = "Username already exists.Please choose a different username.";
                        
                        break;
                    case -2:
                        ViewData["UserAccountNotAvailable"]  = "Supplied email address has already been used.";
                        break;
                    default:
                        ViewData["UserAccountNotAvailable"]  = "Registration Successful.";
                        TempData["UserAccountSuccessMessage"] = "Registration Successful. Verify Your Email Before You Can Login!";
                        SendActivationEmail(user);
                    return RedirectToAction("Index", "UserLogin");
                    //break;
                }

                return View(user);
            }

            return View(user);
        }


        public ActionResult Activation()
        {
            ViewBag.Message = "Invalid Activation code.";
            if (RouteData.Values["id"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());

                string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();
                using (SqlConnection connection = new SqlConnection(constring))
                {

                    SqlCommand command = new SqlCommand("sp_UserAccountEmailConfirmed", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@activationCode", activationCode);

                    SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                    DataTable dtProducts = new DataTable();
                    //IDinserted = (int)command.ExecuteScalar();

                    connection.Open();

                    command.ExecuteNonQuery();
                    ViewBag.Message = "Activation successful.";
                    
                    connection.Close();
                   
                }
            }
            return View();
        }

        // GET: UserRegsCon/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserReg userReg = db.UserRegs.Find(id);
            if (userReg == null)
            {
                return HttpNotFound();
            }
            return View(userReg);
        }

        // POST: UserRegsCon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Userid,UserName,Email,UserPassword,UserRePassword,Firstname,Lastname,Gender,Age,ContactNumber,Address,imgname,imgext,imgpath,AccountRole,EmailConfirmed")] UserReg userReg)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userReg).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userReg);
        }

        // GET: UserRegsCon/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserReg userReg = db.UserRegs.Find(id);
            if (userReg == null)
            {
                return HttpNotFound();
            }
            return View(userReg);
        }

        // POST: UserRegsCon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserReg userReg = db.UserRegs.Find(id);
            db.UserRegs.Remove(userReg);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        private void SendActivationEmail(UserReg user)
        {
            Guid activationCode = Guid.NewGuid();
            ONLINE_DATABASE_SYSTEMEntities usersEntities = new ONLINE_DATABASE_SYSTEMEntities();
            usersEntities.UserActivations.Add(new UserActivation
            {
                UserId = user.Userid,
                ActivationCode = activationCode
            });
            usersEntities.SaveChanges();

            using (MailMessage mm = new MailMessage("ghooqmail@gmail.com", user.Email))
            {
                mm.Subject = "Account Activation";
                string body = "Hello " + user.UserName + ",";
                body += "<br /><br />Please click the following link to activate your account";
                body += "<br /><a href = '" + string.Format("{0}://{1}/UserRegsCon/Activation/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Click here to activate your account.</a>";
                body += "<br /><br />Thanks";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("ghooqmail@gmail.com", "pxhpnqvmbmbgjxws");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                //smtp.UseDefaultCredentials = false;
                smtp.Send(mm);
            }
        }

        #endregion


    }
}
