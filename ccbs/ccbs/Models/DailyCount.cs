//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ccbs.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DailyCount
    {
        public int Id { get; set; }
        public System.DateTime WhichDate { get; set; }
        public int Count { get; set; }
        public int EmailAccountId { get; set; }
    
        public virtual EmailAccount EmailAccount { get; set; }
    }
}
