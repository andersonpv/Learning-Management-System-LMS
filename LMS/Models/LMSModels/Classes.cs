using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enrollment = new HashSet<Enrollment>();
        }

        public TimeSpan? Start { get; set; }
        public TimeSpan? Stop { get; set; }
        public string Location { get; set; }
        public string CatalogId { get; set; }
        public string Season{ get; set; }
        public uint? Year { get; set; }
        public uint ClassId { get; set; }
        public string ProfId { get; set; }

        public virtual Courses Catalog { get; set; }
        public virtual Professors Prof { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enrollment> Enrollment { get; set; }

        //public enum Seasons
        //{
        //    Spring = 0,
        //    Summer = 1,
        //    Fall = 2
        //}
    }
}
