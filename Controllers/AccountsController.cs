﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPlannerBD.Models;
using Microsoft.AspNet.Identity;

namespace FinancialPlannerBD.Controllers
{
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.Bank).Include(a => a.Household).Include(a => a.Type);
            return View(accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            //var userId = User.Identity.GetUserId();
            //var user = db.Users.Find(userId);

            ViewBag.BankId = new SelectList(db.Banks, "Id", "Name");
            //ViewBag.HouseholdId = user.HouseholdId;
            ViewBag.TypeId = new SelectList(db.AccountTypes, "Id", "Type");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,HouseholdId,TypeId,BankId,Created,StartingBalance,CurrentBalance")] Account account)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            if (ModelState.IsValid)
            {
                account.Created = DateTime.Now;
                account.CurrentBalance = account.StartingBalance;
                account.HouseholdId = (int)user.HouseholdId;
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Dashboard", "Households");
            }

            ViewBag.BankId = new SelectList(db.Banks, "Id", "Name", account.BankId);
            //ViewBag.HouseholdId = user.HouseholdId;
            ViewBag.TypeId = new SelectList(db.AccountTypes, "Id", "Type", account.TypeId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Name", account.BankId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", account.HouseholdId);
            ViewBag.TypeId = new SelectList(db.AccountTypes, "Id", "Type", account.TypeId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,HouseholdId,TypeId,BankId,Created,Updated,StartingBalance,CurrentBalance")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BankId = new SelectList(db.Banks, "Id", "Name", account.BankId);
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", account.HouseholdId);
            ViewBag.TypeId = new SelectList(db.AccountTypes, "Id", "Type", account.TypeId);
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
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
