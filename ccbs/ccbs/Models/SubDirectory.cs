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
    
    public partial class SubDirectory
    {
        public SubDirectory()
        {
            this.SubDirectories = new HashSet<SubDirectory>();
            this.Articles = new HashSet<Article>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public Nullable<int> SubDirectoryId { get; set; }
        public int Number { get; set; }
    
        public virtual ICollection<SubDirectory> SubDirectories { get; set; }
        public virtual SubDirectory TopDirectory { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
    }
}
