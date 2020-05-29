using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;

namespace PetShop.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Admin/Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllProducts()
        {
            var products = db.Products.ToList();
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Suppliers.ToList(), "Id", "Name");
            return View(products);
        }

        
    }
}