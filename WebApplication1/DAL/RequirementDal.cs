using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using WebApplication1.Models;
using System.Data;
using System.Globalization;

namespace WebApplication1.DAL
{
    public class RequirementDal
    {
        public string constring = ConfigurationManager.ConnectionStrings["adoConnectionString"].ToString();

        public bool InsertorUpdateRequirement(RequirementsModel requirementsModel)
        {
            int id = 0;
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand command = new SqlCommand("sp_InsertorUpdateRequirement", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "sp_InsertorUpdateRequirement";
                if (requirementsModel.BusinessRequirementId == null) requirementsModel.BusinessRequirementId = "";
                
                command.Parameters.AddWithValue("@businessrequirementid", requirementsModel.BusinessRequirementId);
                command.Parameters.AddWithValue("@expirationdate", Convert.ToDateTime(requirementsModel.ExpirationDate));
                //command.Parameters.AddWithValue("@expirationdate", Convert.ToDateTime(DateTime.ParseExact(requirementsModel.ExpirationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture))); 
                command.Parameters.AddWithValue("@expirationstatus", requirementsModel.ExpirationStatus);
                command.Parameters.AddWithValue("@applicablestatus", requirementsModel.ApplicableStatus);
                command.Parameters.AddWithValue("@serialno", requirementsModel.SerialNo);

                if (requirementsModel.RequirementId == null) requirementsModel.RequirementId = "";
                command.Parameters.AddWithValue("@RequirementId", requirementsModel.RequirementId);
                command.Parameters.AddWithValue("@BusinessId", requirementsModel.BusinessId);

                //if (requirementsModel.imgpath == null) requirementsModel.imgpath = "";
                command.Parameters.AddWithValue("@imgpath", requirementsModel.imgpath);

                if (requirementsModel.ApprovalStatus == null) requirementsModel.ApprovalStatus = "2";
                command.Parameters.AddWithValue("@ApprovalStatus", requirementsModel.ApprovalStatus);

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


        public List<RequirementsModel> ListofRequirements(string ClassificationId, string BusinessId)
        {
            List<RequirementsModel> Requirementslist = new List<RequirementsModel>();
            using (SqlConnection connection = new SqlConnection(constring))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "PopulateAllRequirements";
                command.Parameters.AddWithValue("@ClassificationId", ClassificationId);
                command.Parameters.AddWithValue("@BusinessId", BusinessId);
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtRequirments = new DataTable();


                connection.Open();
                sqlDA.Fill(dtRequirments);
                connection.Close();

                foreach (DataRow dr in dtRequirments.Rows)
                {

                    if (dr["BusinessRequirementId"] != System.DBNull.Value)
                    {
                        Requirementslist.Add(new RequirementsModel
                        {

                            Requirement = dr["Requirement"].ToString(),
                            ExpirationDate = dr["ExpirationDate"].ToString(),
                            ExpirationStatus = (bool)dr["ExpirationStatus"],
                            ApplicableStatus = (bool)dr["ApplicableStatus"],
                            SerialNo = dr["SerialNo"].ToString(),
                            BusinessRequirementId = dr["BusinessRequirementId"].ToString(),
                            RequirementId = dr["requirementidlink"].ToString(),
                            BusinessId = dr["BusinessId2"].ToString(),
                            ApprovalStatusText = dr["EvaluationStatus"].ToString(),
                            imgpath = dr["imgpath"].ToString(),

                        });
                    }
                    if (dr["BusinessRequirementId"] == System.DBNull.Value)
                    {
                        Requirementslist.Add(new RequirementsModel
                        {

                            Requirement = dr["Requirement"].ToString(),
                            
                            SerialNo = dr["SerialNo"].ToString(),
                            BusinessRequirementId = dr["BusinessRequirementId"].ToString(),
                            RequirementId = dr["requirementidlink"].ToString(),
                            BusinessId = dr["BusinessId2"].ToString(),
                            imgpath = "~/RequirementImages/requirementdefaultpic.jpg",

                    });
                    }

                }
            }
            return Requirementslist;
        }

        public List<RequirementsModel> GetRequirementbyID(string businessrequirementid, string requirementtext, string RequirementId, string BusinessId)
        {

            List<RequirementsModel> requirmentlist = new List<RequirementsModel>();

            if (businessrequirementid == "" || businessrequirementid == null)
            {
                //DateTime date = DateTime.ParseExact(DateTime.Now.ToString(), "dd/MM/yyyy", null);
                requirmentlist.Add(new RequirementsModel
                {
                    Requirement = requirementtext,
                    //ExpirationDate = Convert.ToDateTime(@DateTime.Now.ToString()),
                    ExpirationStatus = false,
                    ApplicableStatus = false,
                    SerialNo = "",
                    BusinessRequirementId = "",
                    RequirementId = RequirementId,
                    BusinessId = BusinessId,
                    ApprovalStatus = "",
                    ExpirationDate = DateTime.Now.ToShortDateString(),
                    imgpath = "~/RequirementImages/requirementdefaultpic.jpg",
                }); 
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(constring))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "sp_GetbyRequirementID";
                    int reqid = int.Parse(businessrequirementid);
                    command.Parameters.AddWithValue("@requirementid", reqid);
                    
                    command.Parameters.AddWithValue("@BusinessId", BusinessId);

                    //command.Parameters.AddWithValue("@requirementtext", requirementtext);
                    SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                    DataTable dtProducts = new DataTable();

                    connection.Open();
                    SqlDataReader dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        //sqlDA.Fill(dtProducts);
                        //connection.Close();
                        ////DateTime datetime;
                        //foreach (DataRow dr in dtProducts.Rows)
                        //{
                        requirmentlist.Add(new RequirementsModel
                        {

                            Requirement = dr["Requirement"].ToString(),
                            ExpirationDate = dr["ExpirationDate"].ToString(),
                            ExpirationStatus = (bool)dr["ExpirationStatus"],
                            ApplicableStatus = (bool)dr["ApplicableStatus"],
                            //ApplicableStatus = 0,
                            SerialNo = dr["SerialNo"].ToString(),
                            BusinessRequirementId = dr["BusinessRequirementId"].ToString(),
                            RequirementId = dr["RequirementId"].ToString(),
                            BusinessId = dr["BusinessId"].ToString(),
                            ApprovalStatus = dr["ApprovalStatus"].ToString(),
                            imgpath = dr["imgpath"].ToString(),
                        });
                    }
                    connection.Close();
                }
            }
            return requirmentlist;
        }

        //public List<Product> PopulateApprovalStatusList()
        //{
        //    List<Product> productlist = new List<Product>();
        //    using (SqlConnection connection = new SqlConnection(constring))
        //    {
        //        SqlCommand command = connection.CreateCommand();
        //        command.CommandType = System.Data.CommandType.StoredProcedure;
        //        command.CommandText = "PopulateApprovalStatusList";
        //        SqlDataAdapter sqlDA = new SqlDataAdapter(command);
        //        DataTable dtProducts = new DataTable();

        //        connection.Open();
        //        sqlDA.Fill(dtProducts);
        //        connection.Close();

        //        foreach (DataRow dr in dtProducts.Rows)
        //        {
        //            productlist.Add(new Product
        //            {
        //                ProductID = Convert.ToInt32(dr["ProductId"]),
        //                BusinessName = dr["BusinessName"].ToString(),
        //                BusinessAddress = dr["BusinessAddress"].ToString(),
        //                ContactNumber = dr["ContactNumber"].ToString(),

        //                BusinessStatus = dr["StatusId"].ToString(),
        //            });
        //        }
        //    }
        //    return productlist;
        //}



    }
}