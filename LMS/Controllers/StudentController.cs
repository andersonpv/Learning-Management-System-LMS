using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Catalog()
    {
      return View();
    }

    public IActionResult Class(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }


    public IActionResult ClassListings(string subject, string num)
    {
      System.Diagnostics.Debug.WriteLine(subject + num);
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }


    /*******Begin code to modify********/

    /// <summary>
    /// Returns a JSON array of the classes the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 5530)
    /// "name" - The course name
    /// "season" - The season part of the semester
    /// "year" - The year part of the semester
    /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
      var query = from e in db.Enrollment
                  join classes in db.Classes on e.ClassId equals classes.ClassId into eJoinClasses
                  from ec in eJoinClasses
                  join courses in db.Courses on ec.CatalogId equals courses.CatalogId
                  where e.UId == uid
                  select new
                  {
                    subject = courses.Abrev,
                    number = courses.Number,
                    name = courses.CName,
                    season = ec.Season,
                    year = ec.Year,
                    grade = e.Grade ?? "--"
                  };

      return Json(query.ToArray());
    }

    /// <summary>
    /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The category name that the assignment belongs to
    /// "due" - The due Date/Time
    /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="uid"></param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInClass(string subject, int num, string season, int year, string uid)
    {
      var query = from classes in db.Classes
                  join courses in db.Courses on classes.CatalogId equals courses.CatalogId into clJoinCourses
                  from cc in clJoinCourses
                  join ac in db.AssignmentCategories on classes.ClassId equals ac.ClassId into ccJoinAc
                  from ccac in ccJoinAc
                  join asg in db.Assignments on ccac.AcId equals asg.AcId
                  where (
                          cc.Abrev == subject
                          && int.Parse(cc.Number) == num
                          && classes.Season == season
                          && classes.Year == year
                        )
                  select new
                  {
                    aname = asg.HwName,
                    cname = ccac.AcName,
                    due = asg.DueDate,
                    score = (from sub in db.Submissions
                             where (asg.HwId == sub.HwId && sub.UId == uid)
                             select sub.Score).FirstOrDefault()
                  };

      return Json(query.ToArray());
    }



    /// <summary>
    /// Adds a submission to the given assignment for the given student
    /// The submission should use the current time as its DateTime
    /// You can get the current time with DateTime.Now
    /// The score of the submission should start as 0 until a Professor grades it
    /// If a Student submits to an assignment again, it should replace the submission contents
    /// and the submission time (the score should remain the same).
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="uid">The student submitting the assignment</param>
    /// <param name="contents">The text contents of the student's submission</param>
    /// <returns>A JSON object containing {success = true/false}</returns>
    public IActionResult SubmitAssignmentText(string subject, int num, string season, int year,
      string category, string asgname, string uid, string contents)
    {
      var getHwId = from classes in db.Classes
                    join courses in db.Courses on classes.CatalogId equals courses.CatalogId into clJoinCourses
                    from cc in clJoinCourses
                    join ac in db.AssignmentCategories on classes.ClassId equals ac.ClassId into ccJoinAc
                    from ccac in ccJoinAc
                    join asg in db.Assignments on ccac.AcId equals asg.AcId
                    where (
                            cc.Abrev == subject
                            && int.Parse(cc.Number) == num
                            && classes.Season == season
                            && classes.Year == year
                            && asg.HwName == asgname
                            && ccac.AcName == category
                          )
                    select asg.HwId;

      var getSubmission = from submission in db.Submissions
                          where (submission.UId == uid && submission.HwId == getHwId.First())
                          select submission;


      if (getSubmission.Any())
      {
        Submissions s = getSubmission.First();
        s.STime = DateTime.Now;
        s.Contents = contents;
        db.SaveChanges();
      }
      else
      {
        Submissions s = new Submissions();
        s.UId = uid;
        s.STime = DateTime.Now;
        s.Contents = contents;
        s.Score = 0;
        s.HwId = getHwId.First();
        db.Submissions.Add(s);
        db.SaveChanges();
      }
      return Json(new { success = true });
    }


    /// <summary>
    /// Enrolls a student in a class.
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester</param>
    /// <param name="year">The year part of the semester</param>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing {success = {true/false}. 
    /// false if the student is already enrolled in the class, true otherwise.</returns>
    public IActionResult Enroll(string subject, int num, string season, int year, string uid)
    {
      bool success = false;
      uint classId = 0;
      var getClassId = from classes in db.Classes
                       join courses in db.Courses on classes.CatalogId equals courses.CatalogId
                       where
                       (
                           classes.Year == year
                           && courses.Abrev == subject
                           && int.Parse(courses.Number) == num
                           && classes.Season == season
                       )
                       select classes.ClassId;
      classId = getClassId.FirstOrDefault();
      var uidAndClassid = from e in db.Enrollment
                          where (e.UId == uid && e.ClassId == classId)
                          select e;
      success = uidAndClassid.ToArray().Length == 0;


      if (success)
      {
        Enrollment e = new Enrollment();
        e.ClassId = classId;
        e.UId = uid;
        db.Enrollment.Add(e);
        db.SaveChanges();
      }
      return Json(new { success });
    }



    /// <summary>
    /// Calculates a student's GPA
    /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
    /// Assume all classes are 4 credit hours.
    /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
    /// If a student is not enrolled in any classes, they have a GPA of 0.0.
    /// Otherwise, the point-value of a letter grade is determined by the table on this page:
    /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
    /// </summary>
    /// <param name="uid">The uid of the student</param>
    /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
    public IActionResult GetGPA(string uid)
    {
      double gpa = 0.0;
      double total = 0.0;
      int numClasses = 0;
      // Get all of the classes the student is enrolled in
      var getClasses = from e in db.Enrollment
                       where e.UId == uid
                       select e.Grade;
      // Convert those grades into a number
      foreach (string grade in getClasses)
      {
        if (!(grade is null))
        {
          numClasses++;
          total += GetNumericalGrade(grade);
        }
      }
      // Average those numbers and set the average of those numbers to gpa.
      if (numClasses > 0)
        gpa = (double)((int)(total / numClasses * 1000))/1000;

      return Json(new { gpa });
    }

    /// <summary>
    /// This converts a letter grade to the numerical values described in 
    /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
    /// </summary>
    /// <param name="grade"></param>
    /// <returns></returns>
    private double GetNumericalGrade(string grade)
    {
      
      switch (grade.ToUpper())
      {
        case "A":
          return 4.0;
        case "A-":
          return 3.7;
        case "B+":
          return 3.3;
        case "B":
          return 3.0;
        case "B-":
          return 2.7;
        case "C+":
          return 2.3;
        case "C":
          return 2.0;
        case "C-":
          return 1.7;
        case "D+":
          return 1.3;
        case "D":
          return 1.0;
        case "D-":
          return 0.7;
        case "E":
          return 0.0;
        default:
          return 0.0;
      }
    }

    /*******End code to modify********/

  }
}