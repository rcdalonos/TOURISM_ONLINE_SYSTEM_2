using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.DAL; //DAL MEANING DATA ACCESS LAYER
using WebApplication1.Models;

using PagedList.Mvc;
using PagedList;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using System.Web.Security;
using System.IO;

namespace WebApplication1.Controllers
{
    public class RequirementController : Controller
    {
        RequirementDal _requirementDalNew = new RequirementDal();
        //Product_DAL _productDAL = new Product_DAL();
        public static RequirementsModel newRequirementsModel;
        

        public ActionResult signout()
        {
            Session["username"] = null;
            Session["userid"] = null;
            Session["userimagepath"] = null;
            Session["accountrole"] = null;

            return RedirectToAction("Index", "UserLogin");
        }

        
        public ActionResult ListOfRequirements(string ClassificationId, string businessid, int? i)
        {
            ViewBag.ClassificationId = ClassificationId;
            ViewBag.businessid = businessid;

            Session["ClassificationId"] = ClassificationId;
            Session["businessid"] = businessid;
            List<RequirementsModel> RequirmentList = _requirementDalNew.ListofRequirements(ClassificationId, businessid);

            return View(RequirmentList.ToPagedList(i ?? 1, 10));

        }

        public ActionResult AdminListOfRequirements(string ClassificationId, string businessid, int? i)
        {
            ViewBag.ClassificationId = ClassificationId;
            ViewBag.businessid = businessid;

            Session["ClassificationId"] = ClassificationId;
            Session["businessid"] = businessid;
            List<RequirementsModel> RequirmentList = _requirementDalNew.ListofRequirements(ClassificationId, businessid);

            return View(RequirmentList.ToPagedList(i ?? 1, 10));

        }

        //[HttpGet]
        public ActionResult EditRequirements(string id, string requirementtext, string RequirementId, string BusinessId)
        {

            var requirements = _requirementDalNew.GetRequirementbyID(id, requirementtext, RequirementId, BusinessId).FirstOrDefault();
            //requirements.Requirement = requirementtext;
            requirements.ApprovalStatusList = FillListApprovalStatus();
            return View(requirements);
        }

        public ActionResult AdminEditRequirements(string id, string requirementtext, string RequirementId, string BusinessId)
        {

            var requirements = _requirementDalNew.GetRequirementbyID(id, requirementtext, RequirementId, BusinessId).FirstOrDefault();
            requirements.Requirement = requirementtext;
            requirements.ApprovalStatusList = FillListApprovalStatus();
            return View(requirements);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult EditRequirements(RequirementsModel requirementsModelnew, HttpPostedFileBase file)
        {

            bool IsInserted = false;

            string ImagePathFolder = requirementsModelnew.imgpath;
            if (file != null)
            {
                requirementsModelnew.imgname = Path.GetFileName(requirementsModelnew.file.FileName);
                requirementsModelnew.imgext = Path.GetExtension(requirementsModelnew.imgname);
                ImagePathFolder = Path.Combine(Server.MapPath("~/RequirementImages/"), Path.GetFileName(requirementsModelnew.file.FileName));
                requirementsModelnew.imgpath = "~/RequirementImages/" + Path.GetFileName(requirementsModelnew.file.FileName);
                //====

            }
            if (ModelState.IsValid)
            {
                IsInserted = _requirementDalNew.InsertorUpdateRequirement(requirementsModelnew);
                if (IsInserted)
                {
                    
                    if (file != null && file.ContentLength > 0)
                    {
                        file.SaveAs(ImagePathFolder);
                        
                    }

                    return RedirectToAction("ListOfRequirements", "Requirement", new { ClassificationId = Session["ClassificationId"], BusinessId = Session["businessid"] });
                }
                else
                {
                    
                }

            }
            else
            {

            }

          
            return View(requirementsModelnew);
            
        }

        [HttpPost, ActionName("AdminEditRequirements")]
        public ActionResult AdminEditRequirementsPost(RequirementsModel requirementsModelnew)
        {
            requirementsModelnew.ApprovalStatusList = FillListApprovalStatus();
            bool IsInserted = false;
            //try
            //{
            if (ModelState.IsValid)
            {
                IsInserted = _requirementDalNew.InsertorUpdateRequirement(requirementsModelnew);
                if (IsInserted)
                {
                    //TempData["SuccessMessage"] = "Requirement's Details saved Successfully!";
                    //file.SaveAs(ImagePathFolder);
                    //Session["ClassificationId"] = ClassificationId;
                    //Session["businessid"] = businessid;
                    return RedirectToAction("AdminListOfRequirements", "Requirement", new { ClassificationId = Session["ClassificationId"], BusinessId = Session["businessid"] });
                }
                else
                {
                    //TempData["UserLogInNotAvailable"] = "User or Email is already taken.";
                    //TempData["SuccessMessage"] = "Product Details saved Successfully!";
                }

            }
            else
            {

            }

            //productclass.ImagePath = "";
            return View(requirementsModelnew);
            //}
            //catch (Exception ex)
            //{
            //    //productclass.ImagePath = "";
            //    TempData["ErrorMessage"] = ex.Message;
            //    return View(requirementsModelnew);
            //}
        }


        public ActionResult Details(int id)
        {
            return View();
        }


        public List<SelectListItem> FillList()
        {
            var list = new List<SelectListItem>();

            string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();
            SqlConnection connection = new SqlConnection(constring);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PopulateBusinessClassification";
            connection.Open();
            SqlDataReader sdr = command.ExecuteReader();
            while (sdr.Read())
            {
                list.Add(new SelectListItem { Text = sdr["Classification"].ToString(), Value = sdr["ClassificationId"].ToString() });
            }

            return list;
        }

        public List<SelectListItem> FillListApprovalStatus()
        {
            var list = new List<SelectListItem>();

            string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();
            SqlConnection connection = new SqlConnection(constring);
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PopulateApprovalStatusList";
            connection.Open();
            SqlDataReader sdr = command.ExecuteReader();
            while (sdr.Read())
            {
                list.Add(new SelectListItem { Text = sdr["ApprovStatus"].ToString(), Value = sdr["ApprovalStatusListId"].ToString() });
            }

            return list;
        }


    }
}