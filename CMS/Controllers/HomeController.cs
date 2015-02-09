using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMS.Models;
using System.Web.Helpers;
using System.Data.Entity;
using CMS.Utils;

namespace CMS.Controllers
{
    public class HomeController : Controller
    {
        CMSEntities dbContext = new CMSEntities();
        public ActionResult Index()
        {
            var reqList = dbContext.Requests.ToList();

            List<SelectListItem> reqs = new List<SelectListItem>();

            foreach (Request c in reqList)
            {
                reqs.Add(new SelectListItem { Text = c.RequestName, Value = c.Id.ToString() });
            }

            ViewBag.RequestList = reqs;

            var positionList = dbContext.Positions.ToList();

            List<SelectListItem> post = new List<SelectListItem>();

            foreach (Position p in positionList)
            {
                post.Add(new SelectListItem { Text = p.Name, Value = p.Id.ToString() });
            }

            ViewBag.PositionList = post;

            List<SelectListItem> statusList = new List<SelectListItem>();

            foreach (int value in Enum.GetValues(typeof(Constant.Status)))
            {
                statusList.Add(new SelectListItem
                {
                    Text = Enum.GetName(typeof(Constant.Status), value),
                    Value = value.ToString()
                });
            }

            ViewBag.StatusList = statusList;

            var interviewList = dbContext.InterviewResults.OrderByDescending(p => p.InterviewTime).Take(10).ToList();

            return View(interviewList);
        }

