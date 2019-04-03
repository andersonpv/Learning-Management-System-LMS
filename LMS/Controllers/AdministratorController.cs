using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Department(string subject)
        {
            ViewData["subject"] = subject;
            return View();
        }

        public IActionResult Course(string subject, string num)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            return View();
        }

        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of all the courses in the given department.
        /// Each object in the array should have the following fields:
        /// "number" - The course number (as in 5530)
        /// "name" - The course name (as in "Database Systems")
        /// </summary>
        /// <param name="subject">The department subject abbreviation (as in "CS")</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetCourses(string subject)
        {
            var query = from c in db.Courses
                        where c.Abrev == subject
                        select new
                        {
                            number = c.Number,
                            name = c.CName,
                        };
                        
            return Json(query.ToArray());
        }





        /// <summary>
        /// Returns a JSON array of all the professors working in a given department.
        /// Each object in the array should have the following fields:
        /// "lname" - The professor's last name
        /// "fname" - The professor's first name
        /// "uid" - The professor's uid
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <returns>The JSON result</returns>
        public IActionResult GetProfessors(string subject)
        {
            var query = from p in db.Professors
                        where p.Abrev == subject
                        select new
                        {
                            lname = p.LastName,
                            fname = p.FirstName,
                            uid = p.UId
                        };
            return Json(query.ToArray());
        }



        /// <summary>
        /// Creates a course.
        /// A course is uniquely identified by its number + the subject to which it belongs
        /// </summary>
        /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
        /// <param name="number">The course number</param>
        /// <param name="name">The course name</param>
        /// <returns>A JSON object containing {success = true/false}.
        /// false if the course already exists, true otherwise.</returns>
        public IActionResult CreateCourse(string subject, int number, string name)
        {
            bool successfulAdd = false;
            // if the course exists, don't add anything.
            var query = from c in db.Courses
                        where (c.Abrev == subject
                        && c.Number == number.ToString())
                        select c.CName;
            if (query.FirstOrDefault() is null)
            {
                successfulAdd = true;
            }
            // if it doesn't exist, add the stuff.
            if (successfulAdd)
            {
                Courses c = new Courses();
                c.CName = name;
                c.Number = number.ToString();
                c.Abrev = subject;
                c.CatalogId = GetCatalogId();
                db.Courses.Add(c);
                db.SaveChanges();
            }
            
            return Json(new { success = successfulAdd });
        }



        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        /// false if another class occupies the same location during any time 
        /// within the start-end range in the same semester, or if there is already
        /// a Class offering of the same Course in the same Semester,
        /// true otherwise.</returns>
        public IActionResult CreateClass(string subject, int number, string season, 
            int year, DateTime start, DateTime end, string location, string instructor)
        {
            bool isSuccessful = false;

            var query = from cl in db.Courses
                        where (cl.Abrev == subject && cl.Number == number.ToString())
                        select cl.CatalogId;


            var getExistingClass = from cl in db.Classes
                                   where (cl.CatalogId == query.FirstOrDefault() 
                                   && cl.Season == season
                                   && cl.Year == year)
                                   select cl.ClassId;
            //var getClassesInLocationAtThatTime;
            if (getExistingClass.ToArray().Length == 0 
                /* && the other query for locationn is also empty */)
            {
                isSuccessful = true;
            }

            if (isSuccessful)
            {
                Classes c = new Classes();
                c.CatalogId = query.FirstOrDefault();
                c.Season = season;
                c.Year = (uint)year;
                c.ProfId = instructor;
                c.Location = location;
                // TODO: add a start and stop time.
                db.Classes.Add(c);
                db.SaveChanges();
            }

            return Json(new { success = isSuccessful});
        }

        /// <summary>
        /// Generates a new unique CatalogId.
        /// </summary>
        /// <returns></returns>
        private string GetCatalogId()
        {
            var query = (from c in db.Courses
                        orderby c.CatalogId descending
                        select c.CatalogId);

            string prev = query.FirstOrDefault().ToString();
            return new string('0', 5 - prev.Length) +(int.Parse(prev) + 1).ToString();
        }
        /*******End code to modify********/

    }
}