using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PetShop.Models;

namespace PetShop.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Admin/Order
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ListAllOrders(int? page, string searchStr)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (String.IsNullOrEmpty(searchStr))
            {
                searchStr = " ";
            }

            var orders = db.Orders.Where(x => x.Customer.FullName.Contains(searchStr)
            || x.Customer.Username.Contains(searchStr)).ToList();
            ViewBag.StatusId = new SelectList(db.Status.ToList(), "Id", "Name");
            return View(orders.ToPagedList(page ?? 1, 5));
        }

        public ActionResult ListOrderDetailsByOrderId(long id)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var orderDetails = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            return PartialView(orderDetails);
        }

        [HttpPost]
        public ActionResult OrderStatus(long id)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
            return Json(order.StatusId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateStatus(long id, int statusId)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
            if(order != null)
            {
                order.StatusId = statusId;
                UpdateModel(order);
                db.SubmitChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("CannotFindOrder", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete]
        public ActionResult Delete(long id)
        {
            if (Session["AdminLogin"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            var orderDetails = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            var order = db.Orders.Where(x => x.Id == id).FirstOrDefault();
            if (orderDetails != null)
            {
                foreach(var item in orderDetails)
                {
                    db.OrderDetails.DeleteOnSubmit(item);
                    db.SubmitChanges();
                }
            }
            if (order != null)
            {
                db.Orders.DeleteOnSubmit(order);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}