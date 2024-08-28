using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using System.Web.Security;
using System.IO;
using ReportViewerForMvc;
using WebApplication1;

namespace WebApplication1.Controllers
{
    public class ReportConController : Controller
    {
        // GET: ReportCon
        public string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();

        public ActionResult signout()
        {
            Session["username"] = null;
            Session["userid"] = null;
            Session["userimagepath"] = null;
            Session["accountrole"] = null;

            return RedirectToAction("Index", "UserLogin");
        }

        MyDataSet dsReport = new MyDataSet();
        public ActionResult ReportListofComliantBusiness()
        {
            using (SqlConnection connection = new SqlConnection(constring))
            {

                ReportViewer reportviewer = new ReportViewer();
                reportviewer.ProcessingMode = ProcessingMode.Local;
                reportviewer.SizeToReportContent = true;
                reportviewer.Width = Unit.Percentage(200);
                reportviewer.Height = Unit.Percentage(200);

                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "sp_ReportListOfBusiness";
                //command.Parameters.AddWithValue("@ClassificationId", ClassificationId);
                //command.Parameters.AddWithValue("@BusinessId", BusinessId);
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtListofBusiness = new DataTable();

                connection.Open();

                //sqlDA.Fill(dsReport, dsReport.sp_ReportListOfBusiness.TableName);
                sqlDA.Fill(dsReport, dsReport.sp_ReportListOfBusiness.TableName);
                string path = Path.Combine(Server.MapPath("~/Reports"), "Report1.rdlc");
                //reportviewer.LocalReport.ReportPath = Server.MapPath(@"/Reports/Report1.rdlc");
                reportviewer.LocalReport.ReportPath = path;
                reportviewer.LocalReport.DataSources.Add(new ReportDataSource("MyDataSet", dsReport.Tables[0]));
                ViewBag.ReportViewer = reportviewer;

                connection.Close();

                return View();


            }
        }


        NEWDATASET dsReport2 = new NEWDATASET();

        public ActionResult GraphReportofBusinessPerClassification()
        {

            using (SqlConnection connection2 = new SqlConnection(constring))
            {

                ReportViewer reportviewer2 = new ReportViewer();
                reportviewer2.ProcessingMode = ProcessingMode.Local;
                reportviewer2.SizeToReportContent = true;
                reportviewer2.Width = Unit.Percentage(200);
                reportviewer2.Height = Unit.Percentage(200);

                SqlCommand command2 = connection2.CreateCommand();
                command2.CommandType = System.Data.CommandType.StoredProcedure;
                command2.CommandText = "ReportCountofBusinessPerClassification";
                //command.Parameters.AddWithValue("@ClassificationId", ClassificationId);
                //command.Parameters.AddWithValue("@BusinessId", BusinessId);
                SqlDataAdapter sqlDA2 = new SqlDataAdapter(command2);
                DataTable dtListofBusiness2 = new DataTable();

                connection2.Open();
                //sqlDA.Fill(dtListofBusiness);

                sqlDA2.Fill(dsReport2, dsReport2.ReportCountofBusinessPerClassification.TableName);
                string path2 = Path.Combine(Server.MapPath("~/Reports"), "Report2.rdlc");
                //reportviewer.LocalReport.ReportPath = Server.MapPath(@"/report/Report1.rdlc");
                reportviewer2.LocalReport.ReportPath = path2;
                reportviewer2.LocalReport.DataSources.Add(new ReportDataSource("NEWDATASET", dsReport2.Tables[0]));
                ViewBag.ReportViewer2 = reportviewer2;

                connection2.Close();

            }

            return View();

        }
    }
}