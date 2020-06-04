using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;

namespace PetShop.Areas.Admin.Controllers
{
    public class CustomerController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Admin/Customer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllCustomers()
        {
            var customers = db.Customers.ToList();
            return PartialView(customers);
        }

        [HttpDelete]
        public ActionResult Delete(long id)
        {
            var customer = db.Customers.Where(user => user.Id == id).FirstOrDefault();
            if (customer != null)
            {
                db.Customers.DeleteOnSubmit(customer);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public JsonResult Details(long id)
        {
            var customer = db.Customers.Where(x => x.Id == id).FirstOrDefault();
            if (customer != null)
            {
                return Json(new
                {
                    FullName = customer.FullName,
                    Gender = customer.Gender,
                    Username = customer.Username,
                    Address = customer.Address,
                    Phone = customer.Phone,
                    Email = customer.Email
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("CannotFindCustomer", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Update(long id, string fullName, bool gender, string username, string address, string phone, string email)
        {

            var customer = db.Customers.Where(x => x.Id == id).FirstOrDefault();
            customer.FullName = fullName;
            customer.Gender = gender;
            customer.Username = username;
            customer.Address = address;
            customer.Phone = phone;
            customer.Email = email;

            UpdateModel(customer);
            db.SubmitChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

       
    }
}