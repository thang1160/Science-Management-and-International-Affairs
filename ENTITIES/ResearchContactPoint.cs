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
    
    public partial class ResearchContactPoint
    {
        public string contact_point_name { get; set; }
        public string contact_point_phone { get; set; }
        public string contact_point_email { get; set; }
        public int research_partner_id { get; set; }
    
        public virtual ResearchPartner ResearchPartner { get; set; }
    }
}
