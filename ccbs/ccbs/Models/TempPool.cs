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
    public partial class TempPool
    {
        public TempPool()
        {
            this.NewStudents = new HashSet<NewStudent>();
        }
    
        public int Id { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public string UserName { get; set; }
    
        public virtual ICollection<NewStudent> NewStudents { get; set; }
    }
    
}
