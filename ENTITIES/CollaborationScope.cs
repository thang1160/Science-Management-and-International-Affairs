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
    
    public partial class CollaborationScope
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CollaborationScope()
        {
            this.PartnerScopes = new HashSet<PartnerScope>();
        }
    
        public int scope_id { get; set; }
        public string scope_abbreviation { get; set; }
        public string scope_name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PartnerScope> PartnerScopes { get; set; }
    }
}
