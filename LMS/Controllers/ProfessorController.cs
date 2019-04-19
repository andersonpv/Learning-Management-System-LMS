using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
    [Authorize(Roles = "Professor")]
    public class ProfessorController : CommonController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Students(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
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

        public IActionResult Categories(string subject, string num, string season, string year)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            return View();
        }

        public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
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

        public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            return View();
        }

        public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
        {
            ViewData["subject"] = subject;
            ViewData["num"] = num;
            ViewData["season"] = season;
            ViewData["year"] = year;
            ViewData["cat"] = cat;
            ViewData["aname"] = aname;
            ViewData["uid"] = uid;
            return View();
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Returns a JSON array of all the students in a class.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "dob" - date of birth
        /// "grade" - the student's grade in this class
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
        {
            var query = from s in db.Students
                        join e in db.Enrollment
                        on s.UId equals e.UId into sJoinE
                        from se in sJoinE
                        join c in db.Classes on se.ClassId equals c.ClassId into SEC
                        from sec in SEC
                        join course in db.Courses on sec.CatalogId equals course.CatalogId
                        where (course.Abrev == subject && int.Parse(course.Number) == num
                        && sec.Year == year && sec.Season == season)
                        select new
                        {
                            fname = s.FirstName,
                            lname = s.LastName,
                            uid = s.UId,
                            dob = s.Dob,
                            grade = se.Grade
                        };

                        

            return Json(query.ToArray());
        }



        /// <summary>
        /// Returns a JSON array with all the assignments in an assignment category for a class.
        /// If the "category" parameter is null, return all assignments in the class.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The assignment category name.
        /// "due" - The due DateTime
        /// "submissions" - The number of submissions to the assignment
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class, 
        /// or null to return assignments from all categories</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
        {
            if (category is null)
            {
                var allAssignments = from courses in db.Courses
                            join cl in db.Classes on courses.CatalogId equals cl.CatalogId into cJoinCl
                            from instance in cJoinCl
                            join ac in db.AssignmentCategories on instance.ClassId equals ac.ClassId into acStuff
                            from b in acStuff
                            join asg in db.Assignments on b.AcId equals asg.AcId
                            where (courses.Abrev == subject && int.Parse(courses.Number) == num &&
                            instance.Season == season && instance.Year == year)
                            select new
                            {
                                aname = asg.HwName,
                                cname = b.AcName,
                                due = asg.DueDate,
                                submissions = (from i in db.Submissions where i.HwId == asg.HwId select i.HwId).Count(),
                            };
                return Json(allAssignments.ToArray());
            }
            var query = from courses in db.Courses
                        join cl in db.Classes on courses.CatalogId equals cl.CatalogId into cJoinCl
                        from instance in cJoinCl
                        join ac in db.AssignmentCategories on instance.ClassId equals ac.ClassId into acStuff
                        from b in acStuff
                        join asg in db.Assignments on b.AcId equals asg.AcId
                        where (courses.Abrev == subject && int.Parse(courses.Number) == num &&
                        instance.Season == season && instance.Year == year && b.AcName == category)
                        select new
                        {
                            aname = asg.HwName,
                            cname = b.AcName,
                            due = asg.DueDate,
                            submissions = (from i in db.Submissions where i.HwId == asg.HwId select i.HwId).Count(),
                        };
            return Json(query.ToArray());
        }


        /// <summary>
        /// Returns a JSON array of the assignment categories for a certain class.
        /// Each object in the array should have the folling fields:
        /// "name" - The category name
        /// "weight" - The category weight
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
        {
            var query = from courses in db.Courses
                        join cl in db.Classes on courses.CatalogId equals cl.CatalogId into cJoinCl
                        from instance in cJoinCl
                        join ac in db.AssignmentCategories on instance.ClassId equals ac.ClassId
                        where (courses.Abrev == subject && int.Parse(courses.Number) == num &&
                        instance.Season == season && instance.Year == year)
                        select new
                        {
                            name = ac.AcName,
                            weight = int.Parse(ac.Weight),
                        };

            return Json(query.ToArray());
        }

        /// <summary>
        /// Creates a new assignment category for the specified class.
        /// If a category of the given class with the given name already exists, return success = false.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The new category name</param>
        /// <param name="catweight">The new category weight</param>
        /// <returns>A JSON object containing {success = true/false} </returns>
        public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
        {
            bool success = false;
            // First, find the ClassId using the first 4 arguments
            var getClassId = from cl in db.Classes
                             join course in db.Courses
                             on cl.CatalogId equals course.CatalogId
                             where (course.Abrev == subject && int.Parse(course.Number) == num
                             && cl.Season == season && cl.Year == year)
                             select cl.ClassId;
            uint classId = getClassId.FirstOrDefault();

            // Then check if that Class already has an Assignment category with that name.
            var checkIfCatExists = from ac in db.AssignmentCategories
                                   where ac.AcName == category && ac.ClassId == classId
                                   select category;
            if (checkIfCatExists.ToArray().Length == 0)
            {
                success = true;
            }

            // Then create an AssignmentCatagory using category and catweight if it doesn't already exist.
            if (success)
            {
                AssignmentCategories ac = new AssignmentCategories();
                ac.AcName = category;
                string weight = new string('0', 3 - catweight.ToString().Length) + catweight.ToString();
                ac.Weight = weight;
                ac.ClassId = classId;
                db.AssignmentCategories.Add(ac);
                db.SaveChanges();

            }
            return Json(new { success });
        }

        /// <summary>
        /// Creates a new assignment for the given class and category.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="asgpoints">The max point value for the new assignment</param>
        /// <param name="asgdue">The due DateTime for the new assignment</param>
        /// <param name="asgcontents">The contents of the new assignment</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
        {
            var query = from cl in db.Classes
                        join courses in db.Courses on cl.CatalogId equals courses.CatalogId into clCourses
                        from clC in clCourses
                        join ac in db.AssignmentCategories on cl.ClassId equals ac.ClassId
                        where (clC.Abrev == subject && int.Parse(clC.Number) == num && cl.Season == season && cl.Year == year && ac.AcName == category)
                        select ac.AcId;

            var acHasSameName = from a in db.Assignments
                                where a.AcId == query.FirstOrDefault()
                                && a.HwName == asgname
                                select a;

            bool success = acHasSameName.ToArray().Length == 0;
            if (success)
            {
                Assignments assignment = new Assignments();
                assignment.HwName = asgname;
                assignment.MaxPoints = (short)asgpoints;
                assignment.Instructions = asgcontents;
                assignment.DueDate = asgdue;
                assignment.AcId = query.FirstOrDefault();
                db.Assignments.Add(assignment);
                db.SaveChanges();
                AdjustAllGradesInAClass(GetClassID(season, year, subject, num));
            }
            
            return Json(new { success });
        }


        /// <summary>
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            var query = from courses in db.Courses
                        join classes in db.Classes on courses.CatalogId equals classes.CatalogId into cJoinCl
                        from courseClass in cJoinCl
                        join assignmentCat in db.AssignmentCategories on courseClass.ClassId equals assignmentCat.ClassId into courseClassAsgnCategories
                        from cca in courseClassAsgnCategories
                        join asg in db.Assignments on cca.AcId equals asg.AcId into CourseClassAsgnCatAsgn
                        from coclaca in CourseClassAsgnCatAsgn
                        join sub in db.Submissions on coclaca.HwId equals sub.HwId into Hwsub
                        from hwsub in Hwsub
                        join s in db.Students on hwsub.UId equals s.UId
                        where (courses.Abrev == subject && int.Parse(courses.Number) == num &&
                        courseClass.Season == season && courseClass.Year == year && cca.AcName == category && coclaca.HwName == asgname)
                        select new
                        {
                            fname = s.FirstName,
                            lname = s.LastName,
                            uid = s.UId,
                            time = hwsub.STime,
                            score = hwsub.Score,
                        };

            return Json(query.ToArray());
        }


        /// <summary>
        /// Set the score of an assignment submission
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <param name="uid">The uid of the student who's submission is being graded</param>
        /// <param name="score">The new score for the submission</param>
        /// <returns>A JSON object containing success = true/false</returns>
        public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
        {
            var query = from courses in db.Courses
                        join classes in db.Classes on courses.CatalogId equals classes.CatalogId into cJoinCl
                        from courseClass in cJoinCl
                        join assignmentCat in db.AssignmentCategories on courseClass.ClassId equals assignmentCat.ClassId into courseClassAsgnCategories
                        from cca in courseClassAsgnCategories
                        join asg in db.Assignments on cca.AcId equals asg.AcId into CourseClassAsgnCatAsgn
                        from coclaca in CourseClassAsgnCatAsgn
                        join sub in db.Submissions on coclaca.HwId equals sub.HwId 
                        where (
                            courses.Abrev == subject 
                            && int.Parse(courses.Number) == num 
                            && courseClass.Season == season 
                            && courseClass.Year == year 
                            && cca.AcName == category 
                            && coclaca.HwName == asgname
                            && sub.UId == uid
                        )
                        select sub;
            
            Submissions submission = query.SingleOrDefault();
            submission.Score = (ushort?)score;
            db.SaveChanges();

            AdjustAllGradesInAClass(GetClassID(season, year, subject, num));

            return Json(new { success = true });
        }


        /// <summary>
        /// Returns a JSON array of the classes taught by the specified professor
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 5530)
        /// "name" - The course name
        /// "season" - The season part of the semester in which the class is taught
        /// "year" - The year part of the semester in which the class is taught
        /// </summary>
        /// <param name="uid">The professor's uid</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            var query = from course in db.Courses
                        join cl in db.Classes
                        on course.CatalogId equals cl.CatalogId
                        where cl.ProfId == uid
                        select new
                        {
                            subject = course.Abrev,
                            number = course.Number,
                            name = course.CName,
                            season = cl.Season,
                            year = cl.Year
                        };
            return Json(query.ToArray());
        }
        /*******End code to modify********/

        private void AdjustAllGradesInAClass(uint classid)
        {
            // For a particular class, we will find
            
            // 1. All the category weights for that class
           // object[] catWeights = GetCategoriesWeight(classid);
            Dictionary<uint, int> catWeights = GetCategoriesWeight(classid);


            // 2. All the total points possible for each category
            Dictionary<uint, int?> categoryPossiblePoints = GetCategoriesPossiblePoints(classid);

            Dictionary<uint, double> categoryConstants = new Dictionary<uint, double>();

            uint[] allAcid = GetAcId(classid);
            foreach (uint acid in allAcid)
            {
                if (categoryPossiblePoints[acid] is null || categoryPossiblePoints[acid] == 0)
                {
                    catWeights.Remove(acid);
                    categoryPossiblePoints.Remove(acid);
                }
                else
                {
                    categoryConstants.Add(acid, (double)catWeights[acid] / (double)categoryPossiblePoints[acid]);
                }
            }

            int totalWeight = 0;
            foreach (uint acid in catWeights.Keys)
            {
                totalWeight += catWeights[acid];
            }

            // 3. for each student in the class, we will find the total points earned in each category.
            var allEnrolledStudents = GetAllEnrolledStudents(classid);
            foreach(string uid in allEnrolledStudents)
            {
                Dictionary<uint, int?> studentScores = GetScoresOfStudentInClass(classid, uid);
                // 4. For each student, we will then calculate and save their grade in the class.
                double grade = 0.0;
                // Cycle through the Assignment Categories, do relevent calculations
                foreach (uint acid in categoryConstants.Keys)
                {
                    double score = studentScores[acid] is null ? 0 : (double)studentScores[acid];
                    grade += score * categoryConstants[acid] / (double)totalWeight;
                }
                // Set the Student's grade to be something.
                string g = ConvertDoubleToGrade(grade*100);
                Enrollment e = (from enroll in db.Enrollment
                                where enroll.UId == uid
                                && enroll.ClassId == classid
                                select enroll).FirstOrDefault();
                e.Grade = g;
                db.SaveChanges();
            }
        }

        private uint[] GetAcId(uint classid)
        {
            var query = from ac in db.AssignmentCategories
                        where ac.ClassId == classid
                        select ac.AcId;
            return query.ToArray();
        }

        private string ConvertDoubleToGrade(double grade)
        {
            if (grade >= 93.0)
                return "A";
            else if (grade >= 90.0)
                return "A-";
            else if (grade >= 87)
                return "B+";
            else if (grade >= 83)
                return "B";
            else if (grade >= 80)
                return "B-";
            else if (grade >= 77)
                return "C+";
            else if (grade >= 73)
                return "C";
            else if (grade >= 70)
                return "C-";
            else if (grade >= 67)
                return "D+";
            else if (grade >= 63)
                return "D";
            else if (grade >= 60)
                return "D-";
            else
                return "E";
        }

        private IQueryable GetAllEnrolledStudents(uint classid)
        {
            var students = from e in db.Enrollment
                           where e.ClassId == classid
                           select e.UId;
            return students;
        }

        private Dictionary<uint, int?> GetScoresOfStudentInClass(uint classid, string uid)
        {
            var query = from ac in db.AssignmentCategories
                        where ac.ClassId == classid
                        select new
                        {
                            ac.AcId,
                            pointsEarned = (from a in ac.Assignments
                                            from s in a.Submissions
                                            where s.UId == uid
                                            select s.Score).Sum(x => x)
                        };
            Dictionary<uint, int?> result = new Dictionary<uint, int?>();
            foreach (var item in query)
            {
                result.Add(item.AcId, item.pointsEarned);
            }
            return result;
        }

        private Dictionary<uint, int?> GetCategoriesPossiblePoints(uint classid)
        {
            var query = from ac in db.AssignmentCategories
                        where ac.ClassId == classid
                        select new
                        {
                            ac.AcId,
                            totalPoint = (from assg in db.Assignments
                                          where assg.AcId == ac.AcId
                                          group assg by assg.AcId into Groups
                                          select Groups.Sum(x => x.MaxPoints)).FirstOrDefault()
                        };

            Dictionary<uint, int?> result = new Dictionary<uint, int?>();
            foreach (var item in query)
            {
                result.Add(item.AcId, item.totalPoint);
            }
            return result;
        }

        /// <summary>
        /// Helper method that returns the query used to find all of the category weights in a particular class.
        /// </summary>
        /// <param name="classid"></param>
        /// <returns></returns>
        private Dictionary<uint, int> GetCategoriesWeight(uint classid)
        {
            var query = from ac in db.AssignmentCategories
                        where ac.ClassId == classid
                        select new
                        {
                            ac.AcId,
                            ac.Weight
                        };
            Dictionary<uint, int> result = new Dictionary<uint, int>();
            foreach(var item in query)
            {
                result.Add(item.AcId, int.Parse(item.Weight));
            }
            return result;
        }

        /// <summary>
        /// Returns the classID of a particular class.
        /// </summary>
        /// <param name="season"></param>
        /// <param name="year"></param>
        /// <param name="subject"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        private uint GetClassID(string season, int year, string subject, int num)
        {
            var classID = from classes in db.Classes
                          join course in db.Courses
                          on classes.CatalogId equals course.CatalogId
                          where course.Abrev == subject
                          && int.Parse(course.Number) == num
                          && classes.Season == season
                          && classes.Year == year
                          select classes.ClassId;

            return classID.FirstOrDefault();
        }
    }
}