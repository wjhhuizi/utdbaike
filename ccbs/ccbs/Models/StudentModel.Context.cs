﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ccbs.Models
{
    public partial class StudentModelContainer : DbContext
    {
        public StudentModelContainer()
            : base("name=StudentModelContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<NewStudent> NewStudents { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrgRequest> OrgRequests { get; set; }
        public DbSet<LocalHelp> LocalHelps { get; set; }
        public DbSet<GuestParticipant> GuestParticipants { get; set; }
        public DbSet<ManualAssignInfo> ManualAssignInfoes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<OperationRecord> OperationRecords { get; set; }
        public DbSet<TempPool> TempPools { get; set; }
        public DbSet<RegisterEntry> RegisterEntries { get; set; }
        public DbSet<EmailHistory> EmailHistories { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ManualVolunteer> ManualVolunteers { get; set; }
        public DbSet<FacssDepartment> FacssDepartments { get; set; }
        public DbSet<Disclaimer> Disclaimers { get; set; }
    }
}
