using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        //public string SearchString { get; set; }

        public string ImageFileName { get; set; }


        [DisplayName("Business Status")]
        public string BusinessStatus { get; set; }

        [DisplayName("Approval Status")]
        public string BusinessStatustext { get; set; }

        [NotMapped]
        [DisplayName("Business Status")]
        public List<SelectListItem> BusinessSatusList { get; set; }

        [Required(ErrorMessage = "Please Enter Business Name")]
        [DisplayName("Business Name")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Please Enter Business Owner")]
        [DisplayName("Business Owner")]
        public string BusinessOwner { get; set; }

        [Required(ErrorMessage = "Please Enter Business Address")]
        [DisplayName("Business Address")]
        public string BusinessAddress { get; set; }


        [Required(ErrorMessage = "Please Enter Contact Number")]
        [DisplayName("Contact Number")]
        public string ContactNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address Format")]
        [Required(ErrorMessage = "Please Enter Email")]
        [DisplayName("Email")]
        public string Email { get; set; }

        //[Url(ErrorMessage = "Invalid URL Format")]
        //[Required(ErrorMessage = "Please Enter Website")]
        [DisplayName("Website")]
        public string Website { get; set; }


        //[Required(ErrorMessage = "Please Add Business Image")]
        //public string file { get; set; }

        [NotMapped]
        [DisplayName("Classification")]
        public List<SelectListItem> classificationlist { get; set; }

        [Required(ErrorMessage = "please select classification")]
        [DisplayName("Classification")]
        public string ClassificationId { get; set; }

        //[NotMapped]
        public string UserId { get; set; }

        //[NotMapped]
        //[Required(ErrorMessage = "please select ImagePath")]
        public string ImagePath { get; set; }

        //========image
        [Required(ErrorMessage = "please select image")]
        public HttpPostedFileBase file { get; set; }

        public string fileNameOrig { get; set; }
        public byte[] fileContent { get; set; }

        //=========
    }
}