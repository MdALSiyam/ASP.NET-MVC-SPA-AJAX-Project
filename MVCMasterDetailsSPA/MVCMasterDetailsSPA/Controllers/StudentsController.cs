using MVCMasterDetailsSPA.DAL;
using MVCMasterDetailsSPA.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using MVCMasterDetailsSPA.ViewModels;
using System.IO;
using System.Web;

namespace MVCMasterDetailsSPA.Controllers
{
    public class StudentsController : Controller
    {
        AppDbContext db=new AppDbContext();
        public ActionResult Index()
        {
            IEnumerable<Student> students = db.Students.Include(c => c.Course).Include(c => c.CourseModules).ToList();
            return View(students);
        }
        [HttpGet]
        public ActionResult CreatePartial()
        {
            StudentViewModel student = new StudentViewModel();
            student.Courses = db.Courses.ToList();
            student.CourseModules.Add(new CourseModule() { CourseModuleId = 1 });
            return PartialView("_CreateStudentPartial", student);
        }
        [HttpPost]
     
        [ValidateAntiForgeryToken]
        public ActionResult CreateStudent(StudentViewModel vobj)
        {
            if (!ModelState.IsValid)
            {
                vobj.Courses = db.Courses.ToList();
                return View(vobj);
            }
            Student student = new Student
            {
                StudentName = vobj.StudentName,
                AdmissionDate = vobj.AdmissionDate,
                MobileNo = vobj.MobileNo,
                CourseId = vobj.CourseId,
                IsEnrolled = vobj.IsEnrolled,
                CourseModules = vobj.CourseModules
            };

            if (vobj.ProfileFile != null)
            {
                string uniqueFileName = GetFileName(vobj.ProfileFile);
                student.ImageUrl = uniqueFileName;
            }
            else
            {
                student.ImageUrl = "noimage.png";
            }

            db.Students.Add(student);
            try
            {
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                vobj.Courses = db.Courses.ToList();
                return View(vobj);
            }
        }
        private string GetFileName(HttpPostedFileBase file)
        {
            string fileName = "";
            if (file != null)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string path = Path.Combine(Server.MapPath("~/images/"), fileName);
                file.SaveAs(path);
            }
            return fileName;
        }

        
        public ActionResult Delete(int id)
        {
            Student student = db.Students.Find(id);
            if (student != null)
            {
                var modules = db.CourseModules.Where(s => s.StudentId == id).ToList();
                db.CourseModules.RemoveRange(modules);
                db.Entry(student).State = EntityState.Deleted;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        public ActionResult EditPartial(int id)
        {
            var student = db.Students.Include(c => c.Course).Include(c => c.CourseModules).Where(s => s.StudentId == id).FirstOrDefault();
            if (student == null)
            {
                return HttpNotFound("Student Not found");
            }
            var vObj = new StudentViewModel
            {
                StudentName = student.StudentName,
                StudentId = student.StudentId,
                AdmissionDate = student.AdmissionDate,
                MobileNo = student.MobileNo,
                CourseId = student.CourseId,
                IsEnrolled = student.IsEnrolled,
                ImageUrl = student.ImageUrl,
                CourseModules = student.CourseModules.ToList(),
                Courses = db.Courses.ToList()
            };
            return PartialView("_EditStudentPartial", vObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditStudent(StudentViewModel vobj, string OldImageUrl)
        {
            if (!ModelState.IsValid)
            {
                vobj.Courses = db.Courses.ToList();
                return Json(new { success = false });
            }
            Student obj = db.Students
                .Include(a => a.CourseModules)
                .FirstOrDefault(x => x.StudentId == vobj.StudentId);
            if (obj != null)
            {
                obj.StudentName = vobj.StudentName;
                obj.CourseId = vobj.CourseId;
                obj.MobileNo = vobj.MobileNo;
                obj.IsEnrolled = vobj.IsEnrolled;
                obj.AdmissionDate = vobj.AdmissionDate;
                if (vobj.ProfileFile != null)
                {
                    string uniqueFileName = GetFileName(vobj.ProfileFile);
                    obj.ImageUrl = uniqueFileName;
                }
                else
                {
                    obj.ImageUrl = OldImageUrl;
                }

                var existingModuleIds = obj.CourseModules.Select(m => m.CourseModuleId).ToList();
                var updatedModuleIds = vobj.CourseModules.Where(m => m.CourseModuleId > 0).Select(m => m.CourseModuleId).ToList();
                var modulesToRemove = obj.CourseModules.Where(m => !updatedModuleIds.Contains(m.CourseModuleId)).ToList();
                foreach (var moduleToRemove in modulesToRemove)
                {
                    db.CourseModules.Remove(moduleToRemove);
                }
                foreach (var item in vobj.CourseModules)
                {
                    if (item.CourseModuleId > 0)
                    {
                        var existingModule = obj.CourseModules.FirstOrDefault(m => m.CourseModuleId == item.CourseModuleId);
                        if (existingModule != null)
                        {
                            existingModule.ModuleName = item.ModuleName;
                            existingModule.Duration = item.Duration;
                        }
                    }
                    else
                    {
                        var newModule = new CourseModule
                        {
                            StudentId = obj.StudentId,
                            ModuleName = item.ModuleName,
                            Duration = item.Duration
                        };
                        obj.CourseModules.Add(newModule);
                    }
                }

                try
                {
                    db.SaveChanges();
                    return Json(new { success = true, redirectUrl = Url.Action("Index") });
                }
                catch (Exception ex)
                {
                    
                    vobj.Courses = db.Courses.ToList();
                    return Json(new { success = false });
                }
            }
            return Json(new { success = false, errors = new[] { "Student not found." } });
        }

    }
}