using MVCMasterDetailsSPA.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCMasterDetailsSPA.ViewModels
{
    public class StudentViewModel
    {
        public int StudentId { get; set; }
        [Required(ErrorMessage = "Student Name is required.")]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; }
        [Required(ErrorMessage = "Admission Date is required.")]
        [Display(Name = "Admission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AdmissionDate { get; set; }
        [Required(ErrorMessage = "Mobile Number is required.")]

        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }
        [Display(Name = "Enrolled?")]
        public bool IsEnrolled { get; set; }
        [Required(ErrorMessage = "Course is required.")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }
        public string CourseName { get; set; }
        public int CourseModuleId { get; set; }
        public string ModuleName { get; set; }
      
        public int Duration { get; set; }

        [Display(Name = "Upload Picture")]
        public HttpPostedFileBase ProfileFile { get; set; }
        public virtual IList<Course> Courses { get; set; }
        public virtual IList<Student> Students { get; set; }
        public virtual IList<CourseModule> CourseModules { get; set; } = new List<CourseModule>();
    }
}