using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Spedizioni.Models
{
    public class Shipments
    {
        public int IdShipping { get; set; }

        [Required]
        [Display(Name = "Mittente")]
        public int IdUser { get; set; }

        [Display(Name = "Codice spedizione")]
        public string Code { get; set; }

        [Display(Name = "Città di destinazione")]
        public string DestinationCity { get; set; }

        [Display(Name = "Indirizzo di destinazione")]
        public string DestinationAddress { get; set; }

        [Display(Name = "Nome recapito")]
        public string RecipientName { get; set; }

        [Display(Name = "Prezzo")]
        public double Price { get; set; }

        [Display(Name = "Peso in grammi")]
        public double Weight { get; set; }

        [Display(Name = "Data di spedizione")]
        [DataType(DataType.Date)]
        public DateTime ShippingDate { get; set; }

        [Display(Name = "Data presunta di consegna")]
        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }

        public Users sender { get; set; }

        public List<Details> details = new List<Details>();
        public Details detail { get; set; }

        public static List<Shipments> GetAllShipments()
        {
            List<Shipments> shipments = new List<Shipments>();

            string connString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select IdShipping, Code, DestinationAddress, DestinationCity, ShippingDate , Name, Surname FROM Shipments as S Inner JOIN Users as U ON s.IdUser = U.IdUser", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Users sender = new Users();
                        sender.Name = reader["Name"].ToString();
                        sender.Surname = reader["Surname"].ToString();

                        Shipments s = new Shipments();
                        s.IdShipping = Convert.ToInt16(reader["IdShipping"].ToString());
                        s.Code = reader["Code"].ToString();
                        s.DestinationAddress = reader["DestinationAddress"].ToString();
                        s.DestinationCity = reader["DestinationCity"].ToString();
                        s.ShippingDate = Convert.ToDateTime(reader["ShippingDate"].ToString());
                        s.sender = sender;
                        shipments.Add(s);
                    }
                }
            }
            catch { Exception ex; }
            finally { conn.Close(); }

            return shipments;
        }

        public static Shipments GetShipmentById(int id)
        {
            Shipments s = new Shipments();

            string connString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select IdShipping, Code,Weight, Price, RecipientName, DestinationAddress, DestinationCity, ShippingDate, DeliveryDate , Name, Surname FROM Shipments as S Inner JOIN Users as U ON s.IdUser = U.IdUser WHERE IdShipping=@IdShipping", conn);
                cmd.Parameters.AddWithValue("IdShipping", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Users sender = new Users();
                        sender.Name = reader["Name"].ToString();
                        sender.Surname = reader["Surname"].ToString();

                        s.IdShipping = Convert.ToInt16(reader["IdShipping"].ToString());
                        s.Price = Convert.ToDouble(reader["Price"]);
                        s.Weight = Convert.ToDouble(reader["Weight"]);
                        s.Code = reader["Code"].ToString();
                        s.DestinationAddress = reader["DestinationAddress"].ToString();
                        s.DestinationCity = reader["DestinationCity"].ToString();
                        s.RecipientName = reader["RecipientName"].ToString();
                        s.ShippingDate = Convert.ToDateTime(reader["ShippingDate"]);
                        s.DeliveryDate = Convert.ToDateTime(reader["DeliveryDate"]);
                        s.sender = sender;
                    }
                }
            }
            catch { Exception ex; }
            finally { conn.Close(); }

            return s;
        }

        public static List<Shipments> GetAllTodayShipments()
        {
            List<Shipments> shipments = new List<Shipments>();
            DateTime today = DateTime.Today;
            string connString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("select IdShipping, Code, DestinationAddress, DestinationCity, ShippingDate , Name, Surname FROM Shipments as S Inner JOIN Users as U ON s.IdUser = U.IdUser Where DeliveryDate=@DeliveryDate", conn);
                cmd.Parameters.AddWithValue("DeliveryDate", today);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Users sender = new Users();
                        sender.Name = reader["Name"].ToString();
                        sender.Surname = reader["Surname"].ToString();

                        Shipments s = new Shipments();
                        s.IdShipping = Convert.ToInt16(reader["IdShipping"].ToString());
                        s.Code = reader["Code"].ToString();
                        s.DestinationAddress = reader["DestinationAddress"].ToString();
                        s.DestinationCity = reader["DestinationCity"].ToString();
                        s.ShippingDate = Convert.ToDateTime(reader["ShippingDate"].ToString());
                        s.sender = sender;
                        shipments.Add(s);
                    }
                }
            }
            catch { Exception ex; }
            finally { conn.Close(); }

            return shipments;
        }

        public static int GetNumberShipmentsInDelivery()
        {
            int count = 0;
            string connString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT count (distinct D.IdShipping) as Tot FROM details as D INNER JOIN Shipments as S ON D.IdShipping = S.IdShipping WHERE D.IdShipping NOT IN(SELECT IdShipping FROM details WHERE State = 'Consegnato')", conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        count = Convert.ToInt16(reader["Tot"]);
                    }
                }
            }
            catch { Exception ex; }
            finally { conn.Close(); }

            return count;
        }
    }
}