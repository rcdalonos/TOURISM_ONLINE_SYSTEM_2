using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using WebApplication1.Models;
using System.Web.Security;
using WebApplication1.DAL;

using System.IO;
using System.Net.Mail;


using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Web.Helpers;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

//using Microsoft.AspNetCore;
using SendGrid;
using System.Net;
using System.Diagnostics;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
//using WebApplication1.Models;
using WebApplication1;
using System.Threading.Tasks;
using CaptchaMvc.HtmlHelpers;

namespace WebApplication1.Controllers
{
    public class UserLoginController : Controller
    {
 

        public static string connectionString = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();
        public static LoginModelClass newLogInModel;

        UserLoginDAL _userLoginDAL = new UserLoginDAL();


        public ActionResult signout()
        {
            Session["username"] = null;
            Session["userid"] = null;
            Session["userimagepath"] = null;
            Session["accountrole"] = null;

            return RedirectToAction("Index", "UserLogin");
        }
        public ActionResult logout()
        {
            Session["username"] = null;
            Session["userid"] = null;
            Session["UserImagePath"] = null;
            Session["accountrole"] = null;

            return RedirectToAction("Index", "UserLogin");
        }

        public ActionResult EditUserAccount()
        {
           
                string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();

                List<LoginModelClass> useracountlist = new List<LoginModelClass>();
                using (SqlConnection connection = new SqlConnection(constring))
                {


                    SqlCommand command = new SqlCommand("sp_GetUserAccountId", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Userid", Session["userid"].ToString());

                    //command.Parameters.AddWithValue("@userid", userid);
                    SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                    DataTable dtProducts = new DataTable();
                    connection.Open();
                    sqlDA.Fill(dtProducts);
                    connection.Close();

                    foreach (DataRow dr in dtProducts.Rows)
                    {
                        useracountlist.Add(new LoginModelClass
                        {
                            Userid = Convert.ToInt32(dr["userid"].ToString()),
                            UserName = dr["UserName"].ToString(),
                            Email = dr["Email"].ToString(),
                            Password = dr["UserPassword"].ToString(),
                            UserRePassword = dr["UserRePassword"].ToString(),
                            Firstname = dr["Firstname"].ToString(),
                            Lastname = dr["Lastname"].ToString(),
                            genderid = dr["Gender"].ToString(),
                            Age = Convert.ToInt32(dr["Age"]),
                            ContactNumber = dr["ContactNumber"].ToString(),
                            Address = dr["Address"].ToString(),
                            imgname = dr["imgname"].ToString(),
                            imgext = dr["imgext"].ToString(),
                            imgpath = dr["imgpath"].ToString()

                        });
                    }
                }
                //return useracountlist;
                return View(useracountlist.FirstOrDefault());

        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult EditUserAccount(LoginModelClass loginModelClass, HttpPostedFileBase file)
        {

            //===image
            string ImagePathFolder = loginModelClass.imgpath;
            if (file != null)
            {
                loginModelClass.imgname = Path.GetFileName(loginModelClass.file.FileName);
                loginModelClass.imgext = Path.GetExtension(loginModelClass.imgname);
                ImagePathFolder = Path.Combine(Server.MapPath("~/UserImages/"), Path.GetFileName(loginModelClass.file.FileName));
                loginModelClass.imgpath = "~/UserImages/" + Path.GetFileName(loginModelClass.file.FileName);
                //====

            }
            //else
            //{
            //    loginModelClass.imgext = "";
            //    loginModelClass.imgname = "";
            //    loginModelClass.imgpath = "";
            //}


            if (ModelState.IsValid)
            {

                string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();

                    //List<LoginModelClass> useracountlist = new List<LoginModelClass>();
                    using (SqlConnection connection = new SqlConnection(constring))
                    {

                    //============save image=========
                        if (file != null && file.ContentLength > 0)
                        {
                            file.SaveAs(ImagePathFolder);
                            //Session["UserImagePath"] = ImagePathFolder;
                        }
                        //============save image=========

                        int id = 0;
                        SqlCommand command = new SqlCommand("sp_UpdateUserAccount", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "sp_UpdateUserAccount";
                        command.Parameters.AddWithValue("@Userid", loginModelClass.Userid);
                        command.Parameters.AddWithValue("@UserName", loginModelClass.UserName);

                        command.Parameters.AddWithValue("@Email", loginModelClass.Email);
                        command.Parameters.AddWithValue("@UserPassword", loginModelClass.Password);
                        command.Parameters.AddWithValue("@UserRePassword", loginModelClass.UserRePassword);
                        command.Parameters.AddWithValue("@Firstname", loginModelClass.Firstname);
                        command.Parameters.AddWithValue("@Lastname", loginModelClass.Lastname);
                        command.Parameters.AddWithValue("@Gender", loginModelClass.genderid);

                        command.Parameters.AddWithValue("@Age", loginModelClass.Age);

                        command.Parameters.AddWithValue("@ContactNumber", loginModelClass.ContactNumber);

                        command.Parameters.AddWithValue("@Address", loginModelClass.Address);
                        if (loginModelClass.imgname == null)
                        {
                            loginModelClass.imgname = "";
                        }
                        command.Parameters.AddWithValue("@imgname", loginModelClass.imgname);
                        if (loginModelClass.imgext == null)
                        {
                            loginModelClass.imgext = "";
                        }
                        command.Parameters.AddWithValue("@imgext", loginModelClass.imgext);
                        if (loginModelClass.imgpath == null)
                        {
                            loginModelClass.imgpath = "";
                        }
                        command.Parameters.AddWithValue("@imgpath", loginModelClass.imgpath);
                        Session["UserImagePath"] = loginModelClass.imgpath;
                    //command.Parameters.AddWithValue("@StatusId", product.StatusId);
                    SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                        DataTable dtProducts = new DataTable();

                        connection.Open();

                        id = command.ExecuteNonQuery();
                        connection.Close();
                        TempData["SuccessMessage"] = "User Account Updated";

                        return RedirectToAction("Index", "Product");

                    }

            }

            return View();
        }


        public ActionResult EditUserAccountForgotPassword()
        {
            //ViewBag.Message = "Invalid Activation code.";
            if (RouteData.Values["id"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());

                string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();

                List<LoginModelClass> useracountlist = new List<LoginModelClass>();
                using (SqlConnection connection = new SqlConnection(constring))
                {

                    
                    SqlCommand command = new SqlCommand("sp_GetUserAccountIdforEdit", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@activationCode", activationCode);

                    //command.Parameters.AddWithValue("@userid", userid);
                    SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                    DataTable dtProducts = new DataTable();


                    connection.Open();
                    sqlDA.Fill(dtProducts);
                    connection.Close();

                    foreach (DataRow dr in dtProducts.Rows)
                    {
                        useracountlist.Add(new LoginModelClass
                        {
                            Userid = Convert.ToInt32(dr["ID"].ToString()),
                            UserName = dr["UserName"].ToString(),
                            Email = dr["Email"].ToString(),
                            Password = dr["UserPassword"].ToString(),
                            UserRePassword = dr["UserRePassword"].ToString(),
                            Firstname = dr["Firstname"].ToString(),
                            Lastname = dr["Lastname"].ToString(),
                            genderid = dr["Gender"].ToString(),
                            Age = Convert.ToInt32(dr["Age"]),
                            ContactNumber = dr["ContactNumber"].ToString(),
                            Address = dr["Address"].ToString(),
                            imgname = dr["imgname"].ToString(),
                            imgext = dr["imgext"].ToString(),
                            imgpath = dr["imgpath"].ToString()

                        });
                    }
                }
                //return useracountlist;
                return View(useracountlist.FirstOrDefault());
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult EditUserAccountForgotPassword(LoginModelClass loginModelClass)
        {
            //ViewBag.Message = "Invalid Activation code.";
            if (ModelState.IsValid)
            {
                if (RouteData.Values["id"] != null)
                {
                    //Guid activationCode = new Guid(RouteData.Values["id"].ToString());

                    string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();

                    //List<LoginModelClass> useracountlist = new List<LoginModelClass>();
                    using (SqlConnection connection = new SqlConnection(constring))
                    {

                        int id = 0;
                        SqlCommand command = new SqlCommand("sp_UpdateUserAccount", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "sp_UpdateUserAccount";
                        command.Parameters.AddWithValue("@Userid", loginModelClass.Userid);
                        command.Parameters.AddWithValue("@UserName", loginModelClass.UserName);

                        command.Parameters.AddWithValue("@Email", loginModelClass.Email);
                        command.Parameters.AddWithValue("@UserPassword", loginModelClass.Password);
                        command.Parameters.AddWithValue("@UserRePassword", loginModelClass.UserRePassword);
                        command.Parameters.AddWithValue("@Firstname", loginModelClass.Firstname);
                        command.Parameters.AddWithValue("@Lastname", loginModelClass.Lastname);
                        command.Parameters.AddWithValue("@Gender", loginModelClass.genderid);

                        command.Parameters.AddWithValue("@Age", loginModelClass.Age);

                        command.Parameters.AddWithValue("@ContactNumber", loginModelClass.ContactNumber);

                        command.Parameters.AddWithValue("@Address", loginModelClass.Address);
                        if (loginModelClass.imgname == null)
                        {
                            loginModelClass.imgname = "";
                        }
                        command.Parameters.AddWithValue("@imgname", loginModelClass.imgname);
                        if (loginModelClass.imgext == null)
                        {
                            loginModelClass.imgext = "";
                        }
                        command.Parameters.AddWithValue("@imgext", loginModelClass.imgext);
                        if (loginModelClass.imgpath == null)
                        {
                            loginModelClass.imgpath = "";
                        }
                        command.Parameters.AddWithValue("@imgpath", loginModelClass.imgpath);
                        //command.Parameters.AddWithValue("@StatusId", product.StatusId);
                        SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                        DataTable dtProducts = new DataTable();

                        connection.Open();

                        id = command.ExecuteNonQuery();
                        connection.Close();
                        TempData["UserAccountSuccessMessage"] = "User Account Updated";

                        return RedirectToAction("Index");

                    }
                    //return useracountlist;

                }
            }

                    return View();
        }


        [AllowAnonymous]
        public ActionResult forgotpassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult forgotpassword(LoginModelClass Logindata)
        {
            Session["userid"] = "";
            string mainconn = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[GetForgotPasswordCode]");
            sqlcon.Open();
            sqlcomm.Connection = sqlcon;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@Email", Logindata.Email);
            
            SqlDataReader sdr = sqlcomm.ExecuteReader();


            if (sdr.Read())
            {
                
                if (sdr["ForgotPasswordCode"].ToString() != null)
                {

                    using (MailMessage mm = new MailMessage("ghooqmail@gmail.com", Logindata.Email))
                    {
                        mm.Subject = "Account Forgot Password";
                        string body = "Hello! ";
                        body += "<br /><br />Please click the following link to Reset your passsword account";
                        body += "<br /><a href = '" + string.Format("{0}://{1}/UserLogin/EditUserAccountForgotPassword/{2}", Request.Url.Scheme, Request.Url.Authority, sdr["ForgotPasswordCode"].ToString()) + "'>Click Here</a>";
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
                    TempData["UserAccountSuccessMessage"] = "A link is sent to your Email. Click it to Change your Password";
                   
                    return RedirectToAction("Index");
                }
                
            }

            else
            {
                ViewData["EmailNotFound"] = "This Email is Not Registered.";
            }

            sqlcon.Close();
            return View();
        }

        public ActionResult Activation2()
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

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Index(LoginModelClass lc, HttpPostedFileBase Image)
        {

            Session["userid"] = "";
            string mainconn = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(mainconn);
            SqlCommand sqlcomm = new SqlCommand("[dbo].[UserLogin]");
            sqlcon.Open();
            sqlcomm.Connection = sqlcon;
            sqlcomm.CommandType = CommandType.StoredProcedure;
            sqlcomm.Parameters.AddWithValue("@Email", lc.Email);
            sqlcomm.Parameters.AddWithValue("@Password", lc.Password);
            SqlDataReader sdr = sqlcomm.ExecuteReader();
            

            if (sdr.Read())
            {
                if(!this.IsCaptchaValid(""))
                {
                    ViewBag.wrongcaptcha = "Invalid Captcha";
                    return View();
                }
                

                if (sdr["EmailConfirmed"].ToString() == "True")
                {
                    FormsAuthentication.SetAuthCookie(lc.Email, true);
                    Session["username"] = sdr["UserName"].ToString();
                    Session["userid"] = sdr["Userid"].ToString();
                    Session["UserImagePath"] = sdr["imgpath"].ToString();
                    Session["accountrole"] = "";
                    string role = sdr["AccountRole"].ToString();

                    if (role == "TourismAdminRole")
                    {

                        //return RedirectToAction("AdminIndex", "Product");
                        Session["accountrole"] = "TourismAdminRole";
                    }
                    else
                    {

                        //RedirectToAction("view","controller");
                        Session["accountrole"] = "user";
                    }

                    return RedirectToAction("Index", "Product");
                    TempData["SuccessMessage"] = null;

                }
                else
                {
                    ViewData["LoginError"] = "A Confirmation Link was Sent to your Email, Click it and then Continue to Login.";
                    string mainconn2 = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ConnectionString;
                    SqlConnection sqlcon2 = new SqlConnection(mainconn2);
                    SqlCommand sqlcomm2 = new SqlCommand("GetActivationCode");
                    sqlcon2.Open();
                    sqlcomm2.Connection = sqlcon2;
                    sqlcomm2.CommandType = CommandType.StoredProcedure;
                    sqlcomm2.Parameters.AddWithValue("@Userid", sdr["Userid"].ToString());
                    SqlDataReader sdr2 = sqlcomm2.ExecuteReader();

                    if (sdr2.Read())
                    {

                        using (MailMessage mm = new MailMessage("ghooqmail@gmail.com", lc.Email))
                        {
                            mm.Subject = "Account Activation";
                            string body = "Hello! ";
                            body += "<br /><br />Please click the following link to activate your account";
                            body += "<br /><a href = '" + string.Format("{0}://{1}/UserLogin/Activation2/{2}", Request.Url.Scheme, Request.Url.Authority, sdr2["ActivationCode"].ToString()) + "'>Click here to activate your account.</a>";
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

                    sqlcon2.Close();
                    return View();
                }

            }
                else
            {

                ViewData["LoginError"] = "Login Details Failed!";

            }

            sqlcon.Close();
            return View();

        }



        public ActionResult Welcome()
        {
            return View();
        }


        //=======for email confirmation=======
        public ActionResult Confirmation()
        {
            return View();
        }
        

        public ActionResult NewUserAccountRegistration()
        {

            LoginModelClass c = new LoginModelClass();
            newLogInModel = new LoginModelClass();
            c.genderlist = newLogInModel.genderlist = FillList();
            //ViewBag.LblCountry = "";
            return View(c);

        }

        public ActionResult NewUserAccountRegistration2()
        {

            LoginModelClass c = new LoginModelClass();
            newLogInModel = new LoginModelClass();
            c.genderlist = newLogInModel.genderlist = FillList();
            //ViewBag.LblCountry = "";
            return View(c);

        }

        [HttpPost]
        public  ActionResult NewUserAccountRegistration2(LoginModelClass loginModelClass, HttpPostedFileBase file)
        {
            //===image
            string ImagePathFolder = "";
            if (file != null && file.ContentLength > 0)
            {
                loginModelClass.imgname = Path.GetFileName(file.FileName);
                loginModelClass.imgext = Path.GetExtension(loginModelClass.imgname);

                //===image
                ImagePathFolder = Path.Combine(Server.MapPath("~/UserImages/"), Path.GetFileName(file.FileName));
                loginModelClass.imgpath = "~/UserImages/" + Path.GetFileName(file.FileName);
                //====

            }
            else
            {
                loginModelClass.imgext = "";
                loginModelClass.imgname = "";
                loginModelClass.imgpath = "";
            }

            loginModelClass.genderlist = FillList();

                //bool IsInserted = false;

                if (ModelState.IsValid)
                {

                    //string IDinserted = "";
                    //int id = 0;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        
                        SqlCommand command = new SqlCommand("sp_InsertUserAccount2", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserName", loginModelClass.UserName);
                        command.Parameters.AddWithValue("@Email", loginModelClass.Email);
                        command.Parameters.AddWithValue("@UserPassword", loginModelClass.UserRePassword);
                        command.Parameters.AddWithValue("@UserRePassword", loginModelClass.UserRePassword);
                        command.Parameters.AddWithValue("@Firstname", loginModelClass.Firstname);
                        command.Parameters.AddWithValue("@Lastname", loginModelClass.Lastname);
                        command.Parameters.AddWithValue("@Gender", loginModelClass.genderid);
                        command.Parameters.AddWithValue("@Age", loginModelClass.Age);
                        command.Parameters.AddWithValue("@ContactNumber", loginModelClass.ContactNumber);
                        command.Parameters.AddWithValue("@Address", loginModelClass.Address);
                        command.Parameters.AddWithValue("@imgext", loginModelClass.imgext);
                        command.Parameters.AddWithValue("@imgname", loginModelClass.imgname);
                        command.Parameters.AddWithValue("@imgpath", loginModelClass.imgpath);
                        command.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                        SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                        DataTable dtProducts = new DataTable();
                        //IDinserted = (int)command.ExecuteScalar();

                        connection.Open();
                        command.ExecuteNonQuery();

                        string NewInsertId = command.Parameters["@id"].Value.ToString();
                        int NewInsertIdint = Convert.ToInt32(NewInsertId);
                        connection.Close();

                        if (NewInsertIdint == -1)
                        {
                            TempData["ErrorMessage"] = "Unable to Save the Business Details.";
                            ViewData["UserLogInNotAvailable"] = "Unable to Save! User Name is already taken";

                            //return View();
                        }

                        else if (NewInsertIdint == -2)
                        {
                            TempData["ErrorMessage"] = "Unable to Save the Business Details.";
                            ViewData["UserLogInNotAvailable"] = "Unable to Save! Email is already registered";

                        }

                        else
                        {
                            //============save image=========
                            if (file != null && file.ContentLength > 0)
                            { 
                                loginModelClass.file.SaveAs(ImagePathFolder);
                            }
                            //============save image=========

                            Guid activationCode = Guid.NewGuid();
                            using (SqlConnection connection2 = new SqlConnection(connectionString))
                            {

                                SqlCommand command2 = new SqlCommand("sp_InsertUserActivation", connection2);
                                command2.CommandType = CommandType.StoredProcedure;
                                command2.Parameters.AddWithValue("@UserId", NewInsertIdint);
                                command2.Parameters.AddWithValue("@ActivationCode", activationCode);
                                connection2.Open();
                                command2.ExecuteNonQuery();
                                connection2.Close();

                            }

                            using (MailMessage mm = new MailMessage("ghooqmail@gmail.com", loginModelClass.Email))
                            {
                                mm.Subject = "Account Activation";
                                string body = "Hello " + loginModelClass.UserName + ",";
                                body += "<br /><br />Please click the following link to activate your account";
                                body += "<br /><a href = '" + string.Format("{0}://{1}/UserLogin/Activation2/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Click here to activate your account.</a>";
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

                            Guid activationCode3 = Guid.NewGuid();
                            using (SqlConnection connection3 = new SqlConnection(connectionString))
                            {

                                SqlCommand command3 = new SqlCommand("sp_InsertUserForgotPassword", connection3);
                                command3.CommandType = CommandType.StoredProcedure;
                                command3.Parameters.AddWithValue("@Userid", NewInsertIdint);
                                command3.Parameters.AddWithValue("@Code", activationCode3);
                                connection3.Open();
                                command3.ExecuteNonQuery();
                                connection3.Close();

                            }

                        TempData["UserAccountSuccessMessage"] = "User Account Details Saved Successfully! An Email Verification is sent to you.";
                            return RedirectToAction("Index");
                        }


                    }


                }
                else
                {


                }
                return View(loginModelClass);
            
            
        }

        

        public List<SelectListItem> FillList()
            {
            var list = new List<SelectListItem>();

            string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();
            SqlConnection connection = new SqlConnection(constring);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "sp_PopulateGender";
            connection.Open();
            SqlDataReader sdr = command.ExecuteReader();
            while (sdr.Read())
            {
                list.Add(new SelectListItem { Text = sdr["Gender"].ToString(), Value = sdr["GenderId"].ToString() });
            }

            return list;
        }





    }
}