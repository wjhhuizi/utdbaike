//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ccbs.Models
{
    public partial class OrgRequest
    {
        public OrgRequest()
        {
            this.Note = "none";
        }
    
        public int Id { get; set; }
        public int NumOfNews { get; set; }
        public string Note { get; set; }
        public string Reply { get; set; }
        public string Progress { get; set; }
        public Nullable<System.DateTime> RequestDate { get; set; }
    
        public virtual Organization Organization { get; set; }
    }
    
}