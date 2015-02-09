using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public partial class Candidate
    {
        public string DOB
        {
            get
            {
                if (DateOfBirth != null)
                    return DateOfBirth.ToShortDateString();

                return string.Empty;
            }
        }

        public string CandidateSkill
        {
            get
            {
                if (CandidateSkills != null)
                {
                    string s = string.Empty;
                    foreach (CandidateSkill sk in CandidateSkills)
                    {
                        s += string.Format(" {0} | ", sk.Skill.Name);
                    }
                    return s;
                }

                return string.Empty;
            }
        }
    }
}
