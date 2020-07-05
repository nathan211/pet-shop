using PetShop.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShop.Controllers
{
    public class CartController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();
        static List<Cart> listCartItem = new List<Cart>();

        // GET: Cart
        public ActionResult Index()
        {
            if (listCartItem.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Total = Total();
            ViewBag.TotalMoney = TotalMoney();

            return View(listCartItem);
        }


        [HttpPost]
        public JsonResult AddToCart(long id)
        {
            
            Cart cartItem = listCartItem.Find(x => x.Id == id);
            if (cartItem == null)
            {
                cartItem = new Cart(id);
                listCartItem.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }
            var counter = listCartItem.Sum(x => x.Count);

            return Json(counter, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IncreaseCount(long id, int count)
        {
          
            Cart cartItem = listCartItem.Find(x => x.Id == id);

            cartItem.Count++;

            var counter = cartItem.Count;
            var total = String.Format("{0:0,0}", TotalMoney());

            return Json(new { Count = counter, TotalMoney = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DecreaseCount(long id, int count)
        {
            Cart cartItem = listCartItem.Find(x => x.Id == id);

            cartItem.Count--;

            var counter = cartItem.Count;
            var total = String.Format("{0:0,0}", TotalMoney());

            return Json(new { Count = counter, TotalMoney = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddToCartInDetails(long id, int count)
        {
            Cart cartItem = listCartItem.Find(x => x.Id == id);

            if (cartItem == null)
            {
                cartItem = new Cart(id);
                cartItem.Count = count;
                listCartItem.Add(cartItem);
            }
            else
            {
                cartItem.Count += count;
            }

            var counter = listCartItem.Sum(x => x.Count);

            return Json(counter, JsonRequestBehavior.AllowGet);
        }

        public int Total()
        {
            int total = 0;
            if (listCartItem != null)
            {
                total = listCartItem.Sum(x => x.Count);
            }
            return total;
        }

        public decimal TotalMoney()
        {
            decimal totalMoney = 0;
            if (listCartItem != null)
            {
                totalMoney = listCartItem.Sum(x => x.TotalPrice);
            }
            return totalMoney;
        }


        public ActionResult CartCounter()
        {
            if (listCartItem != null)
            {
                ViewBag.Total = Total();
                return PartialView();
            }
            ViewBag.Total = 0;
            return PartialView();
        }

        public ActionResult RemoveFromCart(long id)
        {
            Cart item = listCartItem.SingleOrDefault(x => x.Id == id);
            if (item != null)
            {
                listCartItem.RemoveAll(x => x.Id == id);
                return RedirectToAction("Index");
            }
            if (listCartItem.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }


        public ActionResult RemoveAll()
        {
            listCartItem.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CheckOut()
        {

            if (listCartItem.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Total = Total();
            ViewBag.TotalMoney = TotalMoney();
            ViewBag.CustomerInfo = Session["UserLogin"];
            return View(listCartItem);
        }

        [HttpPost]
        public JsonResult UpdateAddress(long id, string address)
        {
            var customer = db.Customers.Where(x => x.Id == id).FirstOrDefault();

            if (customer != null)
            {
                customer.Address = address;
                UpdateModel(customer);
                db.SubmitChanges();
                Session["UserLogin"] = customer;
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("CannotFindCustomer", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult InsertOrder()
        {
            var user = (Customer)Session["UserLogin"];
            var order = new Order();

            order.CustomerId = user.Id;
            order.CreatedDate = DateTime.Now;
            order.StatusId = 1;
            order.TotalMoney = TotalMoney();
            db.Orders.InsertOnSubmit(order);
            db.SubmitChanges();

            var orderNew = db.Orders.OrderByDescending(x => x.Id).First();

            if (orderNew != null)
            {
                foreach (var item in listCartItem)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.OrderId = order.Id;
                    orderDetail.ProductId = item.Id;
                    orderDetail.Count = item.Count;
                    orderDetail.TotalPrice = item.Price * item.Count;
                    db.OrderDetails.InsertOnSubmit(orderDetail);
                    db.SubmitChanges();
                }
            }

            listCartItem.Clear();

            string content = System.IO.File.ReadAllText(Server.MapPath("~/Assets/Email/SendMail.html"));
            content = content.Replace("{{CustomerName}}", user.FullName);
            content = content.Replace("{{Phone}}", user.Phone);
            content = content.Replace("{{Email}}", user.Email);
            content = content.Replace("{{Address}}", user.Address);
            content = content.Replace("{{Total}}", order.TotalMoney.ToString());

            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();
            new SendMail().Mail(toEmail, "Đơn hàng mới từ pet shop", content);
            new SendMail().Mail(user.Email, "Đơn hàng mới từ pet shop", content);

           
            return RedirectToAction("CheckOutConfirm");
        }

        public ActionResult CheckOutConfirm()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CheckSession()
        {
            var userSession = (Customer)Session["UserLogin"];
            var check = 0;
            if (userSession != null)
            {
                check = 1;
            }
            return Json(check, JsonRequestBehavior.AllowGet);
        }
    }
}