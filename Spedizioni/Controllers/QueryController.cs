using Spedizioni.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Spedizioni.Controllers
{
    public class QueryController : Controller
    {
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ShippingsToday()
        {
            List<Shipments> shippingsToday = Shipments.GetAllTodayShipments();

            return Json(shippingsToday, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InDelivery()
        {
            int NumShippingsToday = Shipments.GetNumberShipmentsInDelivery();

            return Json(NumShippingsToday, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShippingsTodayByCity()
        {
            List<TotShipByCity> shippingsByCity = TotShipByCity.GetAllShipmentsByCity();

            return Json(shippingsByCity, JsonRequestBehavior.AllowGet);
        }
    }
}