using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Spedizioni.Models
{
    public class Details
    {
        public int IdState { get; set; }
        public int IdShipping { get; set; }

        [Display(Name = "Stato")]
        public string State { get; set; }

        [Display(Name = "Posizione attuale")]
        public string CurrentLocation { get; set; }

        [Display(Name = "Descrizione")]
        public string Description { get; set; }

        [Display(Name = "Data aggiornamento")]
        public DateTime UpdateTime { get; set; }

        public static List<Details> detailsByShippingId(int id)
        {
            List<Details> detailsList = new List<Details>();

            string connString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand($"select * FROM Details where IdShipping={id}", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Details detail = new Details();
                        detail.IdState = Convert.ToInt16(reader["IdState"].ToString());
                        detail.State = reader["State"].ToString();
                        detail.CurrentLocation = reader["CurrentLocation"].ToString();
                        detail.Description = reader["Description"].ToString();
                        detail.UpdateTime = Convert.ToDateTime(reader["UpdateTime"].ToString());
                        detailsList.Add(detail);
                    }
                }
            }
            catch { Exception ex; }
            finally { conn.Close(); }

            return detailsList;
        }
    }
}