using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace ccbs.Models
{
    public static class LWSFRoles
    {
        public const string admin = "Administrator";
        public const string newStudentAdmin = "NewStudentAdmin";
        public const string localhelpAdmin = "LocalhelpAdmin";
        public const string bangbangAdmin = "BangbangAdmin";
        public const string organizationLeader = "OrganizationLeader";
        public const string groupLeader = "GroupLeader";
        public const string newStudent = "NewStudent";
        public const string volunteer = "Volunteer";
        public const string coworker = "Coworker";
        public const string worshipManager = "WorshipManager";
        public const string bibleStudyManager = "BibleStudyManager";
        public const string emailAdmin = "EmailAdmin";

        public const string BBUser = "BBUser";
    }

    public static class SystemNoticeType
    {
        public const int ToAllNotice = 1;
        public const int VolunteerNotice = 2;
        public const int NewStudentNotice = 3;
        public const int BBUserNotice = 4;

        public static int SetBit(int type, int pos)
        {
            type |= (0x0001 << (pos));
            return type;
        }

        public static int ClearBit(int type, int pos)
        {
            type &= ~(0x0001 << (pos));
            return type;
        }

        public static bool GetBit(int type, int pos)
        {
            bool output = (type & (0x0001 << (pos))) != 0;
            return output;
        }
    }

    public static class EmailType
    {
        public const int StudentRegistration = 1;
        public const int StudentInfoConfirmation = 2;
        public const int ArriveNoticeToStud = 3;
        public const int ArriveNoticeToPickup = 4;
        public const int ArriveNoticeToHost = 5;

        public const int FinalArriveNoticeToStud = 6;
        public const int FinalArriveNoticeToPickup = 7;
        public const int FinalArriveNoticeToHost = 8;
    }

    public static class LocalHelpRestriction
    {
        public const int OPEN_TO_PUBLIC = 0;
        public const int INSIDE_ORG_ONLY = 1;

        public const string StringOpenPublic = "Open to public";
        public const string StringInsideOrg = "Inside my organization";

        public static string ToString(int r)
        {
            if (r == OPEN_TO_PUBLIC)
            {
                return StringOpenPublic;
            }
            if (r == INSIDE_ORG_ONLY)
            {
                return StringInsideOrg;
            }
            return StringInsideOrg;
        }
    }

    public static class NewStudentOperation
    {
        //operation  arg1  arg2  Data
        public const int ASSIGN_TO_ORG = 1; // assign orgId null newstudentlist
        public const int RETURN_FROM_ORG = 2;  //return orgId null newstudentlist
        public const int ASSIGN_TO_GRP = 3; //assign grpId null newstudentlist
        public const int RETURN_FROM_GRP = 4;//  return grpId null newstudentlist

        public static OperationRecord AssignToOrgRecord(int orgId, string data, string description)
        {
            var record = new OperationRecord
            {
                Type = ASSIGN_TO_ORG,
                Arg1 = orgId,
                Data = data,
                Link = "",
                Description = description,
                CreatedDate = DateTime.Now,
            };
            return record;
        }

        public static OperationRecord ReturnFromOrgRecord(int orgId, string data, string description)
        {
            var record = new OperationRecord
            {
                Type = RETURN_FROM_ORG,
                Arg1 = orgId,
                Data = data,
                Link = "",
                Description = description,
                CreatedDate = DateTime.Now,
            };
            return record;
        }

        public static OperationRecord AssignToGrpRecord(int grpId, string data, string description)
        {
            var record = new OperationRecord
            {
                Type = ASSIGN_TO_GRP,
                Arg1 = grpId,
                Data = data,
                Link = "",
                Description = description,
                CreatedDate = DateTime.Now,
            };
            return record;
        }

        public static OperationRecord ReturnFromGrpRecord(int grpId, string data, string description)
        {
            var record = new OperationRecord
            {
                Type = RETURN_FROM_GRP,
                Arg1 = grpId,
                Data = data,
                Link = "",
                Description = description,
                CreatedDate = DateTime.Now,
            };
            return record;
        }
    }

    public static class OrgModelType
    {
        public const int IntGroupless = 0;
        public const int IntGrouped = 1;

        public const string StringGroupless = "统一模式";
        public const string StringGrouped = "分组模式";

        public static string ToString(int m)
        {
            string model = StringGroupless;
            if (m == IntGroupless)
            {
                model = StringGroupless;
            }
            else if (m == IntGrouped)
            {
                model = StringGrouped;
            }
            return model;
        }
    }

    public static class SystemGender
    {
        /***Never Change this definition, otherwise gender will reverse***/
        public const int singleFemale = 0;
        public const int singleMale = 1;
        public const int Family = 2;

        /***Change this for display***/
        public const String singleFemaleString = "Female";
        public const String singleMaleString = "Male";
        public const String FamilyString = "Family";

        public static int ToIntGender(String gender)
        {
            int g = singleFemale;
            if (gender == singleFemaleString)
            {
                g = singleFemale;
            }
            else if (gender == singleMaleString)
            {
                g = singleMale;
            }
            else if (gender == FamilyString)
            {
                g = Family;
            }
            return g;
        }

        public static String ToStringGender(int g)
        {
            String gender = singleFemaleString;
            if (g == singleFemale)
            {
                gender = singleFemaleString;
            }
            else if (g == singleMale)
            {
                gender = singleMaleString;
            }
            else if (g == Family)
            {
                gender = FamilyString;
            }
            return gender;
        }
    }

    public static class ConfirmedStage
    {
        //16 bit int    1111   1111    1111 1111
        //            |email| |reply| |->state<-|
        //|reply| = X-housing, X-pickup, X-student, X-confirmation email sent
        //|email| = 


        public const int UnConfirmed = 0;   //no confirmed
        public const int InfoConfirmed = 1;  //information confirmed
        public const int NeedConfirmed = 2;  //all needs are satisfied
        public const int AllConfirmed = 3;   //three parties confirmed, all done

        public const String StringUnConfirmed = "信息未确认";   //no confirmed
        public const String StringInfoConfirmed = "信息已确认";  //information confirmed
        public const String StringNeedConfirmed = "等待三方确认";  //all needs are satisfied
        public const String StringAllConfirmed = "三方已确认";   //three parties confirmed, all done

        public static int ToInt(String confirm)
        {
            int c = UnConfirmed;
            if (confirm == StringUnConfirmed)
            {
                c = UnConfirmed;
            }
            else if (confirm == StringInfoConfirmed)
            {
                c = InfoConfirmed;
            }
            else if (confirm == StringNeedConfirmed)
            {
                c = NeedConfirmed;
            }
            else if (confirm == StringAllConfirmed)
            {
                c = AllConfirmed;
            }
            return c;
        }

        public static String ToString(int con)
        {
            int c = con & 0xff;
            String confirm = StringUnConfirmed;
            if (c == UnConfirmed)
            {
                confirm = StringUnConfirmed;
            }
            else if (c == InfoConfirmed)
            {
                confirm = StringInfoConfirmed;
            }
            else if (c == NeedConfirmed)
            {
                confirm = StringNeedConfirmed;
            }
            else if (c == AllConfirmed)
            {
                confirm = StringAllConfirmed;
            }
            return confirm;
        }

        /*public static int OnConfirmationEmailSent(int con)
        {
            int c;
            c = con | (0x00000001 << 8);
            return c;
        }

        public static int OnStudentReply(int con)
        {
            int c;
            c = con | (0x00000001 << 9);
            return c;
        }

        public static int OnPickupHostReply(int con)
        {
            int c;
            c = con | (0x00000001 << 10);
            return c;
        }

        public static int OnHousingHostReply(int con)
        {
            int c;
            c = con | (0x00000001 << 11);
            return c;
        }

        public static bool IsConfirmationEmailSent(int c)
        {
            var bit = (c & (0x00000001 << 8)) != 0;
            return bit;
        }

        public static bool IsStudentReplied(int c)
        {
            var bit = (c & (0x00000001 << 9)) != 0;
            return bit;
        }

        public static bool IsPickupHostReplied(int c)
        {
            var bit = (c & (0x00000001 << 10)) != 0;
            return bit;
        }

        public static bool IsHousingHostReplied(int c)
        {
            var bit = (c & (0x00000001 << 11)) != 0;
            return bit;
        }

        public static bool IsAllConfirmed(int c)
        {
            var bit = IsConfirmationEmailSent(c) && IsStudentReplied(c) && IsPickupHostReplied(c) && IsHousingHostReplied(c);
            return bit;
        }

        public static int SetAllConfirmed(int c)
        {
            c = c & (0xff00);
            c = c | AllConfirmed;
            return c;
        }*/
    }

    public static class TempHouseLength
    {
        public const int Under3 = 0;
        public const int From3To7 = 1;
        public const int From7To10 = 2;
        public const int Over10 = 3;
        public const int NotSure = 4;

        public const string StringUnder3 = "< 3 days";
        public const string StringFrom3To7 = "3~7 days";
        public const string StringFrom7To10 = "7~10 days";
        public const string StringOver10 = "over 10 days";
        public const string StringNotSure = "not sure";

        public static int ToInt(String length)
        {
            int c = NotSure;
            if (length == StringUnder3)
            {
                c = Under3;
            }
            else if (length == StringFrom3To7)
            {
                c = From3To7;
            }
            else if (length == StringFrom7To10)
            {
                c = From7To10;
            }
            else if (length == StringOver10)
            {
                c = Over10;
            }
            else if (length == StringNotSure)
            {
                c = NotSure;
            }
            return c;
        }

        public static String ToString(int c)
        {
            String length = StringNotSure;
            if (c == Under3)
            {
                length = StringUnder3;
            }
            else if (c == From3To7)
            {
                length = StringFrom3To7;
            }
            else if (c == From7To10)
            {
                length = StringFrom7To10;
            }
            else if (c == Over10)
            {
                length = StringOver10;
            }
            else if (c == NotSure)
            {
                length = StringNotSure;
            }
            return length;
        }
    }

    public static class VolunteerHelpType
    {
        /***Never Change this definition, otherwise gender will reverse***/
        public const int IntPickup = 1;
        public const int IntHousing = 2;
        public const int IntLocalHelp = 3;
        public const int IntAnyHelp = 4;

        /***Change this for display***/
        public const String StringPickup = "Airport Picking-up";
        public const String StringHousing = "Temp Housing";
        public const String StringLocalHelp = "Local Help";
        public const String StringAnyHelp = "Any Help";

        public static int ToInt(String helpType)
        {
            int h = IntPickup;
            if (helpType == StringPickup)
            {
                h = IntPickup;
            }
            else if (helpType == StringHousing)
            {
                h = IntHousing;
            }
            else if (helpType == StringLocalHelp)
            {
                h = IntLocalHelp;
            }
            else if (helpType == StringAnyHelp)
            {
                h = IntAnyHelp;
            }
            return h;
        }

        public static String ToString(int h)
        {
            String helpType = StringPickup;
            if (h == IntPickup)
            {
                helpType = StringPickup;
            }
            else if (h == IntHousing)
            {
                helpType = StringHousing;
            }
            else if (h == IntLocalHelp)
            {
                helpType = StringLocalHelp;
            }
            else if (h == IntAnyHelp)
            {
                helpType = StringAnyHelp;
            }
            return helpType;
        }
    }

    public static class RelationToUTD
    {
        /***Never Change this definition, otherwise gender will reverse***/
        public const int IntFacss = 1;
        public const int IntCurrStud = 2;
        public const int IntAlumni = 3;
        public const int IntOther = 4;


        /***Change this for display***/
        public const String StringFacss = "Facss Member";
        public const String StringCurrStud = "Current Student";
        public const String StringAlumni = "UTD Alumni";
        public const String StringOther = "Other";

        public static int ToInt(String relation)
        {
            int r = IntOther;
            if (relation == StringFacss)
            {
                r = IntFacss;
            }
            else if (relation == StringCurrStud)
            {
                r = IntCurrStud;
            }
            else if (relation == StringAlumni)
            {
                r = IntAlumni;
            }
            else if (relation == StringOther)
            {
                r = IntOther;
            }
            return r;
        }

        public static String ToString(int r)
        {
            String relation = StringOther;
            if (r == IntFacss)
            {
                relation = StringFacss;
            }
            else if (r == IntCurrStud)
            {
                relation = StringCurrStud;
            }
            else if (r == IntAlumni)
            {
                relation = StringAlumni;
            }
            else if (r == IntOther)
            {
                relation = StringOther;
            }
            return relation;
        }
    }

    public static class ManualAssignType
    {
        /***Never Change this definition, otherwise gender will reverse***/
        public const int IntPickup = 1;
        public const int IntHousing = 2;
        public const int IntBoth = 3;
        public const int IntDelete = 3;

        /***Change this for display***/
        public const String StringPickup = "For Airport Pick Up";
        public const String StringHousing = "For Temporal Housing";
        public const String StringBoth = "For Both";
        public const String StringDelete = "Delete This Manual Assignment";

        public static int ToInt(String type)
        {
            int r = IntPickup;
            if (type == StringPickup)
            {
                r = IntPickup;
            }
            else if (type == StringHousing)
            {
                r = IntHousing;
            }
            else if (type == StringBoth)
            {
                r = IntBoth;
            }
            else if (type == StringDelete)
            {
                r = IntDelete;
            }
            return r;
        }

        public static String ToString(int t)
        {
            String type = StringPickup;
            if (t == IntPickup)
            {
                type = StringPickup;
            }
            else if (t == IntHousing)
            {
                type = StringHousing;
            }
            else if (t == IntBoth)
            {
                type = StringBoth;
            }
            else if (t == IntDelete)
            {
                type = StringDelete;
            }
            return type;
        }
    }

    public class SetupParameters
    {
        public const int CancelDeadline = 7;  //days
        public const int ConfirmEmailDays = 10;
        public const int EmailNotificationDays = 2;
    }

    public class NewStudentInfoModel
    {
        public NewStudentInfoModel()
        {
            DaysOfTempHousing = TempHouseLength.NotSure;
        }

        public NewStudentInfoModel(NewStudent stud)
        {
            this.Id = stud.Id;
            this.UserName = stud.UserName;
            this.Name = stud.Name;
            this.CnName = stud.CnName;
            this.Gender = stud.Gender;
            this.ComeFrom = stud.ComeFrom;
            this.Email = stud.Email;
            this.Phone = stud.Phone;
            this.Major = stud.Major;
            this.Flight = stud.Flight;
            this.ArrivalTime = stud.ArrivalTime;
            this.NeedPickup = stud.NeedPickup;
            this.NeedTempHousing = stud.NeedTempHousing;
            this.DaysOfTempHousing = stud.DaysOfHousing;
            this.Note = stud.Note;
            this.EntryPort = stud.EntryPort;
            this.RegTime = stud.RegTime;
            this.LastUpdate = stud.LastUpdate;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "用户名(只能包含：英文字母，下划线，数字)")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "英文名字(Last, First)")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "中文名字")]
        public string CnName { get; set; }
        [Required]
        [Display(Name = "性别")]
        public int Gender { get; set; }
        [Required]
        [Display(Name = "来自哪里？")]
        public string ComeFrom { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "邮箱号")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "手机号")]
        public string Phone { get; set; }

        [Display(Name = "专业")]
        public string Major { get; set; }

        [Required]
        [Display(Name = "航班号")]
        public string Flight { get; set; }

        [Required]
        [Display(Name = "到达时间")]
        public DateTime ArrivalTime { get; set; }

        [Display(Name = "是否需要接机？")]
        public bool NeedPickup { get; set; }

        [Display(Name = "是否需要临时住宿？")]
        public bool NeedTempHousing { get; set; }

        [Display(Name = "大概需要几天临住？")]
        public int DaysOfTempHousing { get; set; }

        [Display(Name = "有什么需要特别说明的吗？")]
        public string Note { get; set; }

        [Display(Name = "你到美国的第一站是哪个城市？")]
        public string EntryPort { get; set; }

        public DateTime RegTime { get; set; }
        public DateTime LastUpdate { get; set; }

        internal NewStudent GetNewStudentModel()
        {
            var newStudent = new NewStudent
            {
                Id = Id,
                UserName = UserName,
                Name = Name,
                CnName = CnName,
                Gender = this.Gender,
                Major = Major,
                Email = Email,
                Phone = Phone,
                ArrivalTime = ArrivalTime,
                EntryPort = EntryPort,
                Flight = Flight,
                NeedPickup = NeedPickup,
                NeedTempHousing = NeedTempHousing,
                DaysOfHousing = DaysOfTempHousing,
                Note = Note,
                ComeFrom = ComeFrom,
                RegTime = this.RegTime,
                LastUpdate = this.LastUpdate,
                Year = 2013,
                Term = "Fall",
                Confirmed = ConfirmedStage.UnConfirmed,
            };
            return newStudent;
        }

        internal NewStudent GetNewStudentModel(NewStudent newStudent)
        {

            newStudent.Id = this.Id;
            newStudent.UserName = this.UserName;
            newStudent.Name = this.Name;
            newStudent.CnName = this.CnName;
            newStudent.Gender = this.Gender;
            newStudent.Major = this.Major;
            newStudent.Email = this.Email;
            newStudent.Phone = this.Phone;
            newStudent.ArrivalTime = this.ArrivalTime;
            newStudent.EntryPort = this.EntryPort;
            newStudent.Flight = this.Flight;
            newStudent.NeedPickup = this.NeedPickup;
            newStudent.NeedTempHousing = this.NeedTempHousing;
            newStudent.DaysOfHousing = this.DaysOfTempHousing;
            newStudent.Note = this.Note;
            newStudent.ComeFrom = this.ComeFrom;
            newStudent.RegTime = this.RegTime;
            newStudent.LastUpdate = this.LastUpdate;
            newStudent.Year = 2013;
            newStudent.Term = "Fall";

            return newStudent;
        }
    }

    public class NewStudentRegisterModel : NewStudentInfoModel
    {
        public NewStudentRegisterModel()
        {
            this.RegTime = DateTime.Now;
            this.LastUpdate = DateTime.Now;
        }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码(不短于6位)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "密码确认")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }



        internal RegisterModel GetRegisterModel()
        {
            return new RegisterModel
            {
                UserName = this.UserName,
                Email = this.Email,
                Password = this.Password,
                ConfirmPassword = this.ConfirmPassword,
            };
        }
    }

    public class NewStudentViewModel
    {
        public NewStudentViewModel()
        {
            this.Note = "none";
        }

        public NewStudentViewModel(NewStudent s)
        {
            Id = s.Id;
            UserName = s.UserName;
            Name = s.Name;
            Gender = SystemGender.ToStringGender(s.Gender);
            Email = s.Email;
            Phone = s.Phone;
            Major = s.Major;
            Year = s.Year;
            Term = s.Term;
            ArrivalTime = s.ArrivalTime.ToString("MM/dd/yyyy HH:mm");
            Flight = s.Flight;
            EntryPort = s.EntryPort;
            NeedPickup = s.NeedPickup;
            NeedTempHousing = s.NeedTempHousing;
            DaysOfHousing = TempHouseLength.ToString(s.DaysOfHousing);
            Note = s.Note;
            ComeFrom = s.ComeFrom;
            CnName = s.CnName;
            RegTime = s.RegTime.ToString("MM/dd/yyyy HH:mm");
            LastUpdate = s.LastUpdate.ToString("MM/dd/yyyy HH:mm");

            IsManualAssigned = s.IsManualAssigned;
            ManualAssignedHost = s.ManualAssignedHost;
            ManualAssignedPickup = s.ManualAssignedPickup;

            /*HasApt = s.HasApt;
            WhenApt = s.WhenApt;
            WhereApt = s.WhereApt;
            WillingToHelp = s.WillingToHelp;*/
            HelpNote = s.HelpNote;
            Marked = s.Marked;

            if (s.Organization != null)
            {
                AssignedOrg = s.Organization.Name;
            }
            else
            {
                AssignedOrg = "Not Assigned";
            }

            if (s.Group != null)
            {
                AssignedGrp = s.Group.Name;
            }
            else
            {
                AssignedGrp = "Not Assigned";
            }

            var pickManualAssign = s.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntPickup).FirstOrDefault();
            var housingManualAssign = s.ManualAssignInfoes.Where(m => m.Type == ManualAssignType.IntHousing).FirstOrDefault();

            if (NeedPickup)
            {
                if (s.PickupVolunteer != null && pickManualAssign != null)
                {
                    PickupStatus = "手动自动重复分配, 请删除其中之一";
                }
                else if (s.PickupVolunteer != null)
                {
                    PickupStatus = "Assigned: " + s.PickupVolunteer.Name;
                }
                else if (pickManualAssign != null)
                {
                    PickupStatus = "Assigned: " + pickManualAssign.VolName;
                }
                else
                {
                    PickupStatus = "Yes";
                }
            }
            else
            {
                PickupStatus = "No need";
            }
            if (s.NeedTempHousing)
            {
                if (s.TempHouseVolunteer != null && housingManualAssign != null)
                {
                    PickupStatus = "手动自动重复分配, 请删除其中之一";
                }
                else if (s.TempHouseVolunteer != null)
                {
                    TempHousingStatus = "Assigned: " + s.TempHouseVolunteer.Name;
                }
                else if (housingManualAssign != null)
                {
                    TempHousingStatus = "Assigned: " + housingManualAssign.VolName;
                }
                else
                {
                    TempHousingStatus = "Yes: " + DaysOfHousing;
                }
            }
            else
            {
                TempHousingStatus = "No need";
            }

            string studentConfirmed = "";
            string pickupConfirmed = "";
            string housingConfirmed = "";

            if (!s.NeedPickup)
            {
                pickupConfirmed = "不需要接机";
            }
            else
            {
                if (s.PickupVolunteer == null && pickManualAssign == null)
                {
                    pickupConfirmed = "接机等待分配";
                }
                else
                {
                    var pickupEmail = s.EmailHistories.Where(e => e.Type == EmailType.ArriveNoticeToPickup).FirstOrDefault();
                    if (pickupEmail == null)
                    {
                        pickupConfirmed = "等待发送接机确认邮件";
                    }
                    else
                    {
                        if (pickupEmail.Confirmed)
                        {
                            pickupConfirmed = "接机已确认";
                        }
                        else
                        {
                            pickupConfirmed = "接机邮件已发送，等待确认";
                        }
                    }
                }
            }

            if (!s.NeedTempHousing)
            {
                housingConfirmed = "不需要临住";
            }
            else
            {
                if (s.TempHouseVolunteer == null && housingManualAssign == null)
                {
                    housingConfirmed = "临住等待分配";
                }
                else
                {
                    var housingEmail = s.EmailHistories.Where(e => e.Type == EmailType.ArriveNoticeToHost).FirstOrDefault();
                    if (housingEmail == null)
                    {
                        housingConfirmed = "等待发送临住确认邮件";
                    }
                    else
                    {
                        if (housingEmail.Confirmed)
                        {
                            housingConfirmed = "临住已确认";
                        }
                        else
                        {
                            housingConfirmed = "临住邮件已发送，等待确认";
                        }
                    }
                }
            }

            if (s.NeedPickup || s.NeedTempHousing)
            {
                studentConfirmed = ConfirmedStage.ToString(s.Confirmed);
                var studentEmail = s.EmailHistories.Where(e => e.Type == EmailType.ArriveNoticeToStud).FirstOrDefault();
                if (studentEmail == null)
                {

                }
                else
                {
                    if (studentEmail.Confirmed)
                    {
                        studentConfirmed = "新生已最终确认";
                    }
                    else
                    {
                        studentConfirmed = "新生确认邮件已发送";
                    }
                }
            }
            Confirmed = studentConfirmed + " | " + pickupConfirmed + " | " + housingConfirmed;
        }

        public int Id { get; set; }
        [DisplayName("Username")]
        public string UserName { get; set; }
        [DisplayName("管理员备注")]
        public string HelpNote { get; set; }
        [DisplayName("English Name")]
        public string Name { get; set; }
        [DisplayName("中文名")]
        public string CnName { get; set; }
        [DisplayName("来自")]
        public string ComeFrom { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Phone")]
        public string Phone { get; set; }
        [DisplayName("Major")]
        public string Major { get; set; }
        public int Year { get; set; }
        public string Term { get; set; }
        [DisplayName("Flight")]
        public string Flight { get; set; }
        [DisplayName("是否需要接机?")]
        public bool NeedPickup { get; set; }
        [DisplayName("是否需要临住?")]
        public bool NeedTempHousing { get; set; }
        [DisplayName("入境城市")]
        public string EntryPort { get; set; }
        [DisplayName("新生的备注")]
        public string Note { get; set; }
        [DisplayName("是否标记")]
        public bool Marked { get; set; }
        [DisplayName("临住天数")]
        public string DaysOfHousing { get; set; }
        [DisplayName("进度确认")]
        public String Confirmed { get; set; }
        public bool IsManualAssigned { get; set; }
        public string ManualAssignedPickup { get; set; }
        public string ManualAssignedHost { get; set; }

        [DisplayName("Gender")]
        public string Gender { get; set; }
        [DisplayName("到达时间")]
        public string ArrivalTime { get; set; }
        [DisplayName("注册时间")]
        public string RegTime { get; set; }
        [DisplayName("上次更新")]
        public string LastUpdate { get; set; }
        public string AssignedOrg { get; set; }
        public string AssignedGrp { get; set; }
        public string PickupStatus { get; set; }
        public string TempHousingStatus { get; set; }

        internal NewStudent UpdateNewStudentModel(NewStudent stud)
        {
            stud.Name = this.Name;
            stud.CnName = CnName;
            stud.Gender = SystemGender.ToIntGender(this.Gender);
            stud.Email = this.Email;
            stud.Phone = this.Phone;
            stud.Year = this.Year;
            stud.Term = this.Term;
            stud.Major = this.Major;
            stud.ArrivalTime = DateTime.Parse(this.ArrivalTime);
            stud.EntryPort = this.EntryPort;
            stud.Flight = this.Flight;
            stud.NeedPickup = this.NeedPickup;
            stud.NeedTempHousing = this.NeedTempHousing;

            stud.Note = this.Note;
            stud.ComeFrom = this.ComeFrom;
            stud.Marked = this.Marked;

            stud.HelpNote = this.HelpNote;

            return stud;
        }
    }

    public class VolunteerRegisterModel : VolunteerInfoModel
    {
        public VolunteerRegisterModel()
            : base()
        {
            this.RegTime = DateTime.Now;
            this.OrgPasscode = "1234";
        }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        new internal Volunteer GetVolunteerModel()
        {
            Volunteer v = base.GetVolunteerModel();
            v.UserName = UserName;
            return v;
        }

        internal RegisterModel GetRegisterModel()
        {
            return new RegisterModel
            {
                UserName = this.UserName,
                Email = this.Email,
                Password = this.Password,
                ConfirmPassword = this.ConfirmPassword,
            };
        }
    }

    public class VolunteerInfoModel
    {
        public VolunteerInfoModel()
        {
        }

        public VolunteerInfoModel(Volunteer vol)
        {
            this.Id = vol.Id;
            this.UserName = vol.UserName;
            this.Name = vol.Name;
            this.Gender = vol.Gender;

            this.HelpType = VolunteerHelpType.ToInt(vol.HelpType);
            this.Relation = RelationToUTD.ToInt(vol.RelationToUTD);

            this.Email = vol.Email;
            this.Phone = vol.Phone;

            this.VolunteerOrganizationId = vol.OrganizationId;
            this.VolunteerGroupId = (vol.GroupId != null) ? vol.GroupId.Value : 0;
            this.OrgPasscode = vol.Organization.Passcode;

            this.Address = vol.Address;
            this.BriefIntro = vol.BriefIntro;
            this.Note = vol.Note;
            this.RegTime = vol.RegTime;
        }

        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "用户名(只能包含：英文字母，下划线，数字)")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "姓名: Last, First")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "性别")]
        public int Gender { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "邮箱")]
        public string Email { get; set; }



        [Required]
        [Display(Name = "手机号: xxx-xxx-xxxx")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "您是下面哪个机构的志愿者？")]
        [Range(1, 10000, ErrorMessage = "请选择您要注册的机构")]
        public int VolunteerOrganizationId { get; set; }

        [Display(Name = "您属于该机构的哪个小组？(若无分组则忽略)")]
        public int VolunteerGroupId { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "该机构(小组)的Access Code(如果您是UTD FACSS的志愿者，可忽略)")]
        public string OrgPasscode { get; set; }

        [Required]
        [Display(Name = "下面哪一个最准确的描述您和UTD的关系？")]
        public int Relation { get; set; }

        [Display(Name = "如果其他，请填写")]
        public string OtherRelation { get; set; }

        [Required]
        [Display(Name = "您可以为新生提供哪种帮助？")]
        public int HelpType { get; set; }

        [Display(Name = "个人简介(optional)")]
        public string BriefIntro { get; set; }

        [Display(Name = "任何需要特殊说明的吗?")]
        public string Note { get; set; }

        [Display(Name = "住址: APT /City /State /Zip code")]
        public string Address { get; set; }

        public DateTime RegTime { get; set; }

        internal Volunteer GetVolunteerModel()
        {
            Volunteer v = new Volunteer();

            v.Id = this.Id;
            v.UserName = this.UserName;
            v.Name = this.Name;
            v.Gender = this.Gender;
            v.HelpType = VolunteerHelpType.ToString(this.HelpType);

            if (this.Relation != RelationToUTD.IntOther)
            {
                v.RelationToUTD = RelationToUTD.ToString(this.Relation);
            }
            else
            {
                v.RelationToUTD = this.OtherRelation;
            }

            v.Email = this.Email;
            v.Phone = this.Phone;
            v.OrganizationId = this.VolunteerOrganizationId;
            if (this.VolunteerGroupId > 0)
            {
                v.GroupId = this.VolunteerGroupId;
            }
            else
            {
                v.GroupId = null;
            }

            v.Address = this.Address;
            v.BriefIntro = this.BriefIntro;
            v.Note = this.Note;
            v.RegTime = this.RegTime;

            return v;
        }

        internal Volunteer UpdateVolunteerModel(Volunteer v)
        {
            v.Name = this.Name;
            v.Gender = this.Gender;

            v.HelpType = VolunteerHelpType.ToString(this.HelpType);

            if (this.Relation != RelationToUTD.IntOther)
            {
                v.RelationToUTD = RelationToUTD.ToString(this.Relation);
            }
            else
            {
                v.RelationToUTD = this.OtherRelation;
            }

            v.Email = this.Email;
            v.Phone = this.Phone;
            v.OrganizationId = this.VolunteerOrganizationId;
            if (this.VolunteerGroupId > 0)
            {
                v.GroupId = this.VolunteerGroupId;
            }
            else
            {
                v.GroupId = null;
            }

            v.Address = this.Address;
            v.BriefIntro = this.BriefIntro;
            v.Note = this.Note;

            return v;
        }
    }

    public class VolunteerViewModel
    {
        public VolunteerViewModel()
        {
        }

        public VolunteerViewModel(Volunteer vol)
        {
            this.Id = vol.Id;
            this.Name = vol.Name;
            this.Gender = SystemGender.ToStringGender(vol.Gender);
            this.Email = vol.Email;
            this.Phone = vol.Phone;
            this.Relation = vol.RelationToUTD;
            this.HelpType = vol.HelpType;
            this.BriefIntro = vol.BriefIntro;
            this.Note = vol.Note;
            this.Address = vol.Address;
            this.RegTime = vol.RegTime.ToString("MM/dd/yyyy HH:mm");

            if (vol.PickupNewStudents == null)
            {
                this.NumOfPickup = 0;
            }
            else
            {
                this.NumOfPickup = vol.PickupNewStudents.Count;
            }

            if (vol.TempHouseNewStudents == null)
            {
                this.NumOfHousing = 0;
            }
            else
            {
                this.NumOfHousing = vol.TempHouseNewStudents.Count;
            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public String Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public String Relation { get; set; }
        public String HelpType { get; set; }
        public string BriefIntro { get; set; }
        public string Note { get; set; }
        public string Address { get; set; }
        public String RegTime { get; set; }
        public int NumOfPickup { get; set; }
        public int NumOfHousing { get; set; }

        internal Volunteer UpdateVolunteerModel(Volunteer v)
        {
            v.Name = this.Name;
            v.Gender = SystemGender.ToIntGender(this.Gender);
            v.Email = this.Email;
            v.Phone = this.Phone;
            v.RelationToUTD = this.Relation;
            v.HelpType = this.HelpType;
            v.BriefIntro = this.BriefIntro;
            v.Note = this.Note;
            v.Address = this.Address;
            return v;
        }

        public static List<VolunteerViewModel> GetVolunteerViews(IEnumerable<Volunteer> volunteers)
        {
            var vols = new List<VolunteerViewModel>();

            foreach (var v in volunteers)
            {
                var vol = new VolunteerViewModel(v);
                vols.Add(vol);
            }
            return vols;
        }
    }


    public class StatisticTableView
    {
        public string Name { get; set; }
        public int TotalStud { get; set; }
        public int PickupNum { get; set; }
        public int HousingNum { get; set; }
        public int Total { get; set; }
    }

    public class NewStudentListOps
    {
        public static bool IsOrgFacss(string name)
        {
            if (name.ToLower().Contains("facss"))
            {
                return true;
            }
            return false;
        }

        public static List<NewStudentViewModel> _GetAllNewStudentList()
        {
            StudentModelContainer db = new StudentModelContainer();
            var students = _ConvertStudentList(db.NewStudents.OrderBy(s => s.ArrivalTime));
            db.Dispose();
            return students;
        }

        public static List<NewStudentViewModel> _GetAllUnasignedNewStudent()
        {
            StudentModelContainer db = new StudentModelContainer();
            var stud = db.NewStudents.Where(s => s.Organization == null && (s.NeedPickup || s.NeedTempHousing));
            var students = _ConvertStudentList(stud.OrderBy(s => s.ArrivalTime));
            db.Dispose();
            return students;
        }

        public static List<NewStudentViewModel> _GetNoNeedNewStudent()
        {
            StudentModelContainer db = new StudentModelContainer();
            var stud = db.NewStudents.Where(s => (!s.NeedPickup && !s.NeedTempHousing));
            var students = _ConvertStudentList(stud.OrderBy(s => s.ArrivalTime));
            db.Dispose();
            return students;
        }

        public static List<NewStudentViewModel> _GetNewStudentFromOrg(int orgId)
        {
            StudentModelContainer db = new StudentModelContainer();
            var org = db.Organizations.Find(orgId);
            var students = _ConvertStudentList(org.NewStudents.OrderBy(s => s.ArrivalTime));
            db.Dispose();
            return students;
        }

        public static List<NewStudentViewModel> _GetOrgImcompletedNewStudents(int orgId)
        {
            var students = _GetNewStudentFromOrg(orgId);

            var studList = students.Where(s => s.PickupStatus.Contains("Yes") || s.TempHousingStatus.Contains("Yes")).ToList();

            return studList;
        }

        public static List<NewStudentViewModel> _GetOrgUnassignedNewStudents(int orgId)
        {
            StudentModelContainer db = new StudentModelContainer();
            var org = db.Organizations.Find(orgId);
            var students = org.NewStudents.Where(s => s.Group == null).OrderBy(s => s.ArrivalTime);
            var studList = _ConvertStudentList(students);
            db.Dispose();
            return studList;
        }

        public static List<NewStudentViewModel> _GetNewStudentFromGrp(int grpId)
        {
            StudentModelContainer db = new StudentModelContainer();
            var grp = db.Groups.Find(grpId);
            var students = _ConvertStudentList(grp.NewStudents.OrderBy(s => s.ArrivalTime));
            db.Dispose();
            return students;
        }

        public static List<NewStudentViewModel> _GetGrpUnassignedNewStudents(int grpId)
        {
            var students = _GetNewStudentFromGrp(grpId);

            var studList = students.Where(s => s.PickupStatus.Contains("Yes") || s.TempHousingStatus.Contains("Yes")).ToList();

            return studList.ToList();
        }

        public static NewStudentViewModel _GetOneNewStudent(int Id)
        {
            StudentModelContainer db = new StudentModelContainer();
            var s = db.NewStudents.FirstOrDefault(o => o.Id == Id);
            var student = new NewStudentViewModel(s);
            db.Dispose();
            return student;
        }

        public static List<NewStudentViewModel> _ConvertStudentList(IEnumerable<NewStudent> newStudents)
        {
            var students = new List<NewStudentViewModel>();

            foreach (var s in newStudents)
            {
                var stu = new NewStudentViewModel(s);
                students.Add(stu);
            }
            return students;
        }
    }

    public class SetLeaderModel
    {
        public int Id { get; set; }

        [Display(Name = "Please Select the leader")]
        public int LeaderId { get; set; }
    }

    public class ImageOpt
    {
        public static void ScaleImage(string src, string des, int maxWidth, int maxHeight)
        {
            var image = Image.FromFile(src);

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

            newImage.Save(des, ImageFormat.Jpeg);
            image.Dispose();
        }

        public static void ScaleImage(HttpPostedFileBase file, string des, int maxWidth, int maxHeight)
        {
            var image = Image.FromStream(file.InputStream, true, true);

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            if (ratio >= 1)
            {
                goto exit;
            }

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            newImage.Save(des, ImageFormat.Jpeg);

        exit:
            image.Dispose();
        }
    }

    public class EmailSentModel
    {
        public int Count { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        private string smtpServer = SystemEmailAccount.DefaultEmailAccount;

        public EmailSentModel(string smtpServer)
        {
            this.Count = 0;
            this.To = new List<string>();
            this.Cc = new List<string>();
            this.Bcc = new List<string>();
            this.smtpServer = smtpServer;
        }

        public EmailSentModel()
        {
            this.Count = 0;
            this.To = new List<string>();
            this.Cc = new List<string>();
            this.Bcc = new List<string>();
        }

        public bool Send()
        {
            MailMessage email = new MailMessage();

            foreach (var to in this.To)
            {
                if (IsValidEmail(to))
                {
                    email.To.Add(new MailAddress(to));
                    this.Count++;
                }
            }
            foreach (var cc in this.Cc)
            {
                if (IsValidEmail(cc))
                {
                    email.CC.Add(new MailAddress(cc));
                    this.Count++;
                }
            }
            foreach (var bcc in this.Bcc)
            {
                if (IsValidEmail(bcc))
                {
                    email.Bcc.Add(new MailAddress(bcc));
                    this.Count++;
                }
            }
            email.Subject = this.Subject;
            email.Body = this.Body;

            email.IsBodyHtml = true;

            return SyncSend(email);
        }

        public void AsyncSend()
        {

        }

        public bool SyncSend(MailMessage email)
        {
            var web_db = new WebModelContainer();
            EmailAccount smtp = web_db.EmailAccounts.Where(s => s.Name == this.smtpServer).FirstOrDefault();

            try
            {
                SmtpClient smtpClient = new SmtpClient();

                if (smtp != null)
                {
                    email.From = new MailAddress(smtp.From);
                    smtpClient.Host = smtp.Host;
                    smtpClient.Port = smtp.Port;
                    smtpClient.Credentials = new System.Net.NetworkCredential(smtp.Username, smtp.Password);
                }

                smtpClient.EnableSsl = true;  //must be here, otherwise there will be execption
                smtpClient.Send(email);
                email.Dispose();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool IsValidEmail(string email)
        {
            if (!email.Contains("@"))
            {
                return false;
            }
            if (!email.Contains("."))
            {
                return false;
            }
            return true;
        }
    }

    public class ValidateEmail
    {
        bool invalid = false;

        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
            }
            catch (Exception e)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                      RegexOptions.IgnoreCase);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }

    public class SystemEmailAccount
    {
        public const string DefaultEmailAccount = "Facss";
        public const string FacssEmailAccount = "Facss";
        public const string UTDBaikeEmailAccount = "UtdBaike";
    }
}