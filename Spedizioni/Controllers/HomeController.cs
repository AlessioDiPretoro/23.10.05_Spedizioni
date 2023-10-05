using Spedizioni.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Spedizioni.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchShipment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchShipment(Search s)
        {
            return View(s);
        }

        public ActionResult GetShipmentPartial(Search s)
        {
            Shipments shipment = Search.SearchShip(s);
            List<Details> d = Details.detailsByShippingId(shipment.IdShipping);
            shipment.details = d;
            return PartialView("_GetShipmentPartial", shipment);
        }
    }
}