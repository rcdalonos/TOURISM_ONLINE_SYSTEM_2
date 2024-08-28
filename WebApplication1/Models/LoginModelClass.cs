using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
//using Microsoft.AspNetCore.Http;

namespace WebApplication1.Models
{
    public class LoginModelClass
    {
        [Key]
        public int Userid { get; set; }

        //public string Image { get; set; }


        //[Required(ErrorMessage = "Please choose Profile Picture")]
        //[Display(Name = "Profile Picture")]
        //[NotMapped]
        //public IFormFile ProfilePic { get; set; }

        //===========image 
        //[Required(ErrorMessage = "Please Select Profile Picture")]
        public HttpPostedFileBase file { get; set; }
        //==========

        public string imgname { get; set; }
        //[NotMapped]
        public string imgext { get; set; }
        //[NotMapped]
        public string imgpath { get; set; }

        [Required(ErrorMessage = "Please Enter your Last Name")]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Please Enter your First Name")]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }


        [NotMapped]
        public List<SelectListItem> genderlist { get; set; }
        //public string gendertext { get; set; }

        [Required(ErrorMessage = "Please Select your Gender")]
        [Display(Name = "Gender")]
        public string genderid { get; set; }

        //[DefaultValue("")]
        [Required(ErrorMessage = "Please Enter your Age")]
        [Display(Name = "Age")]
        [Range(18, 120)]
        public int Age { get; set; } = 18;

        [Required(ErrorMessage = "Please Enter your Contact Number")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Please Enter your Address")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please Enter your Email")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter the UserName")]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please Enter the Password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter the Confirm Password")]
        [Display(Name = "Re-Type Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]

        public string UserRePassword { get; set; }

    }
}