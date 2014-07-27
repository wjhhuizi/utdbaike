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
    public partial class Volunteer
    {
        public Volunteer()
        {
            this.Phone = "Not Provided";
            this.BriefIntro = "Not Provided";
            this.Note = "none";
            this.Address = "Not Provided";
            this.PickupNewStudents = new HashSet<NewStudent>();
            this.TempHouseNewStudents = new HashSet<NewStudent>();
            this.LocalHelps = new HashSet<LocalHelp>();
            this.PickedUpStudents = new HashSet<Student>();
            this.HostedStudents = new HashSet<Student>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BriefIntro { get; set; }
        public string Note { get; set; }
        public string Address { get; set; }
        public int OrganizationId { get; set; }
        public System.DateTime RegTime { get; set; }
        public string UserName { get; set; }
        public string HelpType { get; set; }
        public string RelationToUTD { get; set; }
        public Nullable<int> GroupId { get; set; }
    
        public virtual ICollection<NewStudent> PickupNewStudents { get; set; }
        public virtual ICollection<NewStudent> TempHouseNewStudents { get; set; }
        public virtual Organization AdminOrg { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Group AdminGroup { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<LocalHelp> LocalHelps { get; set; }
        public virtual ICollection<Student> PickedUpStudents { get; set; }
        public virtual ICollection<Student> HostedStudents { get; set; }
    }
    
}
