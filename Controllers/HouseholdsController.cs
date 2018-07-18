using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPlannerBD.Helpers;
using FinancialPlannerBD.Models;
using Microsoft.AspNet.Identity;

namespace FinancialPlannerBD.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        // GET: Households
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        // GET: Households/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Dashboard
        public ActionResult Dashboard(int? houseId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            houseId = user.HouseholdId;
            Household household = db.Households.Find(houseId);
            if (houseId != null)
            {
                return View(household);
            }
            else
            {
                return RedirectToAction("Join", "Invitations");
            }
        }

        // GET: Households/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Household household)
        {
            if (ModelState.IsValid)
            {

                //Need to set whoever made the hosue to be the head of household
                var userId = User.Identity.GetUserId();
                roleHelper.AddUserToRole(userId, "HeadOfHousehold");

                //Make the new house
                db.Households.Add(household);
                db.SaveChanges();

                //Update the user's record
                var user = db.Users.Find(userId);
                user.HouseholdId = household.Id;
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(household);
        }

        // GET: Households/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Leave(int? houseId)
        {
            //Get the users informaiton
            var userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
            var userRole = roleHelper.ListUserRoles(userId).FirstOrDefault();

            //check if they are the head of household or not
            if (userRole == "HeadOfHousehold")
            {
                //if they are head of household, they can only leave if there are no other members
                //count how many users in the house
                var members = db.Users.Where(u => u.HouseholdId == houseId).Count();
                if (members == 1)
                {
                    roleHelper.RemoveUserFromRole(userId, "HeadOfHousehold");
                    user.HouseholdId = null;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    //now we have to soft delete this house because ya know...its empty
                    var household = db.Households.Find(houseId);
                    household.Deleted = true;
                    db.Entry(household).State = EntityState.Modified;

                    db.SaveChanges();
                }
            }
            //if they are not they are free to leave
            else
            {
                user.HouseholdId = null;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
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
