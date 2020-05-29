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

        [HttpGet]
        public ActionResult AddNew()
        {
            ViewBag.CategoryId = new SelectList(db.Categories.ToList(), "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.Suppliers.ToList(), "Id", "Name");
            return PartialView();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AddNew(Product product)
        {
            if (ModelState.IsValid)
            {
                product.Status = true;
                product.CreatedDate = DateTime.Now;
                db.Products.InsertOnSubmit(product);
                db.SubmitChanges();
                return RedirectToAction("ListAllProducts");
            }
            return this.AddNew();
        }

        [HttpPost]
        public JsonResult ProductInfo(long id)
        {
            var product = db.Products.Where(x => x.Id == id).FirstOrDefault();
            if (product != null)
            {
                return Json(new
                {
                    Name = product.Name,
                    Price = product.Price,
                    Image = product.Image,
                    Description = product.Description,
                    Quantity = product.Quantity,
                    CategoryId = product.CategoryId,
                    SupplierId = product.SupplierId

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("CannotFindProduct", JsonRequestBehavior.AllowGet);
            }
        }

    }
}