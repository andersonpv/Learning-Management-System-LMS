using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Students
    {
        public Students()
        {
            Enrollment = new HashSet<Enrollment>();
            Submissions = new HashSet<Submissions>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Dob { get; set; }
        public string Abrev { get; set; }

        public virtual Departments AbrevNavigation { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
