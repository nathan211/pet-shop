using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;

namespace PetShop.Controllers
{
    public class CustomerController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(string username, string password)
        {
            var customer = db.Customers.Where(x => x.Username == username && x.Password == password).FirstOrDefault();
            if(String.IsNullOrEmpty(username))
            {
                return Json("UsernameIsEmpty", JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (String.IsNullOrEmpty(password))
                {
                    return Json("PasswordIsEmpty", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if(customer != null)
                    {
                        Session["UserLogin"] = customer;
                        return Json(customer.Username, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json("IncorrectUser", JsonRequestBehavior.AllowGet);
                    }
                }
            }           
        }

        public ActionResult ProfileName()
        {
            if(Session["UserLogin"] != null)
            {
                ViewBag.Profile = ((Customer)Session["UserLogin"]).Username;
                return PartialView();
            }
            ViewBag.Profile = "Đăng nhập/ Đăng ký";
            return PartialView();
        }

        public ActionResult LogOut()
        {
            Session["UserLogin"] = null;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult Register(Customer customer)
        {
            var customerNew = new Customer();
            customerNew.FullName = customer.FullName;
            customerNew.Gender = customer.Gender;
            customerNew.Username = customer.Username;
            customerNew.Password = customer.Password;
            customerNew.Address = customer.Address;
            customerNew.Phone = customer.Phone;
            customerNew.Email = customer.Email;
            customerNew.Status = true;
            db.Customers.InsertOnSubmit(customerNew);
            db.SubmitChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        
    }
}