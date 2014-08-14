using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using scratchpad;

namespace scratchpad.Controllers
{
    using Models;
    using EntityState = System.Data.EntityState;

    public class TestUsersController : Controller
    {
        private readonly TestContext _db = new TestContext();

        // GET: TestUsers
        public ActionResult Index()
        {
            return View(_db.Users.ToList());
        }

        // GET: TestUsers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestUser testUser = _db.Users.Find(id);
            if (testUser == null)
            {
                return HttpNotFound();
            }
            return View(testUser);
        }

        // GET: TestUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TestUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TestUserId,Username,RandomNumber")] TestUser testUser)
        {
            if (ModelState.IsValid)
            {
                _db.Users.Add(testUser);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(testUser);
        }

        // GET: TestUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestUser testUser = _db.Users.Find(id);
            if (testUser == null)
            {
                return HttpNotFound();
            }
            return View(testUser);
        }

        // POST: TestUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TestUserId,Username,RandomNumber")] TestUser testUser)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(testUser).State = (System.Data.Entity.EntityState) EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(testUser);
        }

        // GET: TestUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestUser testUser = _db.Users.Find(id);
            if (testUser == null)
            {
                return HttpNotFound();
            }
            return View(testUser);
        }

        // POST: TestUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestUser testUser = _db.Users.Find(id);
            _db.Users.Remove(testUser);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
