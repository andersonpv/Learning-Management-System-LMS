using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignments
    {
        public Assignments()
        {
            Submissions = new HashSet<Submissions>();
        }

        public string HwName { get; set; }
        public short? MaxPoints { get; set; }
        public uint HwId { get; set; }
        public string Instructions { get; set; }
        public DateTime? DueDate { get; set; }
        public uint AcId { get; set; }

        public virtual AssignmentCategories Ac { get; set; }
        public virtual ICollection<Submissions> Submissions { get; set; }
    }
}
