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
    
    public partial class ActivityExpenseDetail
    {
        public double expense_price { get; set; }
        public int expense_quantity { get; set; }
        public string note { get; set; }
        public int expense_type_id { get; set; }
        public int expense_detail_id { get; set; }
        public string evidence { get; set; }
        public Nullable<int> expense_category_id { get; set; }
    
        public virtual ActivityExpenseCategory ActivityExpenseCategory { get; set; }
        public virtual ActivityExpenseType ActivityExpenseType { get; set; }
    }
}
