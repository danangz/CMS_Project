using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMS.Models
{
    public class CandidateNewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please Enter Name")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please Enter DateOfBirth")]
        [Display(Name = "DateOfBirth")]
        public System.DateTime DateOfBirth { get; set; }

        public string WorkingPlace { get; set; }
        public string Experience { get; set; }
        public string Certificate { get; set; }

        [Display(Name = "Mobile")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please Enter Correct Email Address")]
        public string Email { get; set; }
        public string LinkCV { get; set; }
        public string English { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }
    }
}