        public JsonResult GetSkill()
        {
            var skillList = dbContext.Skills.ToList();

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

        public JsonResult Search(string requestName, string candidateName, IEnumerable<string> skill, string position, string status)
        {
            if ((!string.IsNullOrEmpty(requestName) || !string.IsNullOrEmpty(candidateName) ||
                !string.IsNullOrEmpty(position) || !string.IsNullOrEmpty(status)) || !string.IsNullOrEmpty(skill.First().ToString()))
            {
                var interviewList = dbContext.InterviewResults;
                List<InterviewResult> interList = new List<InterviewResult>();

                interList = interviewList.ToList();

                if (!string.IsNullOrEmpty(requestName))
                {
                    int requestId = Convert.ToInt32(requestName);
                    interList = interList.Where(x => x.Request.Id == requestId).ToList();
                }
                if (!string.IsNullOrEmpty(candidateName))
                {
                    interList = interList.Where(x => x.Candidate.Name.Contains(candidateName) || x.Candidate.Phone.Contains(candidateName)).ToList();
                }
                // todo: convert
                if (!string.IsNullOrEmpty(skill.First().ToString()))
                {
                    List<int> intSkill = new List<int>();
                    foreach (var sid in skill)
                    {
                        if (!string.IsNullOrEmpty(sid.ToString()))
                        {
                            intSkill.Add(Convert.ToInt32(sid));
                        }
                    }

                    var candidatesSkill = dbContext.CandidateSkills.Where(ski => intSkill.Contains(ski.SkillId)).ToList();

                    intSkill.Clear();
                    foreach (CandidateSkill cs in candidatesSkill)
                    {
                        intSkill.Add(cs.CandidateId);
                    }

                    var candidate = dbContext.Candidates.Where(x => intSkill.Contains(x.Id)).ToList();

                    intSkill.Clear();
                    foreach (Candidate c in candidate)
                    {
                        intSkill.Add(c.Id);
                    }

                    interList = interList.Where(x => intSkill.Contains(x.CandidateId)).ToList();
                }

                if (!string.IsNullOrEmpty(position))
                {
                    int posId = Convert.ToInt32(position);
                    interList = interList.Where(x => x.PositionId == posId).ToList();
                }
                if (!string.IsNullOrEmpty(status))
                {
                    short sttId = Convert.ToInt16(status);
                    interList = interList.Where(x => x.Status == sttId).ToList();
                }

                var result = new
                {
                    rows = (from ent in interList
                            select new
                            {
                                CandidateId = ent.CandidateId,
                                CandidateName = ent.CandidateName,
                                CandidatePhone = !string.IsNullOrEmpty(ent.CandidatePhone) ? ent.CandidatePhone : string.Empty,
                                CandidateEmail = !string.IsNullOrEmpty(ent.CandidateEmail) ? ent.CandidateEmail : string.Empty,
                                InterviewRequest = !string.IsNullOrEmpty(ent.Request.RequestName) ? ent.Request.RequestName : string.Empty,
                                InterviewTime = ent.InterviewTime.Value.ToShortDateString(),
                                Interviewer = ent.Interviewer,
                                Position = !string.IsNullOrEmpty(ent.Position.Name) ? ent.Position.Name : string.Empty
                            }).ToList()
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var interviewList = dbContext.InterviewResults.OrderByDescending(p => p.InterviewTime).Take(10).ToList();

                var result = new
                {
                    rows = (from ent in interviewList
                            select new
                            {
                                CandidateName = ent.CandidateName,
                                CandidatePhone = !string.IsNullOrEmpty(ent.CandidatePhone) ? ent.CandidatePhone : string.Empty,
                                CandidateEmail = !string.IsNullOrEmpty(ent.CandidateEmail) ? ent.CandidateEmail : string.Empty,
                                InterviewRequest = !string.IsNullOrEmpty(ent.Request.RequestName) ? ent.Request.RequestName : string.Empty,
                                InterviewTime = ent.InterviewTime.Value.ToShortDateString(),
                                Interviewer = ent.Interviewer,
                                Position = !string.IsNullOrEmpty(ent.Position.Name) ? ent.Position.Name : string.Empty
                            }).ToList()
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult ViewDetail(string candidateId)
        {
            int canId = Convert.ToInt32(candidateId);
            var interviewResult = dbContext.InterviewResults.Where(x => x.Candidate.Id == canId).ToList();
            var candidateResult = dbContext.Candidates.First(x => x.Id == canId);
            var candidateSkill = dbContext.CandidateSkills.Where(x => x.CandidateId == canId).ToList();
            string sskill = string.Empty;

            foreach (CandidateSkill s in candidateSkill)
            {
                sskill += string.Format(" {0} | ", s.Skill.Name);
            }

            var result = new
            {
                rows = (from ent in interviewResult
                        select new
                        {
                            CandidateName = candidateResult.Name,
                            CandidatePhone = !string.IsNullOrEmpty(candidateResult.Phone) ? candidateResult.Phone : "None",
                            CandidateWorking = !string.IsNullOrEmpty(candidateResult.WorkingPlace) ? candidateResult.WorkingPlace : "None",
                            CandidateDOB = candidateResult.DateOfBirth.ToShortDateString(),
                            CandidateEmail = !string.IsNullOrEmpty(candidateResult.Email) ? candidateResult.Email : "None",
                            CandidateSkill = !string.IsNullOrEmpty(sskill) ? sskill : "None",
                            CandidateExperience = !string.IsNullOrEmpty(candidateResult.Experience) ? candidateResult.Experience : "None",
                            CandidateCertificate = !string.IsNullOrEmpty(candidateResult.Certificate) ? candidateResult.Certificate : "None",
                            InterviewRequest = ent.Request.RequestName,
                            InterviewTime = ent.InterviewTime.Value.ToShortDateString(),
                            Interviewer = ent.Interviewer,
                            Position = !string.IsNullOrEmpty(ent.Position.Name) ? ent.Position.Name : "None",
                            Result = Constant.GetResult(ent.Result),
                            English = !string.IsNullOrEmpty(ent.Candidate.English) ? ent.Candidate.English : "None",
                            Status = Constant.GetStatus(ent.Status),
                            Comment = !string.IsNullOrEmpty(ent.Comment) ? ent.Comment : "None",
                            Description = !string.IsNullOrEmpty(ent.Description) ? ent.Description : "None",
                            Suggestion = !string.IsNullOrEmpty(ent.Suggestion) ? ent.Suggestion : "None"
                        }).ToList()
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
