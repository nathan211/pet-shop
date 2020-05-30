using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;

namespace PetShop.Controllers
{
    public class HomeController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        public ActionResult Index()
        {
            var products = db.Products.Take(18).ToList();
            return View(products);
        }


        // product details
        public ActionResult Details(long id)
        {
            var product = db.Products.Single(x => x.Id == id);
            ViewBag.ProductsOfTheSameType = ListByCategoryId(product.CategoryId, 12);
            return View(product);
        }

        public List<Product> ListByCategoryId(long? id, int count)
        {
            var products = db.Products.Where(x => x.CategoryId == id).Take(count).ToList();

            return products;
        }


        // changed into the top 3 menu
        public ActionResult ListByMenuSide(long id)
        {
            var products = ListByCategoryId(id, 8);
            var category = db.Categories.Single(x => x.Id == id);
            ViewBag.CategoryTitle = category.Name;
            return View(products);
        }
    }
}