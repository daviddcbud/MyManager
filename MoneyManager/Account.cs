//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MoneyManager
{
    using System;
    using System.Collections.Generic;
    
    public partial class Account
    {
        public Account()
        {
            this.EntryDetails = new HashSet<EntryDetail>();
        }
    
        public int AccountID { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> Goal { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
        public virtual ICollection<EntryDetail> EntryDetails { get; set; }
    }
}
