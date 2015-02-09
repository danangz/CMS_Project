using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public class HomeModel
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string RequestName { get; set; }
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string InterviewTime { get; set; }
        public string Interviewer { get; set; }
        public string Position { get; set; }
        public string Status { get; set; }
    }
}
