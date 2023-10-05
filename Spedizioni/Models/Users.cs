using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Spedizioni.Models
{
    public class Users
    {
        public int IdUser { get; set; }

        [Required]
        [Remote("IsUserValid", "Admin", ErrorMessage = "Nome utente già in uso")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Display(Name = "Cognome")]
        public string Surname { get; set; }

        [Display(Name = "Privato")]
        public bool IsPrivate { get; set; }

        [Display(Name = "Codice Fiscale")]
        public string CFisc { get; set; }

        [Display(Name = "P.IVA")]
        public string PIva { get; set; }

        public string Role { get; set; }

        public static List<Users> GetAllUsersInfos()
        {
            List<Users> users = new List<Users>();

            string connString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("Select Name, Surname, IdUser FROM Users", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Users u = new Users();
                        u.Name = reader["Name"].ToString();
                        u.Surname = reader["Surname"].ToString();
                        u.IdUser = Convert.ToInt16(reader["IdUser"].ToString());
                        users.Add(u);
                    }
                }
            }
            catch { Exception ex; }
            finally { conn.Close(); }

            return users;
        }
    }
}