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
    
    public partial class GalleryItem
    {
        public int Id { get; set; }
        public string Picture { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string HyperLink { get; set; }
        public int Order { get; set; }
        public int SectionId { get; set; }
    
        public virtual Section Section { get; set; }
    }
}
