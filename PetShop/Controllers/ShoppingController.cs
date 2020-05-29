using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;

namespace PetShop.Controllers
{
    public class ShoppingController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Shopping
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShopForDog()
        {
            var products = db.Products.Where(x => x.PetId == 1).ToList();
            return View(products);
        }

        public ActionResult MenuFoodForDog(int parentId = 2, int petId = 1)
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId && x.PetId == petId).ToList();
            return PartialView(categories);
        }

        public ActionResult MenuAccessoryForDog(int parentId = 1, int petId = 1)
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId && x.PetId == petId).ToList();
            return PartialView(categories);
        }

        public ActionResult MenuToyForDog(int parentId = 3, int petId = 1)
        {
            var categories = db.Categories.Where(x => x.ParentId == parentId && x.PetId == petId).ToList();
            return PartialView(categories);
        }
    }
}