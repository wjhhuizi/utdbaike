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
    
    public partial class Question
    {
        public Question()
        {
            this.Answers = new HashSet<Answer>();
        }
    
        public int Id { get; set; }
        public string UserName { get; set; }
        public string MainContent { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public string Title { get; set; }
        public int LikeCount { get; set; }
        public int VisitCount { get; set; }
    
        public virtual ICollection<Answer> Answers { get; set; }
    }
}
