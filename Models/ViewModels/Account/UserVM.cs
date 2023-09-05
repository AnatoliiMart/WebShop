using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebShop.Models.Data;

namespace WebShop.Models.ViewModels.Account
{
    public class UserVM
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Surname")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("EMail")]
        [DataType(DataType.EmailAddress)]
        public string EmailAdress { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }

        public UserVM(UserMDL user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            EmailAdress = user.EmailAdress;
            Login = user.Login;
            Password = user.Password;
        }
    }
}