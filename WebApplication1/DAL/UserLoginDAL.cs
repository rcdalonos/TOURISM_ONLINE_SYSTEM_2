using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

using System.Data;
using WebApplication1.Models;


namespace WebApplication1.DAL
{

    public class UserLoginDAL
    {
        string constring = ConfigurationManager.ConnectionStrings["adoConnectionstring"].ToString();

        public string InsertNewUser(LoginModelClass loginModelClass)
        {
            string IDinserted = "";
            int id = 0;
            using (SqlConnection connection = new SqlConnection(constring))
            {
                loginModelClass.imgext = "";
                loginModelClass.imgname = "";
                loginModelClass.imgpath = "";
                SqlCommand command = new SqlCommand("sp_InsertUserAccount", connection);
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
                command.Parameters.Add("@UserId", SqlDbType.Int).Direction = ParameterDirection.Output;
                SqlDataAdapter sqlDA = new SqlDataAdapter(command);
                DataTable dtProducts = new DataTable();
                //IDinserted = (int)command.ExecuteScalar();

                connection.Open();

                id = command.ExecuteNonQuery();
                //IDinserted = command.Parameters["UserId"].Value.ToString();
                connection.Close();
            }
            if (id > 0)
            {
                //IDinserted
                return IDinserted;
            }
            else
            {
                return "";
            }
        }


    }


}