using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public string CName { get; set; }
        public string Number { get; set; }
        public string CatalogId { get; set; }
        public string Abrev { get; set; }

        public virtual Departments AbrevNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
