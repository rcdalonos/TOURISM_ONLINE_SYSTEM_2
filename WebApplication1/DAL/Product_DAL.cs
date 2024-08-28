using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using WebApplication1.Models;
using System.Data;

namespace WebApplication1.DAL
{
    public class Product_DAL
    {
        public string constring = ConfigurationManager.ConnectionStrings["adoConnectionString"].ToString();

        public List<Product> search_product_name(string searchtitle, string userid)
        {

            List<Product> productlist = new List<Product>();
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.CommandText = "sp_Search";
                command.Parameters.AddWithValue("@searchtitle", searchtitle);
                command.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtProducts = new DataTable();


                connection.Open();
                sqlDA.Fill(dtProducts);
                connection.Close();

                foreach (DataRow dr in dtProducts.Rows)
                {
                    productlist.Add(new Product
                    {
                        ProductID = Convert.ToInt32(dr["ProductId"]),
                        BusinessName = dr["BusinessName"].ToString(),
                        BusinessOwner = dr["BusinessOwner"].ToString(),
                        BusinessAddress = dr["BusinessAddress"].ToString(),
                        ContactNumber = dr["ContactNumber"].ToString(),
                        //=========image
                        ImagePath = dr["ImageLink"].ToString(),
                        ClassificationId = dr["ClassificationId"].ToString(),
                        //BusinessId = dr["ImageLink"].ToString()
                        BusinessStatus = dr["StatusId"].ToString(),
                        BusinessStatustext = dr["businessstatus"].ToString(),

                    });
                }
            }
            return productlist;
        }
        public List<Product> search_admin_product_name(string searchtitle, string userid)
        {

            List<Product> productlist = new List<Product>();
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.CommandText = "sp_AdminSearch";
                command.Parameters.AddWithValue("@searchtitle", searchtitle);
                command.Parameters.AddWithValue("@userid", userid);
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtProducts = new DataTable();


                connection.Open();
                sqlDA.Fill(dtProducts);
                connection.Close();

                foreach (DataRow dr in dtProducts.Rows)
                {
                    productlist.Add(new Product
                    {
                        ProductID = Convert.ToInt32(dr["ProductId"]),
                        BusinessName = dr["BusinessName"].ToString(),
                        BusinessOwner = dr["BusinessOwner"].ToString(),
                        BusinessAddress = dr["BusinessAddress"].ToString(),
                        ContactNumber = dr["ContactNumber"].ToString(),
                        //=========image
                        ImagePath = dr["ImageLink"].ToString(),
                        ClassificationId = dr["ClassificationId"].ToString(),
                        //BusinessId = dr["ImageLink"].ToString()
                        BusinessStatus = dr["StatusId"].ToString(),
                        BusinessStatustext = dr["businessstatus"].ToString(),

                    });
                }
            }
            return productlist;
        }
        //get all products
        public List<Product> GetAllProducts()
        {
            List<Product> productlist = new List<Product>();
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "sp_GetAllProducts";
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtProducts = new DataTable();

                connection.Open();
                sqlDA.Fill(dtProducts);
                connection.Close();

                foreach (DataRow dr in dtProducts.Rows)
                {
                    productlist.Add(new Product
                    {
                        ProductID = Convert.ToInt32(dr["ProductId"]),
                        BusinessName = dr["BusinessName"].ToString(),
                        BusinessAddress = dr["BusinessAddress"].ToString(),
                        ContactNumber = dr["ContactNumber"].ToString(),
                        //Remarks = dr["Remarks"].ToString()
                        BusinessStatus = dr["StatusId"].ToString(),
                    });
                }
            }
            return productlist;
        }



        public bool InsertProduct(Product product)
        {
            int id = 0;
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand command = new SqlCommand("sp_InsertProducts", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_InsertProducts";
                command.Parameters.AddWithValue("@BusinessName", product.BusinessName);
                command.Parameters.AddWithValue("@BusinessOwner", product.BusinessOwner);
                command.Parameters.AddWithValue("@BusinessAddress", product.BusinessAddress);
                command.Parameters.AddWithValue("@ContactNumber", product.ContactNumber);
                command.Parameters.AddWithValue("@Email", product.Email);

                if (product.Website == null) product.Website = "";
                command.Parameters.AddWithValue("@Website", product.Website);

                //product.ImagePath = "";
                command.Parameters.AddWithValue("@ImageLink", product.ImagePath);
                command.Parameters.AddWithValue("@UserId", product.UserId);
                command.Parameters.AddWithValue("@ClassificationId", product.ClassificationId);
                command.Parameters.AddWithValue("@StatusId", "2");
                //product.ImageFileName = "";
                command.Parameters.AddWithValue("@ImageFileName", product.ImageFileName);


                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtProducts = new DataTable();

                connection.Open();

                id = command.ExecuteNonQuery();
                connection.Close();
            }
            if (id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<ProductEdit> GetProductsByID(int ProductID)
        {
            List<ProductEdit> productlist = new List<ProductEdit>();
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "sp_GetProductID";
                command.Parameters.AddWithValue("@ProductID", ProductID);
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtProducts = new DataTable();

                connection.Open();
                sqlDA.Fill(dtProducts);
                connection.Close();

                foreach (DataRow dr in dtProducts.Rows)
                {
                    productlist.Add(new ProductEdit
                    {
                        BusinessStatus = dr["StatusId"].ToString(),
                        ProductID = Convert.ToInt32(dr["ProductID"]),
                        BusinessName = dr["BusinessName"].ToString(),
                        BusinessOwner = dr["BusinessOwner"].ToString(),
                        BusinessAddress = dr["BusinessAddress"].ToString(),
                        ContactNumber = dr["ContactNumber"].ToString(),
                        Email = dr["Email"].ToString(),
                        Website = dr["Website"].ToString(),

                        ClassificationId = dr["ClassificationId"].ToString(),


                        ImagePath = dr["ImageLink"].ToString(),
                        ImageFileName = dr["ImageFileName"].ToString()
                        //ImagePath = "",
                        //ImageFileName = ""
                    });
                }
            }
            return productlist;
        }

        public bool UpdateProduct(ProductEdit product)
        {
            int id = 0;
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand command = new SqlCommand("sp_UpdateProducts", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_UpdateProducts";

                command.Parameters.AddWithValue("@ProductID", product.ProductID);
                command.Parameters.AddWithValue("@BusinessName", product.BusinessName);
                command.Parameters.AddWithValue("@BusinessOwner", product.BusinessOwner);
                command.Parameters.AddWithValue("@BusinessAddress", product.BusinessAddress);
                command.Parameters.AddWithValue("@ContactNumber", product.ContactNumber);
                command.Parameters.AddWithValue("@Email", product.Email);

                if (product.Website == null) product.Website = "";
                command.Parameters.AddWithValue("@Website", product.Website);

                command.Parameters.AddWithValue("@UserId", product.UserId);

                command.Parameters.AddWithValue("@ClassificationId", product.ClassificationId);

                if (product.ImageFileName == null) product.ImageFileName = "";
                command.Parameters.AddWithValue("@ImageFileName", product.ImageFileName);

                command.Parameters.AddWithValue("@StatusId", product.BusinessStatus);

                if(product.ImagePath == null) product.ImagePath = "";
                command.Parameters.AddWithValue("@ImageLink", product.ImagePath);
                //command.Parameters.AddWithValue("@StatusId", product.StatusId);
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtProducts = new DataTable();

                connection.Open();

                id = command.ExecuteNonQuery();
                connection.Close();
            }
            if (id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool AdminUpdateProduct(ProductEdit product)
        {
            int id = 0;
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand command = new SqlCommand("sp_UpdateProducts", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_UpdateProducts";

                command.Parameters.AddWithValue("@ProductID", product.ProductID);
                command.Parameters.AddWithValue("@BusinessName", product.BusinessName);
                command.Parameters.AddWithValue("@BusinessOwner", product.BusinessOwner);
                command.Parameters.AddWithValue("@BusinessAddress", product.BusinessAddress);
                command.Parameters.AddWithValue("@ContactNumber", product.ContactNumber);
                command.Parameters.AddWithValue("@Email", product.Email);
                command.Parameters.AddWithValue("@Website", product.Website);

                command.Parameters.AddWithValue("@UserId", product.UserId);

                command.Parameters.AddWithValue("@ClassificationId", product.ClassificationId);
                command.Parameters.AddWithValue("@ImageFileName", "");

                command.Parameters.AddWithValue("@StatusId", product.BusinessStatus);

                product.ImagePath = "";
                command.Parameters.AddWithValue("@ImageLink", product.ImagePath);
                //command.Parameters.AddWithValue("@StatusId", product.StatusId);
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtProducts = new DataTable();

                connection.Open();

                id = command.ExecuteNonQuery();
                connection.Close();
            }
            if (id > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}