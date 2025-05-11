using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MVCMasterDetailsSPA.Models
{
    public class Student
    {
        public Student()
        {
            this.CourseModules = new HashSet<CourseModule>();
        }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        [Required, Display(Name = "Admission Date"), DataType(DataType.Date),
            DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime AdmissionDate { get; set; }
        public string MobileNo { get; set; }
        public bool IsEnrolled { get; set; }
        public int CourseId { get; set; }
        public string ImageUrl { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<CourseModule> CourseModules { get; set; }
    }
}