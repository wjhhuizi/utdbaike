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
    public partial class EmailHistory
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public System.DateTime LastSend { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool Confirmed { get; set; }
        public string ToEmail { get; set; }
        public int NewStudentId { get; set; }
    
        public virtual NewStudent NewStudent { get; set; }
    }
    
}