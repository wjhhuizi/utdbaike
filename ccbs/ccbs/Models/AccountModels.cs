using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Net.Mail;

namespace ccbs.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    //Model Class    
    public class ResetPasswordModel
    {
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }

        public void ResetPassword(string username)
        {
            MembershipUser currentUser = Membership.GetUser(username);
            string password = currentUser.ResetPassword();
            SendResetEmail(currentUser);
        }

        //Send Email Method
        internal static void SendResetEmail(System.Web.Security.MembershipUser user)
        {
            var emailSendModel = new EmailSentModel();


            emailSendModel.To.Add(user.Email);

            emailSendModel.Subject = "Password Reset";
            string link = "http://www.utdbaike.com/Account/ResetPassword/?username=" + user.UserName + "&reset=" + HashResetParams(user.UserName, user.ProviderUserKey.ToString());
            emailSendModel.Body = "<p>" + user.UserName + " please click the following link to reset your password: <a href='" + link + "'>" + link + "</a></p>";
            emailSendModel.Body += "<p>If you did not request a password reset you do not need to take any action.</p>";
            emailSendModel.Send();
        }

        //Method to hash parameters to generate the Reset URL
        internal static string HashResetParams(string username, string guid)
        {

            byte[] bytesofLink = System.Text.Encoding.UTF8.GetBytes(username + guid);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string HashParams = BitConverter.ToString(md5.ComputeHash(bytesofLink));

            return HashParams;
        }
    }
}
