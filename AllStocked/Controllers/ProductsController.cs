﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AllStocked;
using AllStocked.Models;

namespace AllStocked.Controllers
{
    public class ProductsController : Controller
    {
        private AllStockedDBEntities db = new AllStockedDBEntities();

        protected void submitted(Object sender, EventArgs e)
        {
            if (Request.Form["submitted"] != null)
            {
                string value = Request.Form["txt1"];
                Show(value);
            }
            
        }
        //Show will accept search term from form and update product list
        public ActionResult Show(string searchTerm)
        {
            int currentId = Convert.ToInt32(HttpContext.Session["AccountID"]);
            //find product names that match search term
            var products = db.Products.Where(p => p.AccountID == currentId && p.ProductName.Contains(searchTerm) ).Include(p => p.Account).Include(p => p.Category);
            var productsList = products.ToList();
            // find and add product categories that match search term
            products = ( db.Products.Where(p => p.AccountID == currentId && p.Category.CategoryName.Contains(searchTerm)).Include(p => p.Account).Include(p => p.Category) );
            foreach (var p in products)
            {
                productsList.Add(p);
            }
            return View(productsList);
        }

        // GET: Products
        public ActionResult Index()
        {
            if (SessionHelper.IsMemberLoggedIn())
            {
                int currentId = Convert.ToInt32(HttpContext.Session["AccountID"]);
                //Get list of products that have a an account id that match current session.
                var products = db.Products.Where(p => p.AccountID == currentId).Include(p => p.Account).Include(p => p.Category);
                return View(products.ToList());
            }
            else
            {
                //if current session doesnt exist redirect user to home/register page.
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName");
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,AccountID,CategoryID,ProductName,Par,Demand,Supply,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName", product.AccountID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName", product.AccountID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,AccountID,CategoryID,ProductName,Par,Demand,Supply,Description")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "AccountName", product.AccountID);
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}