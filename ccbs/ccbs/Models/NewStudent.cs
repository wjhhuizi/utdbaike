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
    public partial class NewStudent
    {
        public NewStudent()
        {
            this.ComeFrom = "Unknow";
            this.CnName = "Unknow";
            this.HasApt = false;
            this.WillingToHelp = false;
            this.Marked = false;
            this.IsManualAssigned = false;
            this.ManualAssignInfoes = new HashSet<ManualAssignInfo>();
            this.RegisterEntries = new HashSet<RegisterEntry>();
            this.EmailHistories = new HashSet<EmailHistory>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Major { get; set; }
        public int Year { get; set; }
        public string Term { get; set; }
        public System.DateTime ArrivalTime { get; set; }
        public string Flight { get; set; }
        public bool NeedPickup { get; set; }
        public bool NeedTempHousing { get; set; }
        public string Note { get; set; }
        public string EntryPort { get; set; }
        public string ComeFrom { get; set; }
        public string CnName { get; set; }
        public System.DateTime RegTime { get; set; }
        public bool HasApt { get; set; }
        public string WhenApt { get; set; }
        public string WhereApt { get; set; }
        public bool WillingToHelp { get; set; }
        public string HelpNote { get; set; }
        public bool Marked { get; set; }
        public int DaysOfHousing { get; set; }
        public int Confirmed { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public bool IsManualAssigned { get; set; }
        public string ManualAssignedPickup { get; set; }
        public string ManualAssignedHost { get; set; }
        public string UserName { get; set; }
    
        public virtual Volunteer PickupVolunteer { get; set; }
        public virtual Volunteer TempHouseVolunteer { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<ManualAssignInfo> ManualAssignInfoes { get; set; }
        public virtual Group Group { get; set; }
        public virtual TempPool TempPool { get; set; }
        public virtual ICollection<RegisterEntry> RegisterEntries { get; set; }
        public virtual ICollection<EmailHistory> EmailHistories { get; set; }
        public virtual FacssDepartment ApplyFacssDepartment { get; set; }
    }
    
}
