using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMS.Models;
using System.IO;

namespace CMS.Controllers
{
    public class CandidatesController : Controller
    {
        private CMSEntities db = new CMSEntities();

        // GET: Candidates
        public ActionResult Index()
        {
            return View(db.Candidates.ToList());
        }
        [HttpGet]
        public JsonResult SearchCandidate(string searchString)
        {
            searchString = Utils.Utils.ConvertToUnSign(searchString).ToLower();
            List<Candidate> result = new List<Candidate>();
            foreach (Candidate re in db.Candidates)
                if (Utils.Utils.ConvertToUnSign(re.Name + "").ToLower().Contains(searchString) || Utils.Utils.ConvertToUnSign(re.Email + "").ToLower().Contains(searchString) || Utils.Utils.ConvertToUnSign(re.Phone + "").ToLower().Contains(searchString))
                    result.Add(re);
            var searchResult = new
            {
                rows = (from cou in result
                        select new
                        {
                            Id = cou.Id,
                            CandidateName = cou.Name,
                            Email = cou.Email
                        }).ToList()
            };
            return Json(searchResult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ContentResult UploadFiles()
        {
            var r = new List<UploadFilesResult>();

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf.ContentLength == 0)
                    continue;
                string savedFileName = Path.GetFileName(hpf.FileName);
                savedFileName = savedFileName.Substring(0, savedFileName.LastIndexOf(".")) + DateTime.Now.Ticks + "." + savedFileName.Substring(savedFileName.LastIndexOf(".") + 1, savedFileName.Length - savedFileName.LastIndexOf(".") - 1);
                savedFileName = savedFileName.Replace(" ", "_");
                string fullSavedFileName = Path.Combine(Server.MapPath("~/Upload"), savedFileName);
                hpf.SaveAs(fullSavedFileName); // Save the file

                r.Add(new UploadFilesResult()
                {
                    Name = savedFileName,
                    Length = hpf.ContentLength,
                    Type = hpf.ContentType
                });
            }
            // Returns json
            return Content("{\"name\":\"" + r[0].Name + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");
        }
        // GET: Candidates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = db.Candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // GET: Candidates/Create
        public ActionResult Create()
        {
            var reqList = db.Skills.ToList();

            List<SelectListItem> reqs = new List<SelectListItem>();

            foreach (Skill c in reqList)
            {
                reqs.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }

            ViewBag.SkillList = reqs;

            return View();
        }

        // POST: Candidates/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Name, string Certificate, DateTime DateOfBirth, string Email, string WorkingPlace, string Experience,
            string Phone, string English, string LinkCV, IEnumerable<int> Skills)
        {
            Candidate candidate = new Candidate()
            {
                Name = Name,
                Certificate = Certificate,
                DateOfBirth = DateOfBirth,
                Email = Email,
                WorkingPlace = WorkingPlace,
                Experience = Experience,
                Phone = Phone,
                English = English,
                LinkCV = LinkCV
            };

            if (ModelState.IsValid)
            {
                db.Candidates.Add(candidate);
                db.SaveChanges();

                var removelist = db.CandidateSkills.Where(x => x.CandidateId == candidate.Id);
                if (removelist != null)
                {
                    foreach (CandidateSkill cs in removelist)
                    {
                        db.CandidateSkills.Remove(db.CandidateSkills.FirstOrDefault(x => x.Id == cs.Id));
                    }
                }
                if (Skills != null && !string.IsNullOrEmpty(Skills.First().ToString()))
                {
                    foreach (int i in Skills)
                    {
                        db.CandidateSkills.Add(new CandidateSkill() { CandidateId = candidate.Id, SkillId = i });
                    }
                }

                return RedirectToAction("Index");
            }

            return View(candidate);
        }

        // GET: Candidates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Candidate candidate = db.Candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            var reqList = db.Skills.ToList();
            List<SelectListItem> reqs = new List<SelectListItem>();
            foreach (Skill c in reqList)
            {
                reqs.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }

            ViewBag.SkillList = reqs;

            return View(candidate);
        }

        // POST: Candidates/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string Id, string Name, string Certificate, DateTime DateOfBirth, string Email, string WorkingPlace, string Experience,
            string Phone, string English, string LinkCV, IEnumerable<int> skills)
        {
            int canId = Convert.ToInt32(Id);
            Candidate candidate = new Candidate()
            {
                Id = canId,
                Name = Name,
                Certificate = Certificate,
                DateOfBirth = DateOfBirth,
                Email = Email,
                WorkingPlace = WorkingPlace,
                Experience = Experience,
                Phone = Phone,
                English = English,
                LinkCV = LinkCV
            };

            if (ModelState.IsValid)
            {
                db.Entry(candidate).State = EntityState.Modified;

                var removelist = db.CandidateSkills.Where(x => x.CandidateId == canId);
                if(removelist != null)
                {
                foreach(CandidateSkill cs in removelist)
                {
                    db.CandidateSkills.Remove(db.CandidateSkills.FirstOrDefault(x => x.Id == cs.Id));
                }
                }
                if(skills != null && !string.IsNullOrEmpty(skills.First().ToString()))
                {
                    foreach(int i in skills)
                    {
                        db.CandidateSkills.Add(new CandidateSkill() { CandidateId = canId, SkillId = i });
                    }
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(candidate);
        }

        // GET: Candidates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = db.Candidates.Find(id);
            if (candidate == null)
            {
                return HttpNotFound();
            }
            return View(candidate);
        }

        // POST: Candidates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Candidate candidate = db.Candidates.Find(id);
            db.Candidates.Remove(candidate);
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

        public JsonResult GetSkill(int canId)
        {
            var skillList = db.Skills.ToList();
            var candidateSkill = db.CandidateSkills.Where(x => x.CandidateId == canId);

            var result = new
            {
                rows = (from sk in skillList
                        select new
                        {
                            SkillId = sk.Id,
                            SkillActive = candidateSkill.FirstOrDefault(x=>x.SkillId ==sk.Id) != null ? "Active" : "Inactive",
                            SkillName = sk.Name
                        }).ToList()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSkillInCreate()
        {
            var skillList = db.Skills;

            var result = new
            {
                rows = (from sk in skillList
                        select new
                        {
                            SkillId = sk.Id,
                            SkillName = sk.Name
                        }).ToList()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteCandidates(string canIds)
        {
            if ((canIds + "").Equals(""))
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            String[] deleteIds = canIds.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string requestId in deleteIds)
            {
                int deleteId = Convert.ToInt32(requestId);
                Candidate re = db.Candidates.Single(r => r.Id == deleteId);
                if (re == null)
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                db.Candidates.Remove(re);
            }
            try
            {
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
