using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace Spedizioni.Models
{
    public class Search
    {
        [Display(Name = "Codice Fiscale")]
        public string codFisc { get; set; }

        [Display(Name = "P.IVA")]
        public string pIva { get; set; }

        [Display(Name = "Codice Spedizione")]
        public string code { get; set; }

        public bool isPrivate { get; set; }

        public static Shipments SearchShip(Search s)
        {
            Shipments shipment = new Shipments();
            Users user = new Users();

            string conn = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(conn);
            try
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand($"SELECT Price, IdShipping, RecipientName, DestinationAddress, DestinationCity, ShippingDate, DeliveryDate, Weight, Name, Surname, CFisc, PIva FROM Shipments as S INNER JOIN Users as U on S.IdUser = U.IdUser Where code=@code", sqlConnection);
                cmd.Parameters.AddWithValue("code", s.code);
                if (s.codFisc != null)
                {
                    cmd.CommandText += string.Concat($" And CFisc=@CFisc");
                    cmd.Parameters.AddWithValue("CFisc", s.codFisc);
                }
                else
                {
                    cmd.CommandText += string.Concat($" And PIva=@PIva");
                    cmd.Parameters.AddWithValue("PIva", s.pIva);
                }
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        user.Surname = reader["Surname"].ToString();
                        user.Name = reader["Name"].ToString();
                        shipment.IdShipping = Convert.ToInt32(reader["IdShipping"].ToString());
                        shipment.ShippingDate = Convert.ToDateTime(reader["ShippingDate"].ToString());
                        shipment.DeliveryDate = Convert.ToDateTime(reader["DeliveryDate"].ToString());
                        shipment.Weight = Convert.ToDouble(reader["Weight"].ToString());
                        shipment.DestinationAddress = reader["DestinationAddress"].ToString();
                        shipment.DestinationCity = reader["DestinationCity"].ToString();
                        shipment.RecipientName = reader["RecipientName"].ToString();
                        shipment.Price = Convert.ToDouble(reader["Price"].ToString());
                        shipment.sender = user;
                    }
                    sqlConnection.Close();
                }
                else
                {
                }
            }
            catch { }
            finally { sqlConnection.Close(); }

            return shipment;
        }
    }
}