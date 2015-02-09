using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Utils
{
    public class Constant
    {
        public static string GetResult(short? id)
        {
            if(id == 1)
            {
                return "Yet to Start";
            }
            else
            {
                return "Completed";
            }
        }

        public static string GetStatus(short? id)
        {
            if (id == 1)
            {
                return "Applied";
            }
            else
            {
                return "Rejected";
            }
        }

        public enum Status
        {
            Applied = 0,
            Rejected = 1
        }
    }
}
