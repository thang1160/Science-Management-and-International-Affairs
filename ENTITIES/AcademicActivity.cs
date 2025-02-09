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
    
    public partial class AcademicActivity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AcademicActivity()
        {
            this.AcademicActivityPhases = new HashSet<AcademicActivityPhase>();
            this.AcademicActivityLanguages = new HashSet<AcademicActivityLanguage>();
            this.ActivityOffices = new HashSet<ActivityOffice>();
            this.ActivityInfoes = new HashSet<ActivityInfo>();
            this.ActivityPartners = new HashSet<ActivityPartner>();
        }
    
        public int activity_id { get; set; }
        public Nullable<System.DateTime> activity_date_start { get; set; }
        public Nullable<System.DateTime> activity_date_end { get; set; }
        public int activity_type_id { get; set; }
        public int activity_status_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AcademicActivityPhase> AcademicActivityPhases { get; set; }
        public virtual AcademicActivityType AcademicActivityType { get; set; }
        public virtual AcademicActivityStatu AcademicActivityStatu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AcademicActivityLanguage> AcademicActivityLanguages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityOffice> ActivityOffices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityInfo> ActivityInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityPartner> ActivityPartners { get; set; }
    }
}
