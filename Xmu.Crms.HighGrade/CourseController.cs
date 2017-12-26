using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Xmu.Crms.Services.HighGrade;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Mobile.Controllers.vo;
using Type = Xmu.Crms.Shared.Models.Type;



namespace Xmu.Crms.HighGrade.Controllers
{
    [Route("")]
    [Produces("application/json")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CourseController : Controller
    {

        private readonly ICourseService _courseService;
        private readonly IClassService _classService;
        private readonly IUserService _userService;
        private readonly ISeminarService _seminarService;
        private readonly JwtHeader _header;
        public CourseController(CrmsContext db, ICourseService courseService, IClassService classService, ISeminarService seminarService, IUserService userService, JwtHeader header)
        {

            _courseService = courseService;
            _userService = userService;
            _classService = classService;
            _seminarService = seminarService;
            _header = header;
        }
        [Route("")]
        [HttpGet("/course")]
        public IActionResult GetUserCourses()
        {
            try
            {
                var courses = _courseService.ListCourseByUserId(User.Id());
                return Json(courses);
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }


        }

        //创建课程
        //POST:/course
        [HttpPost("course")]
        public IActionResult PostCourse(long userId, Course course)
        {

            try
            {
                var courses = _courseService.InsertCourseByUserId(userId, course);

            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return Json(new
            {
                id = course.Id
            });
            //JsonResult result = new JsonResult();
            //var course = new { id = 23, name = json.name, description = json.description, startTime = json.startTime, endTime = json.endTime, proportions = json.proportions };
            //result.Data = course.id;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
        }

        //按ID获取课程
        //GET:/course/{courseId}
        [HttpGet("course/{courseId}")]
        public IActionResult GetCourseByCourseId(long courseId)
        {
            try
            {
                var courses = _courseService.GetCourseByCourseId(courseId);
                return Json(new
                {
                    id = courses.Id,
                    name = courses.Name,


                    startdate = courses.StartDate,
                    enddate = courses.EndDate,

                    teacherName = courses.Teacher.Name,
                    teacherMail = courses.Teacher.Email,
                    description = courses.Description
                });
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            //JsonResult result = new JsonResult();
            //var data = new { id = 23, name = "OOAD", description = "面向对象分析与设计", teacherName = "邱明", teacherEmail = "mingqiu@xmu.edu.cn" };
            //result.Data = data;
            //return result;
        }

        //修改课程
        //PUT:/course/{courseId}
        [HttpPut("course/{courseId}")]
        public IActionResult ModifyCourse(long courseId, Course course)
        {
            try
            {
                _courseService.UpdateCourseByCourseId(courseId, course);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(403, new { msg = "无法修改他人课程" });
            }
            return NoContent();
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //return response;
        }

        //按Id删除课程
        //DELETE:/course/{courseId}
        [HttpDelete("course/{courseId}")]
        public IActionResult DeleteCourse(long courseId)
        {
            try
            {
                _courseService.DeleteCourseByCourseId(courseId);
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return NoContent();
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //return response;
            //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //return response;
        }

        //按ID获取课程的班级列表
        //GET:/course/{courseId}/class
        [HttpGet("course/{courseId}/class")]
        public IActionResult GetClass(long courseId)
        {
            try
            {
                var classes = _classService.ListClassByCourseId(courseId);
                return Json(classes.Select(c => new
                {
                    id = c.Id,
                    name = c.Name

,
                    time = c.ClassTime,
                    site = c.Site,
                    courseTeacher = c.Course.Teacher.Name

,
                    courseName = c.Course.Name

                }));
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }

            //JsonResult result = new JsonResult();
            //var data = new object[] {
            //    new { id = 45, name = "周三1-2节"},
            //    new { id = 48, name = "周三3-4节"}
            //};
            //result.Data = data;
            //return result;
        }

        //在指定ID的课程创建班级
        //POST:/course/{courseId}/class
        [HttpPost("course/{courseId}/class")]
        public IActionResult PostClass(long courseId, ClassInfo classInfo)
        {

            try
            {
                var classes = _classService.InsertClassById(courseId, classInfo);
                return Json(classes);
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(403, new { msg = "学生无法创建课程" });
            }
            //JsonResult result = new JsonResult();
            //var Class = new { id = 45 };
            //result.Data = Class;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
        }

        //按课程ID获取讨论课详情列表
        //GET: /course/{courseId}/seminar
        [HttpGet("/course/{courseID}/seminar")]
        public IActionResult Getcourse([FromRoute]int courseID, [FromQuery] string embedgrade)
        {
            //Console.WriteLine("llllllll");
            //Console.WriteLine(courseID);

            IList<Seminar> t = _seminarService.ListSeminarByCourseId(courseID);
            for (int i = 1; i < t.Count(); i++)
                for (int j = 0; j < t.Count() - i; j++)
                {
                    if (DateTime.Compare(t[j].StartTime, t[j + 1].StartTime) < 0)
                    {
                        Seminar temp2 = new Seminar();
                        temp2 = t[j];
                        t[j] = t[j + 1];
                        t[j + 1] = temp2;
                    }
                }
            List<Dseminar> temp = new List<Dseminar>();
            for (int i = 0; i < t.Count(); i++)
            {
                Dseminar a = new Dseminar();
                a.ID = (int)t[i].Id;
                a.Starttime = t[i].StartTime.ToString("yyyy-MM-dd");
                a.Endtime = t[i].EndTime.ToString("yyyy-MM-dd");
                if (t[i].IsFixed == true)
                    a.Groupingmethod = "固定分组";
                else
                    a.Groupingmethod = "随机分组";
                a.Grade = 3;             //没有此方法啊啊啊啊啊啊啊啊！！！
                temp.Add(a);
            }
            return Json(new { message1 = temp, message2 = t.Count() });

            //this.ViewData["count"] = 4;
            //int count = 4;
            //List<Dseminar> temp = new List<Dseminar>();
            //Dseminar a = new Dseminar();
            //a.ID = 4;
            //a.Starttime = "11月4日";
            //a.Endtime = "11月12日";
            //a.Grade = 5;
            //a.Groupingmethod = "固定分组";
            //Dseminar b = new Dseminar();
            //b.ID = 3;
            //b.Starttime = "11月28日";
            //b.Endtime = "11月4日";
            //b.Grade = 4;
            //b.Groupingmethod = "随机分组";
            //Dseminar c = new Dseminar();
            //c.ID = 2;
            //c.Starttime = "10月20日";
            //c.Endtime = "10月28日";
            //c.Grade = 4;
            //c.Groupingmethod = "固定分组";
            //Dseminar d = new Dseminar();
            //d.ID = 1;
            //d.Starttime = "10月10日";
            //d.Endtime = "10月20日";
            //d.Grade = 3;
            //d.Groupingmethod = "随机分组";
            //temp.Add(a);
            //temp.Add(b);
            //temp.Add(c);
            //temp.Add(d);
            ////return Json(temp);
            //return Json(new { message1 = temp, message2 = count });
        }

        //在指定ID的课程创建讨论课
        //POST: /course/{courseId}/seminar
        [HttpPost("course/{courseId}/seminar")]
        public IActionResult PostSeminar([FromRoute]long courseId, Seminar seminar)
        {
            try
            {
                var seminars = _seminarService.InsertSeminarByCourseId(courseId, seminar);
                return Json(seminars);
            }
            catch (InvalidOperationException)
            {
                return StatusCode(403, new { msg = "学生无法创建" });
            }

            //JsonResult result = new JsonResult();
            //var Class = new { id = 32 };
            //result.Data = Class;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
        }

        //获取课程正在进行的讨论课
        ///???对应service未找到没有找到接口
        //GET: /course/{courseId}/seminar/current
        [HttpGet("course/{courseId}/seminar/current")]
        public IActionResult GetSeminar([FromRoute]int courseId)
        {
            try
            {
                var seminars = _seminarService.ListSeminarByCourseId(courseId);
                var classes= _classService.ListClassByCourseId(courseId);
                return Json(new
                {
                    sem = seminars.Select(s => new
                    {
                        id = s.Id,
                        name = s.Name,
                        description = s.Description,
                        groupingMethod = (s.IsFixed == true) ? "fixed" : "random",
                        startTime = s.StartTime,
                        endTime = s.EndTime,
                    }),
                    cla = classes.Select(c => new
                    {
                        id = c.Id,
                        name = c.Name
                    })
                }
                );
            }
            catch (CourseNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到课程" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "课程ID格式错误" });
            }
        }
    }
}