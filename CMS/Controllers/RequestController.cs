using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMS.Models;

namespace CMS.Controllers
{
    public class RequestController : Controller
    {
        private CMSEntities db = new CMSEntities();

        // GET: /Request/
        public ActionResult Index()
        {
            return View(db.Requests.ToList());
        }
        public ActionResult SearchResult(string searchString)
        {
            searchString = Utils.Utils.ConvertToUnSign(searchString).ToLower();
            List<Request> result = new List<Request>();
            foreach (Request re in db.Requests)
                if (Utils.Utils.ConvertToUnSign(re.RequestName).ToLower().Contains(searchString))
                    result.Add(re);
            return View("Index",result);
        }
        [HttpGet]
        public JsonResult SearchRequest(string searchString)
        {
            searchString = Utils.Utils.ConvertToUnSign(searchString).ToLower();
            List<Request> result = new List<Request>();
            foreach (Request re in db.Requests)
                if (Utils.Utils.ConvertToUnSign(re.RequestName + "").ToLower().Contains(searchString))
                    result.Add(re);
            var searchResult = new
            {
                rows = (from cou in result
                        select new
                        {
                            Id = cou.Id,
                            RequestName = cou.RequestName,
                            Description = cou.Description
                        }).ToList()
            };
            return Json(searchResult, JsonRequestBehavior.AllowGet);
        }
        // GET: /Request/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: /Request/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Request/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Create([Bind(Include="Id,RequestName,Description")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Requests.Add(request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(request);
        }

        // GET: /Request/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: /Request/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Edit([Bind(Include="Id,RequestName,Description")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        // GET: /Request/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: /Request/Delete/5
        [HttpPost, ActionName("Delete")]

        public ActionResult DeleteConfirmed(int id)
        {
            Request request = db.Requests.Find(id);
            db.Requests.Remove(request);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult DeleteRequest(string requestIds)
        {
            if ((requestIds+"").Equals(""))
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            String[] deleteIds = requestIds.Split(new char[]{'|'},StringSplitOptions.RemoveEmptyEntries);
            foreach (string requestId in deleteIds)
            {
                int deleteId = Convert.ToInt32(requestId);
                Request re = db.Requests.Single(r => r.Id == deleteId);
                if (re == null)
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                db.Requests.Remove(re);
            }
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
