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
    
    public partial class Citation
    {
        public int citation_id { get; set; }
        public string source { get; set; }
        public Nullable<int> count { get; set; }
        public string link { get; set; }
        public int people_id { get; set; }
        public string currnet_mssv_msnv { get; set; }
    
        public virtual Person Person { get; set; }
    }
}
