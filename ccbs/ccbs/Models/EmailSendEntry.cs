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
    
    public partial class EmailSendEntry
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsSucceed { get; set; }
        public int RetryCount { get; set; }
        public int ExceptionType { get; set; }
    
        public virtual EmailRecord EmailRecord { get; set; }
    }
}
