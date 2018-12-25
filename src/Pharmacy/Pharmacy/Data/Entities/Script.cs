using System.Collections.Generic;

namespace Pharmacy.Data.Entities
{
    public class Script : Entity
    {
        public Script()
        {
            ScriptDetails = new HashSet<ScriptDetail>();
        }

        public virtual Patient Patient { get; set; }
        public int PatientId { get; set; }
        public string Request { get; set; }
        public virtual Doctor Doctor { get; set; }
        public int? DoctorId { get; set; }
        public string Response { get; set; }
        public ScriptStatuses Status { get; set; }

        public virtual Order Order { get; set; }
        public virtual ICollection<ScriptDetail> ScriptDetails { get; set; }
    }
}
