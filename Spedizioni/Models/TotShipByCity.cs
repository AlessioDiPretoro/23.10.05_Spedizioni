using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Spedizioni.Models
{
    public class TotShipByCity
    {
        public string City { get; set; }
        public int numbForCity { get; set; }

        public static List<TotShipByCity> GetAllShipmentsByCity()
        {
            List<TotShipByCity> shipments = new List<TotShipByCity>();

            string connString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select COUNT (DestinationCity) as Conto, DestinationCity  FROM Shipments GROUP BY DestinationCity", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        TotShipByCity s = new TotShipByCity();
                        s.numbForCity = Convert.ToInt16(reader["Conto"]);
                        s.City = reader["DestinationCity"].ToString();
                        shipments.Add(s);
                    }
                }
            }
            catch { Exception ex; }
            finally { conn.Close(); }

            return shipments;
        }
    }
}