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
    
    public partial class Award
    {
        public int award_id { get; set; }
        public string competion_name { get; set; }
        public Nullable<System.DateTime> award_time { get; set; }
        public string rank { get; set; }
        public int people_id { get; set; }
    
        public virtual Person Person { get; set; }
    }
}
