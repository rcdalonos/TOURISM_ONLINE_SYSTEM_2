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
    public class RequirementsModel
    {
        //[Key]
        //public int RequirementId { get; set; }
        [NotMapped]
        public string imgname { get; set; }
        [NotMapped]
        public string imgext { get; set; }
        [NotMapped]
        public string imgpath { get; set; }


        [DisplayName("Evaluation Status")]
        public string ApprovalStatus { get; set; }

        [DisplayName("Evaluation Status")]
        public string ApprovalStatusText { get; set; }

        [DisplayName("Status")]
        public List<SelectListItem> ApprovalStatusList { get; set; }

        public string RequirementId { get; set; }
        public string BusinessId { get; set; }

        //[EmailAddress(ErrorMessage = "Invalid Email Address Format")]
        //[Required(ErrorMessage = "Please Enter Email")]
        [DisplayName("Not Applicable")]
        public Boolean ApplicableStatus { get; set; }
        public string ImageFileName { get; set; }

        //[Required(ErrorMessage = "Please Enter Requirement")]
        [DisplayName("Requirement")]
        //[Required(ErrorMessage = "Please Enter Requirement.")]
        public string Requirement { get; set; }

        [Required(ErrorMessage = "Please Enter Certificate No.")]
        [DisplayName("Certificate / Serial No.")]
        public string SerialNo { get; set; }


        [DisplayName("No Expiration")]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")]
        public Boolean ExpirationStatus { get; set; }
        //[Required(ErrorMessage = "Please Enter Business Address")]


        //public string ApplicableStatus { get; set; }

        public string BusinessRequirementId { get; set; }


        //[Required]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:mm-dd-yyyy}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "1/1/1753", "12/31/9999",
        //    ErrorMessage = "Value for {0} must be between {1} and {2}")]

        [Required(ErrorMessage = "Please Select Expiration Date")]
        [DisplayName("Expiration Date")]
        //[DataType(DataType.Date)]
        public string ExpirationDate { get; set; }
        //public DateTime ExpirationDate { get; set; }


        [NotMapped]
        //[Required(ErrorMessage = "please select ImagePath")]
        public string ImagePath { get; set; }

        [DisplayName("Image")]
        //[Required(ErrorMessage = "Please Select Image")]
        public HttpPostedFileBase file { get; set; }

        public string ClassificationId { get; set; }

    }
}