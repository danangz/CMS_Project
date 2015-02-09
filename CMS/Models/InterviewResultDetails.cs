using CMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public partial class InterviewResult
    {
        public string CandidateName
        {
            get
            {
                if (Candidate != null)
                    return Candidate.Name;

                return string.Empty;
            }
        }

        public string CandidatePhone
        {
            get
            {
                if (Candidate != null)
                    return Candidate.Phone;

                return string.Empty;
            }
        }

        public string CandidateEmail
        {
            get
            {
                if (Candidate != null)
                    return Candidate.Email;

                return string.Empty;
            }
        }

        public string RequestName
        {
            get
            {
                if (Request != null)
                    return Request.RequestName;

                return string.Empty;
            }
        }

        public string PositionName
        {
            get
            {
                if (Position != null)
                    return Position.Name;

                return string.Empty;
            }
        }

        public string InterStatus
        {
            get
            {
                if (Status != null)
                    return Constant.GetStatus(Status);

                return string.Empty;
            }
        }

        public string InterResult
        {
            get
            {
                if (Result != null)
                    return Constant.GetResult(Result);

                return string.Empty;
            }
        }
    }
}
