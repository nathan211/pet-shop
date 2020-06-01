using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetShop.Models;

namespace PetShop.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        PetShopDataContext db = new PetShopDataContext();

        // GET: Admin/Category
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListAllCategories()
        {
            var categories = db.Categories.ToList();
            ViewBag.ParentId = new SelectList(db.ParentCategories.ToList(), "Id", "Name");
            return View(categories);
        }

        public ActionResult AddNew()
        {
            ViewBag.ParentId = new SelectList(db.ParentCategories.ToList(), "Id", "Name");
            return PartialView();
        }
    }
}