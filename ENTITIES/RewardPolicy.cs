//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ENTITIES
{
    using System;
    using System.Collections.Generic;
    
    public partial class RewardPolicy
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RewardPolicy()
        {
            this.ConferenceSupports = new HashSet<ConferenceSupport>();
            this.Criteria = new HashSet<Criterion>();
        }
    
        public int reward_policy_id { get; set; }
        public System.DateTime valid_date { get; set; }
        public Nullable<System.DateTime> expired_date { get; set; }
        public int file_id { get; set; }
    
        public virtual File File { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConferenceSupport> ConferenceSupports { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Criterion> Criteria { get; set; }
    }
}
