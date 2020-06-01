using PetShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PetShop.Controllers
{
    public class CartController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Cart
        public ActionResult Index()
        {
            List<Cart> list = TakeCart();
            if (list.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Total = Total();
            ViewBag.TotalMoney = TotalMoney();

            return View(list);
        }

        public List<Cart> TakeCart()
        {
            List<Cart> list = Session["Cart"] as List<Cart>;
            if (list == null)
            {
                list = new List<Cart>();
                Session["Cart"] = list;
            }
            return list;
        }

        [HttpPost]
        public JsonResult AddToCart(long id)
        {
            List<Cart> list = TakeCart();
            Cart cartItem = list.Find(x => x.Id == id);
            if (cartItem == null)
            {
                cartItem = new Cart(id);
                list.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }
            var counter = list.Sum(x => x.Count);

            return Json(counter, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IncreaseCount(long id, int count)
        {
            List<Cart> list = TakeCart();
            Cart cartItem = list.Find(x => x.Id == id);

            cartItem.Count++;

            var counter = cartItem.Count;
            var total = String.Format("{0:0,0}", TotalMoney());

            return Json(new { Count = counter, TotalMoney = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DecreaseCount(long id, int count)
        {
            List<Cart> list = TakeCart();
            Cart cartItem = list.Find(x => x.Id == id);

            cartItem.Count--;

            var counter = cartItem.Count;
            var total = String.Format("{0:0,0}", TotalMoney());

            return Json(new { Count = counter, TotalMoney = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddToCartInDetails(long id, int count)
        {
            List<Cart> list = TakeCart();
            Cart cartItem = list.Find(x => x.Id == id);

            if (cartItem == null)
            {
                cartItem = new Cart(id);
                cartItem.Count = count;
                list.Add(cartItem);
            }
            else
            {
                cartItem.Count += count;
            }

            var counter = list.Sum(x => x.Count);

            return Json(counter, JsonRequestBehavior.AllowGet);
        }

        public int Total()
        {
            int total = 0;
            List<Cart> list = Session["Cart"] as List<Cart>;
            if (list != null)
            {
                total = list.Sum(x => x.Count);
            }
            return total;
        }

        public decimal TotalMoney()
        {
            decimal totalMoney = 0;
            List<Cart> list = Session["Cart"] as List<Cart>;
            if (list != null)
            {
                totalMoney = list.Sum(x => x.TotalPrice);
            }
            return totalMoney;
        }


        public ActionResult CartCounter()
        {
            if (Session["Cart"] != null)
            {
                ViewBag.Total = Total();
                return PartialView();
            }
            ViewBag.Total = 0;
            return PartialView();
        }

        public ActionResult RemoveFromCart(long id)
        {
            List<Cart> list = TakeCart();
            Cart item = list.SingleOrDefault(x => x.Id == id);
            if (item != null)
            {
                list.RemoveAll(x => x.Id == id);
                return RedirectToAction("Index");
            }
            if (list.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }


        public ActionResult RemoveAll()
        {
            List<Cart> list = TakeCart();
            list.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CheckOut()
        {

            List<Cart> list = TakeCart();
            if (list.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Total = Total();
            ViewBag.TotalMoney = TotalMoney();
            ViewBag.CustomerInfo = Session["UserLogin"];
            return View(list);
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
                List<Cart> cartItems = TakeCart();
                
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.OrderId = order.Id;
                    orderDetail.ProductId = item.Id;
                    orderDetail.Count = item.Count;
                    orderDetail.TotalPrice = item.Price * item.Count;
                    db.OrderDetails.InsertOnSubmit(orderDetail);
                }
            }
            db.SubmitChanges();
            Session["Cart"] = null;
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
            var cartItems = TakeCart();
            var check = 0;
            if (userSession != null)
            {
                check = 1;
            }
            return Json(check, JsonRequestBehavior.AllowGet);
        }
    }
}