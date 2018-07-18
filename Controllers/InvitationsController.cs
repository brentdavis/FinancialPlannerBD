using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FinancialPlannerBD.Helpers;
using FinancialPlannerBD.Models;
using Microsoft.AspNet.Identity;

namespace FinancialPlannerBD.Controllers
{
    public class InvitationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();

        // GET: Invitations
        public ActionResult Index()
        {
            return View(db.Invitations.ToList());
        }

        // GET: Invitations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // GET: Invitations/Create
        public ActionResult Create(int houseId)
        {
            ViewBag.HouseholdId = houseId;
            return View();
        }

        // POST: Invitations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "HouseholdId,Created,Email,Body,Code,Accepted")] Invitation invitation)
        {

            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (ModelState.IsValid)
            {
                //Make invitation, and use the guid to generate us a unique code
                invitation.Created = DateTime.Now;
                invitation.Code = Guid.NewGuid().ToString();
                var callbackUrl = Url.Action("Join", "Invitations", new { userId = user.Id }, protocol: Request.Url.Scheme);
                invitation.Body = "Enter this code into the Join Household screen to join by clicking <a href=\"" + callbackUrl + "\">here!</a>" + invitation.Code;

                try
                {
                    IdentityMessage email = null;
                    email = new IdentityMessage()
                    {
                        Subject = "You have been invited to a household!",
                        Body = invitation.Body,
                        Destination = invitation.Email
                    };

                    var svc = new EmailService();
                    await svc.SendAsync(email);
                }
                catch (Exception e)
                {

                }

                db.Invitations.Add(invitation);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.HouseholdId = invitation.HouseholdId;

                //makes the validation summary work
                var message = string.Join(" | ", ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage));
                ModelState.AddModelError("", message);
                return View(invitation);
            }
        }

        //get some fools in the house
        [Authorize]
        public ActionResult Join()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Join(string email, string code)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var invite = db.Invitations.FirstOrDefault(i => i.Email == email && i.Code == code);
            if (invite != null && user.Email == invite.Email)
            {
                invite.Accepted = true;
                user.HouseholdId = invite.HouseholdId;
                db.SaveChanges();

                rolesHelper.AddUserToRole(userId, "Member");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Invitations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Created,Email,Body,Code,Accepted")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(invitation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invitation invitation = db.Invitations.Find(id);
            if (invitation == null)
            {
                return HttpNotFound();
            }
            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Invitation invitation = db.Invitations.Find(id);
            db.Invitations.Remove(invitation);
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
