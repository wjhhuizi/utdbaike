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
    public partial class ManualAssignInfo
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string VolName { get; set; }
        public int VolGender { get; set; }
        public string VolEmail { get; set; }
        public string VolPhone { get; set; }
        public string VolAddr { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public int NewStudentId { get; set; }
    
        public virtual NewStudent NewStudent { get; set; }
    }
    
}