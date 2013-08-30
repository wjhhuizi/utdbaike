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
    public partial class LocalHelp
    {
        public LocalHelp()
        {
            this.RegisterEntries = new HashSet<RegisterEntry>();
            this.Volunteers = new HashSet<Volunteer>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Restriction { get; set; }
        public Nullable<int> OrganizationId { get; set; }
    
        public virtual ICollection<RegisterEntry> RegisterEntries { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<Volunteer> Volunteers { get; set; }
    }
    
}
