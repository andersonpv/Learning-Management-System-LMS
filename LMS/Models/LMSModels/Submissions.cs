using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submissions
    {
        public uint HwId { get; set; }
        public string UId { get; set; }
        public DateTime STime { get; set; }
        public ushort? Score { get; set; }
        public string Contents { get; set; }

        public virtual Assignments Hw { get; set; }
        public virtual Students U { get; set; }
    }
}
