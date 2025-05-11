namespace MVCMasterDetailsSPA.Models
{
    public class CourseModule
    {
        public int CourseModuleId { get; set; }
        public int StudentId { get; set; }
        public string ModuleName { get; set; }
        public int Duration { get; set; }
        public virtual Student Student { get; set; }
    }
}