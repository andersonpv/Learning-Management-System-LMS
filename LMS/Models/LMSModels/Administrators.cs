using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Administrators
    {
        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Dob { get; set; }
    }
}
