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
    
    public partial class News
    {
        public News()
        {
            this.Hidden = false;
            this.Paragraphs = new HashSet<Paragraph>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public string Description { get; set; }
        public bool Hidden { get; set; }
    
        public virtual ICollection<Paragraph> Paragraphs { get; set; }
    }
}
