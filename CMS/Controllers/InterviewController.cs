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
    public class InterviewController : Controller
    {
        private CMSEntities db = new CMSEntities();

        // GET: /Interview/
        public ActionResult Index()
        {
            var interviewresults = db.InterviewResults.Include(i => i.Candidate).Include(i => i.Position).Include(i => i.Request).OrderByDescending(x => x.InterviewTime);

            return View(interviewresults.ToList());
        }

        // GET: /Interview/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InterviewResult interviewresult = db.InterviewResults.Find(id);
            if (interviewresult == null)
            {
                return HttpNotFound();
            }
            return View(interviewresult);
        }

        // GET: /Interview/Create
        public ActionResult Create()
        {
            ViewBag.CandidateId = new SelectList(db.Candidates.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.PositionId = new SelectList(db.Positions.OrderBy(x => x.Name), "Id", "Name");
            ViewBag.RequestId = new SelectList(db.Requests.OrderBy(x => x.RequestName), "Id", "RequestName");
            return View();
        }

        // POST: /Interview/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,CandidateId,RequestId,Status,InterviewTime,Interviewer,PositionId,Description,Comment,Result,Suggestion")] InterviewResult interviewresult)
        {
            if (ModelState.IsValid)
            {
                db.InterviewResults.Add(interviewresult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CandidateId = new SelectList(db.Candidates, "Id", "Name", interviewresult.CandidateId);
            ViewBag.PositionId = new SelectList(db.Positions, "Id", "Name", interviewresult.PositionId);
            ViewBag.RequestId = new SelectList(db.Requests, "Id", "RequestName", interviewresult.RequestId);
            return View(interviewresult);
        }

        // GET: /Interview/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InterviewResult interviewresult = db.InterviewResults.Find(id);
            if (interviewresult == null)
            {
                return HttpNotFound();
            }
            ViewBag.CandidateId = new SelectList(db.Candidates.OrderBy(x => x.Name), "Id", "Name", interviewresult.CandidateId);
            ViewBag.PositionId = new SelectList(db.Positions.OrderBy(x => x.Name), "Id", "Name", interviewresult.PositionId);
            ViewBag.RequestId = new SelectList(db.Requests.OrderBy(x => x.RequestName), "Id", "RequestName", interviewresult.RequestId);
            return View(interviewresult);
        }

        // POST: /Interview/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,CandidateId,RequestId,Status,InterviewTime,Interviewer,PositionId,Description,Comment,Result,Suggestion")] InterviewResult interviewresult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(interviewresult).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CandidateId = new SelectList(db.Candidates, "Id", "Name", interviewresult.CandidateId);
            ViewBag.PositionId = new SelectList(db.Positions, "Id", "Name", interviewresult.PositionId);
            ViewBag.RequestId = new SelectList(db.Requests, "Id", "RequestName", interviewresult.RequestId);
            return View(interviewresult);
        }

        // GET: /Interview/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InterviewResult interviewresult = db.InterviewResults.Find(id);
            if (interviewresult == null)
            {
                return HttpNotFound();
            }
            return View(interviewresult);
        }

        // POST: /Interview/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InterviewResult interviewresult = db.InterviewResults.Find(id);
            db.InterviewResults.Remove(interviewresult);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult DeleteInterview(string interviewIds)
        {
            if ((interviewIds + "").Equals(""))
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            String[] deleteIds = interviewIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string interviewId in deleteIds)
            {
                int deleteId = Convert.ToInt32(interviewId);
                InterviewResult re = db.InterviewResults.Single(r => r.Id == deleteId);
                if (re == null)
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                db.InterviewResults.Remove(re);
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
