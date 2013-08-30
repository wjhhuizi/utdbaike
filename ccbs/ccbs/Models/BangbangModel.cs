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
    public class BBUserInfoModel
    {
        public BBUserInfoModel()
        {
        }

        public BBUserInfoModel(BBUser user)
        {
            this.Id = user.Id;
            this.Username = user.Username;
            this.Name = user.Name;
            this.Gender = user.Gender;
            this.ComeFrom = user.ComeFrom;
            this.Email = user.Email;
            this.Phone = user.Phone;
            this.Major = user.Major;
            this.Year = user.Year;
            this.Avatar = user.Avatar;
            this.LocationId = user.LocationId;
            this.RegTime = user.RegTime;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        [Display(Name = "Username(只能包含：英文字母，下划线，数字)")]
        public string Username { get; set; }
        [Required]
        [Display(Name = "Name: First Last")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public int Gender { get; set; }
        [Required]
        [Display(Name = "Where Are you from?")]
        public string ComeFrom { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Display(Name = "Year")]
        public int Year { get; set; }

        [Display(Name = "Major")]
        public string Major { get; set; }

        [Display(Name = "Avatar")]
        public string Avatar { get; set; }

        [Required]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        public DateTime RegTime { get; set; }

        internal BBUser GetBBUserModel()
        {
            var user = new BBUser
            {
                Id = this.Id,
                Username = this.Username,
                Name = this.Name,
                Gender = this.Gender,
                Year = this.Year,
                Major = this.Major,
                Email = this.Email,
                Phone = this.Phone,
                ComeFrom = this.ComeFrom,
                Avatar = this.Avatar,
                IsActive = false,
                LocationId = this.LocationId,
                RegTime = this.RegTime,
            };
            return user;
        }

        internal bool IsValideEduEmail()
        {
            return true;
        }

        public bool IsValideEduEmail(string email, string domain)
        {
            var endValid = email.EndsWith(domain);
            return endValid;
        }
    }

    public class BBUserRegisterModel : BBUserInfoModel
    {
        public BBUserRegisterModel()
        {
            this.RegTime = DateTime.Now;
        }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password(Minimum Length: 6)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmed Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }



        internal RegisterModel GetRegisterModel()
        {
            return new RegisterModel
            {
                UserName = this.Username,
                Email = this.Email,
                Password = this.Password,
                ConfirmPassword = this.ConfirmPassword,
            };
        }
    }
}