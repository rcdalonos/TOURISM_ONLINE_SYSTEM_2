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
    public class ProductController : Controller
    {
        Product_DAL _productDAL = new Product_DAL();
        public static Product newProductModel;

        public ActionResult signout()
        {
            Session["username"] = null;
            Session["userid"] = null;
            Session["userimagepath"] = null;
            Session["accountrole"] = null;

            return RedirectToAction("Index", "UserLogin");
        }

        public ActionResult AdminIndex(string SearchString, int? i)
        {

            if (SearchString == null)
            {
                SearchString = "";
            }
            List<Product> productList = _productDAL.search_admin_product_name(SearchString, Session["userid"].ToString()).ToList();

            //return View(productList.ToPagedList(i ?? 1, 10));
            return View(productList.ToPagedList(i ?? 1, 10));
            //productList.ToPagedList()
        }

        //[HttpPost]
        public ActionResult Index(string SearchString, int? i)
        {

            if (SearchString == null)
            {
                SearchString = "";
            }

            List<Product> productList;
            if (Session["accountrole"].ToString() == "TourismAdminRole")
            {
                productList = _productDAL.search_admin_product_name(SearchString, Session["userid"].ToString()).ToList();

            }
            else
            { 
                productList = _productDAL.search_product_name(SearchString, Session["userid"].ToString()).ToList();
            }
            //return View(productList.ToPagedList(i ?? 1, 10));
            return View(productList.ToPagedList(i ?? 1, 10));
            //productList.ToPagedList()
        }




        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {

            Product c = new Product();
            c.ImagePath = "~/ProductImages/businessdefaultpic.jpg";
            newProductModel = new Product();
            c.classificationlist = newProductModel.classificationlist = FillList();
            c.ImagePath = "~/ProductImages/businessdefaultpic.jpg";
            //ViewBag.LblCountry = "";
            return View(c);

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

        public List<SelectListItem> BusinessStatusFillList()
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


        // POST: Product/Create
        //public ActionResult Create(Product productclass, HttpPostedFileBase file)
        [HttpPost]
        public ActionResult Create(Product productclass)
        {
            //==========image
            string ImagePathFolder = "";
            if (productclass.file != null && productclass.file.ContentLength > 0)
            {

                ImagePathFolder = Path.Combine(Server.MapPath("~/ProductImages/"), Path.GetFileName(productclass.file.FileName));
                productclass.ImagePath = "~/ProductImages/" + Path.GetFileName(productclass.file.FileName);
                productclass.ImageFileName = productclass.file.FileName;
                //emp_leaves.uploadFile.InputStream.Read(emp_leaves.fileContent, 0, emp_leaves.uploadFile.ContentLength);
                Session["file"] = productclass.file;
            }
            //==========image

            productclass.UserId = Session["userid"].ToString();

            productclass.classificationlist = FillList();

            bool IsInserted = false;
            try
            {
                if (ModelState.IsValid)
                {
                    IsInserted = _productDAL.InsertProduct(productclass);
                    if (IsInserted)
                    {
                        TempData["SuccessMessage"] = "Business Details saved Successfully!";
                        //====image
                        productclass.file.SaveAs(ImagePathFolder);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        productclass.file = (HttpPostedFileBase)Session["file"];
                        TempData["ErrorMessage"] = "Unable to save the Business Details. Business Name is already taken!";
                    }

                }
                else
                {
                    if (productclass.file != null && productclass.file.ContentLength > 0)
                    {


                    }
                }

                //productclass.ImagePath = "";
                return View(productclass);
            }
            catch (Exception ex)
            {
                //productclass.ImagePath = "";
                TempData["ErrorMessage"] = ex.Message;
                return View(productclass);
            }
        }


        public ActionResult Edit(int id)
        {

            var products = _productDAL.GetProductsByID(id).FirstOrDefault();
            products.ProductID = id;
            products.classificationlist = FillList();
            products.BusinessSatusList = BusinessStatusFillList();
            if (products == null)
            {
                TempData["InfoMessage"] = "Product not available with ID " + id.ToString();
                return RedirectToAction("Index");
            }
            return View(products);
        }

        public ActionResult AdminEdit(int id)
        {

            var products = _productDAL.GetProductsByID(id).FirstOrDefault();
            products.ProductID = id;
            products.classificationlist = FillList();
            products.BusinessSatusList = BusinessStatusFillList();
            if(products.ImagePath==""|| products.ImagePath == null)
            {
                products.ImagePath = "~/ProductImages/businessdefaultpic.jpg";
            }
            if (products == null)
            {
                TempData["InfoMessage"] = "Product not available with ID " + id.ToString();
                return RedirectToAction("Index");
            }
            return View(products);
        }

        // POST: Product/Edit/5
        [HttpPost, ActionName("Edit")]
        public ActionResult UpdateProduct(ProductEdit productclassEdit, HttpPostedFileBase file)
        {
            //=========image
            string ImagePathFolder = productclassEdit.ImagePath;
            if (file != null && file.ContentLength > 0)
            {

                ImagePathFolder = Path.Combine(Server.MapPath("~/ProductImages/"), Path.GetFileName(file.FileName));
                productclassEdit.ImagePath = "~/ProductImages/" + Path.GetFileName(file.FileName);
                productclassEdit.ImageFileName = file.FileName;
            }
            //==========

            productclassEdit.UserId = @Session["userid"].ToString();

            productclassEdit.classificationlist = FillList();
            //productclass.ClassificationId = 

            bool IsUpdated = false;


            //try {
            if (ModelState.IsValid)
            {
                IsUpdated = _productDAL.UpdateProduct(productclassEdit);
                if (IsUpdated)
                {
                    TempData["SuccessMessage"] = "Product details updated successfully!";
                    //=========image
                    if (file != null && file.ContentLength > 0)
                    {
                        file.SaveAs(ImagePathFolder);
                    }
                    //=========
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to update the product details!";

                }
                return View(productclassEdit);
            }
            //}
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = ex.Message;
            //    return View(productclass);
            //}
            return View(productclassEdit);
        }


        [HttpPost, ActionName("AdminEdit")]
        public ActionResult AdminEdit(ProductEdit productclassEdit, HttpPostedFileBase file)
        {
            //=========image
            //string ImagePathFolder = productclassEdit.ImagePath;
            //if (file != null && file.ContentLength > 0)
            //{

            //    ImagePathFolder = Path.Combine(Server.MapPath("~/ProductImages/"), Path.GetFileName(file.FileName));
            //    productclassEdit.ImagePath = "~/ProductImages/" + Path.GetFileName(file.FileName);
            //    productclassEdit.ImageFileName = file.FileName;
            //}
            //==========

            productclassEdit.UserId = @Session["userid"].ToString();

            productclassEdit.classificationlist = FillList();
            productclassEdit.BusinessSatusList = BusinessStatusFillList();

            bool IsUpdated = false;


            //try {
            if (ModelState.IsValid)
            {
                IsUpdated = _productDAL.AdminUpdateProduct(productclassEdit);
                if (IsUpdated)
                {
                    TempData["SuccessMessage"] = "Business details updated successfully!";
                    //=========image
                    //if (file != null && file.ContentLength > 0)
                    //{
                    //    file.SaveAs(ImagePathFolder);
                    //}
                    //=========
                    return RedirectToAction("AdminIndex");
                }
                else
                {
                    TempData["ErrorMessage"] = "Unable to update the Business details!";

                }
                return View(productclassEdit);
            }
            //}
            //catch (Exception ex)
            //{
            //    TempData["ErrorMessage"] = ex.Message;
            //    return View(productclass);
            //}
            return View(productclassEdit);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            var products = _productDAL.GetProductsByID(id).FirstOrDefault();
            products.ProductID = id;
            products.classificationlist = FillList();
            if (products == null)
            {
                TempData["InfoMessage"] = "Product not available with ID " + id.ToString();
                return RedirectToAction("Index");
            }
            return View(products);
        }

        // POST: Product/Delete/5
        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteProduct(int id)
        {
            try
            {
                string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();

                //List<LoginModelClass> useracountlist = new List<LoginModelClass>();
                using (SqlConnection connection = new SqlConnection(constring))
                {


                    SqlCommand command = new SqlCommand("sp_DeleteProducts", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ProductID", id);

                    //command.Parameters.AddWithValue("@userid", userid);
                    SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                    DataTable dtProducts = new DataTable();
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return RedirectToAction("Index");
                }
                    
            }
            catch
            {
                return View();
            }
        }
    }
}
