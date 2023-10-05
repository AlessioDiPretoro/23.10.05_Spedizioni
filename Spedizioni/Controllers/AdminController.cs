using Spedizioni.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Spedizioni.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public List<SelectListItem> ListClients
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<Users> users = Users.GetAllUsersInfos();
                SelectListItem item1 = new SelectListItem { Text = "--- Seleziona ---", Value = "0" };
                list.Add(item1);
                foreach (Users u in users)
                {
                    SelectListItem item = new SelectListItem { Text = u.Surname + " " + u.Name, Value = u.IdUser.ToString() };
                    list.Add(item);
                }
                return list;
            }
        }

        public List<SelectListItem> ListStates
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<Users> users = Users.GetAllUsersInfos();
                list.Add(new SelectListItem { Text = "--- Seleziona ---", Value = "0" });
                list.Add(new SelectListItem { Text = "In Preparazione", Value = "1" });
                list.Add(new SelectListItem { Text = "In Transito", Value = "2" });
                list.Add(new SelectListItem { Text = "In Consegna", Value = "3" });
                list.Add(new SelectListItem { Text = "Consegnato", Value = "4" });
                list.Add(new SelectListItem { Text = "Smarrito", Value = "5" });

                return list;
            }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(Users u)
        {
            string conn = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(conn);
            try
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand("Select * FROM Users WHERE Username=@Username And Password=@Password", sqlConnection);
                cmd.Parameters.AddWithValue("Username", u.Username);
                cmd.Parameters.AddWithValue("Password", u.Password);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    FormsAuthentication.SetAuthCookie(u.Username, true);
                    sqlConnection.Close();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.AuthError = "Autenticazione non riuscita";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.AuthError = "Errore generico" + ex;
            }
            finally
            {
                sqlConnection.Close();
            }
            return View();
            //return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CreateClient()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CreateClient([Bind(Exclude = "Role")] Users u)
        {
            if (ModelState.IsValid)
            {
                if (u.IsPrivate && u.CFisc == null)
                {
                    ViewBag.MessaggioDiErrore = "Inserire il codice fiscale";
                    return View();
                }
                else if (!u.IsPrivate && u.PIva == null)
                {
                    ViewBag.MessaggioDiErrore = "Inserire la partita IVA";
                    return View();
                }
                else
                {//crea utente
                    string conn = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                    SqlConnection sqlConnection = new SqlConnection(conn);
                    try
                    {
                        sqlConnection.Open();
                        SqlCommand cmd = new SqlCommand("INSERT INTO Users VALUES (@Username, @Password, @Name, @Surname, @IsPrivate, @CFisc, @PIva, @Role)", sqlConnection);
                        cmd.Parameters.AddWithValue("Username", u.Username);
                        cmd.Parameters.AddWithValue("Password", u.Password);
                        cmd.Parameters.AddWithValue("Name", u.Name);
                        cmd.Parameters.AddWithValue("Surname", u.Surname);
                        cmd.Parameters.AddWithValue("IsPrivate", Convert.ToBoolean(u.IsPrivate));
                        if (u.CFisc != null)
                        {
                            cmd.Parameters.AddWithValue("CFisc", u.CFisc);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("CFisc", "");
                        }
                        if (u.PIva != null)
                        {
                            cmd.Parameters.AddWithValue("PIva", u.PIva);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("PIva", "");
                        }
                        cmd.Parameters.AddWithValue("Role", "User");

                        int inserimentoEffettuato = cmd.ExecuteNonQuery();

                        if (inserimentoEffettuato > 0)
                        {
                            ViewBag.MessaggioDiSuccesso = "Cliente inserito correttamente";
                            sqlConnection.Close();
                            ModelState.Clear();
                            return View();
                        }
                        else
                        {
                            ViewBag.MessaggioDiErrore = "Errore nel salvataggio, riprovare";
                            return View();
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewBag.MessaggioDiErrore = "Errore generico:" + ex;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
                return View();
            }
            else
            {
                ViewBag.MessaggioDiErrore = "Compilare correttamente i dati cliente";
                return View();
            }
        }

        public ActionResult IsUserValid(string username)
        {
            bool isValid = false;
            string conn = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SqlConnection sqlConnection = new SqlConnection(conn);
            try
            {
                sqlConnection.Open();
                SqlCommand cmd = new SqlCommand("Select * FROM Users WHERE Username=@Username", sqlConnection);
                cmd.Parameters.AddWithValue("Username", username);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    isValid = false;
                }
                else
                {
                    isValid = true;
                }
            }
            catch (Exception ex)
            {
                ViewBag.MessaggioDiErrore = "Errore generico:" + ex;
            }
            finally
            {
                sqlConnection.Close();
            }

            return Json(isValid, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateShipment()
        {
            ViewBag.AllClients = ListClients;
            return View();
        }

        [HttpPost]
        public ActionResult CreateShipment(Shipments s)
        {
            if (ModelState.IsValid)
            {
                string conn = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                SqlConnection sqlConnection = new SqlConnection(conn);
                SqlConnection sqlConnection2 = new SqlConnection(conn);
                try
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Shipments VALUES (@IdUser, @Code, @ShippingDate, @Weight, @DestinationCity, @DestinationAddress, @RecipientName, @Price, @DeliveryDate)", sqlConnection);
                    cmd.Parameters.AddWithValue("IdUser", Convert.ToInt16(s.IdUser));
                    cmd.Parameters.AddWithValue("ShippingDate", s.ShippingDate.ToShortDateString());
                    cmd.Parameters.AddWithValue("Weight", Convert.ToDouble(s.Weight));
                    cmd.Parameters.AddWithValue("DestinationCity", s.DestinationCity);
                    cmd.Parameters.AddWithValue("DestinationAddress", s.DestinationAddress);
                    cmd.Parameters.AddWithValue("RecipientName", s.RecipientName);
                    cmd.Parameters.AddWithValue("Price", Convert.ToDouble(s.Price));
                    cmd.Parameters.AddWithValue("DeliveryDate", s.DeliveryDate.ToShortDateString());
                    cmd.Parameters.AddWithValue("Code", "codiceProva");

                    int inserimentoEffettuato = cmd.ExecuteNonQuery();
                    int lastID = 0;
                    if (inserimentoEffettuato > 0)
                    {
                        SqlCommand cmdGetLastId = new SqlCommand("Select IdShipping FROM Shipments ORDER BY IdShipping DESC", sqlConnection);
                        SqlDataReader sqlDataReader = cmdGetLastId.ExecuteReader();
                        while (sqlDataReader.Read())
                        {
                            lastID = Convert.ToInt16(sqlDataReader["IdShipping"].ToString());
                            break;
                        }

                        // string getIdQuery = "SELECT SCOPE_IDENTITY();";
                        // cmd.CommandText = getIdQuery;
                        // int insertedId = Convert.ToInt32(cmd.ExecuteScalar());
                        sqlConnection.Close();

                        sqlConnection2.Open();
                        SqlCommand updateCode = new SqlCommand("UPDATE Shipments SET Code=@Code WHERE IdShipping=@IdShipping", sqlConnection2);
                        updateCode.Parameters.AddWithValue("IdShipping", lastID);
                        updateCode.Parameters.AddWithValue("Code", $"SDP-{lastID}-{s.ShippingDate.ToString("ddMMyyyy")}");
                        updateCode.ExecuteNonQuery();

                        ViewBag.AllClients = ListClients;
                        ViewBag.messageValid = "Spedizione inserita correttamente";
                        return View();
                    }
                    else
                    {
                        ViewBag.AllClients = ListClients;
                        ViewBag.messageError = "Errore nel complilamento del form";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.AllClients = ListClients;
                    ViewBag.messageError = "Errore generico:" + ex;
                    return View();
                }
                finally
                {
                    sqlConnection.Close();
                    sqlConnection2.Close();
                }
            }
            else
            {
                ViewBag.AllClients = ListClients;
                ViewBag.messageError = "Errore nel complilamento del form";
                return View();
            }
        }

        public ActionResult AllShipments()
        {
            List<Shipments> shipments = Shipments.GetAllShipments();
            return View(shipments);
        }

        public ActionResult EditShipment(int id)
        {
            Shipments shipment = Shipments.GetShipmentById(id);
            return View(shipment);
        }

        [HttpPost]
        public ActionResult EditShipment(Shipments s)
        {
            if (ModelState.IsValid)
            {
                string conn = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                SqlConnection sqlConnection = new SqlConnection(conn);
                try
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Shipments SET ShippingDate=@ShippingDate, Weight=@Weight, DestinationCity=@DestinationCity, DestinationAddress=@DestinationAddress, RecipientName=@RecipientName, Price=@Price, DeliveryDate=@DeliveryDate where IdShipping=@IdShipping", sqlConnection);
                    cmd.Parameters.AddWithValue("IdShipping", s.IdShipping);
                    cmd.Parameters.AddWithValue("ShippingDate", s.ShippingDate.ToShortDateString());
                    cmd.Parameters.AddWithValue("Weight", Convert.ToDouble(s.Weight));
                    cmd.Parameters.AddWithValue("DestinationCity", s.DestinationCity);
                    cmd.Parameters.AddWithValue("DestinationAddress", s.DestinationAddress);
                    cmd.Parameters.AddWithValue("RecipientName", s.RecipientName);
                    cmd.Parameters.AddWithValue("Price", Convert.ToDouble(s.Price));
                    cmd.Parameters.AddWithValue("DeliveryDate", s.DeliveryDate.ToShortDateString());

                    int inserimentoEffettuato = cmd.ExecuteNonQuery();
                    if (inserimentoEffettuato > 0)
                    {
                        ViewBag.AllClients = ListClients;
                        ViewBag.messageValid = "Spedizione modificata correttamente";
                        return RedirectToAction("AllShipments", "Admin");
                    }
                    else
                    {
                        ViewBag.AllClients = ListClients;
                        ViewBag.messageError = "Errore nel complilamento del form";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.AllClients = ListClients;
                    ViewBag.messageError = "Errore generico:" + ex;
                    return View();
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            else
            {
                ViewBag.AllClients = ListClients;
                ViewBag.messageError = "Errore nel complilamento del form";
                return View();
            }
        }

        public ActionResult CreateDetails(int id)
        {
            ViewBag.AllStates = ListStates;
            Details detail = new Details();
            Shipments shipment = Shipments.GetShipmentById(id);
            detail.IdShipping = id;
            shipment.details.Add(detail);
            return View(shipment);
        }

        [HttpPost]
        public ActionResult CreateDetails(Shipments s)
        {
            s.detail.UpdateTime = DateTime.Now;

            if (ModelState.IsValid)
            {
                string conn = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
                SqlConnection sqlConnection = new SqlConnection(conn);
                try
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Details VALUES (@IdShipping, @State, @CurrentLocation, @Description, @UpdateTime)", sqlConnection);
                    cmd.Parameters.AddWithValue("IdShipping", Convert.ToInt16(s.IdShipping));
                    cmd.Parameters.AddWithValue("State", s.detail.State);
                    cmd.Parameters.AddWithValue("CurrentLocation", s.detail.CurrentLocation);
                    if (s.detail.Description != null)
                    {
                        cmd.Parameters.AddWithValue("Description", s.detail.Description);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("Description", "");
                    }
                    cmd.Parameters.AddWithValue("UpdateTime", s.detail.UpdateTime);

                    int inserimentoEffettuato = cmd.ExecuteNonQuery();

                    if (inserimentoEffettuato > 0)
                    {
                        sqlConnection.Close();

                        ViewBag.AllStates = ListStates;
                        ViewBag.messageValid = "Dettaglio inserito correttamente";
                        ModelState.Clear();
                        return RedirectToAction("AllShipments", "Admin");
                    }
                    else
                    {
                        ViewBag.AllStates = ListStates;
                        ViewBag.messageError = "Errore nel complilamento del form";
                        return View(s);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.AllStates = ListStates;
                    ViewBag.messageError = "Errore generico:" + ex;
                    return View(s);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
            else
            {
                ViewBag.AllStates = ListStates;
                ViewBag.messageError = "Errore nel complilamento del form";
                return View();
            }
        }

        public ActionResult DeleteDetail(int id)
        {
            string conString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

            SqlConnection conn = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM Details WHERE IdState=@id";
                cmd.Parameters.AddWithValue("id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
            }
            finally { conn.Close(); }

            return RedirectToAction("AllShipments", "Admin");
        }

        public ActionResult DeleteShipment(int id)
        {
            string conString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;

            SqlConnection conn = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM Shipments WHERE IdShipping=@id";
                cmd.Parameters.AddWithValue("id", id);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception ex)
            {
            }
            finally { conn.Close(); }

            return RedirectToAction("AllShipments", "Admin");
        }

        public ActionResult GetPartialDetail(int id)
        {
            List<Details> listDetails = Details.detailsByShippingId(id);

            return PartialView("_GetPartialDetail", listDetails);
        }
    }
}