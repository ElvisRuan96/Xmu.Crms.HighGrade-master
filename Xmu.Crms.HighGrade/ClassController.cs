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
using System.Security.Claims;

namespace Xmu.Crms.HighGrade.Controllers
{
    [Route("")]
    [Produces("application/json")]
   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClassController : Controller
    {
        private readonly IClassService _classService;
        private readonly IUserService _userService;
        private readonly ISeminarGroupService _seminarGroupService;
        private readonly JwtHeader _header;
        public  ClassController(IClassService classService,ISeminarGroupService seminarGroupService, JwtHeader header)
        {
            _classService = classService;
            _seminarGroupService = seminarGroupService;
            _header = header;
        }

        //获取与当前用户相关的或者符合条件的班级列表
        //GET:/class
        [Route("/class")]
        [HttpGet]
        public IActionResult GetClass()
        {
            IList<ClassInfo> a = _classService.ListClassByUserId(User.Id());
            List<Dclass> list = new List<Dclass>();
            for (int i = 0; i < a.Count(); i++)
            {
                Dclass class1 = new Dclass();
                class1.courseId = (int)a[i].Course.Id;
                class1.Id = (int)a[i].Id;
                class1.Name = a[i].Name;
                //long tid = _classService.getTeacherIdByClassId(class1.Id);
                //UserInfo t = _userService.GetUserByUserId(tid);
                //class1.Teacher = t.Name;
                list.Add(class1);
            }
            return Json(list);
            //var result = new IActionResult(new object[]
            //{
            //    new { id = 23, name="周三1-2节", numStudent = 60,time="周三1-2、周五1-2",site="公寓405",courseName="OOAD",courseTeacher = "邱明"},
            //    new { id = 42, name="一班", numStudent = 60,time="周三34节、周五12节",site="海韵202",courseName=".Net",courseTeacher = "杨律青"}
            //});

            //return result;

        }

        //按ID获取班级详情
        //GET: /class/{classId}
        [HttpGet("class/{classId}")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetClass(int classId,UserInfo user)
        {
            var token = new JwtSecurityToken(_header,
               new JwtPayload(
                   null,
                   null,
                   new[]
                   {
                        new Claim("id", user.Id.ToString()),
                        new Claim("type", user.Type.ToString().ToLower())
                   },
                   null,
                   DateTime.Now.AddDays(7)
               ));
            try
            {
                var classes = _classService.GetClassByClassId(classId);
                return Json(new { id=classes.Id,name=classes.Name,time=classes.ClassTime,site=classes.Site,proportions=classes.PresentationPercentage});
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            //var result = new IActionResult(new { id = 23, name = "周三1-2节", numStudent = 120, time = "周三一二节", site = "海韵201", calling = -1, roster = "/roster/周三12班.xlsx", proportions = new { report = 50, presentation = 50, c = 20, b = 60, a = 20 } });

            //return result;

        }

        //按ID修改班级
        //PUT:/class/{classId}
        [Route("class/{classId}")]
        [HttpPut]
        public IActionResult ModifyClass([FromRoute]long classId, [FromBody]dynamic newclass)
        {
            try
            {
                 _classService.UpdateClassByClassId(classId, newclass);
       
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404,new { msg="未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return NoContent();
        }

        //按Id删除班级
        //DELETE:/course/{classId}
        [Route("class/{classId}")]
        [HttpDelete]
        public IActionResult DeleteClass(int classId)
        {
            try
            {
                _classService.DeleteClassByClassId(classId);

            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return NoContent();
        }

        //按班级ID查找学生列表（查询学号、姓名开头）?????
        //GET:/class/{classId}/student
        //[Route("class/{classId}/student")]
        //[HttpGet]
        //public IActionResult GetStudent(int classId, string numBeginWith, string nameBeginWith)
        //{
        //    var result = new IActionResult(new object[]
        //    {
        //        new { id = 233, name="张三", number="24320152202333"},
        //        new { id = 245, name="张八", number="24320152202334"}
        //    });
        //    return result; 
        //}

        //学生按ID选择班级
        //POST:/class/{classId}/student
        [Route("class/{classId}/student")]
        [HttpPost]
        public IActionResult ChooseClass(int classId, int userId ,[FromBody]dynamic json)
        {
            //IActionResult result = new IActionResult();
            //var Class = new { url = "/class/34/student/2757" };
            //result.Data = Class;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                var classes = _classService.InsertCourseSelectionById(classId, userId);
                
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return NoContent();
        }

        //学生按ID取消选择班级
        //DELETE:/class/{classId}/student/{studentId}
        [Route("class/{classId}/student/{studentId}")]
        [HttpDelete]
        public IActionResult DeleteClass(int classId, int userId, string studentId)
        {
            //IActionResult response = new IActionResult(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //return response;
            try
            {
                _classService.DeleteCourseSelectionById(classId,userId);

            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            //catch (CourseAlreadyExistsException)
            //{
            //    return StatusCode(409, new { msg = "选过同个课程" });
            //}
            return NoContent();
        }

        //按ID获取自身所在班级小组
        //GET:/class/{classId}/classgroup
        [Route("class/{classId}/classgroup")]
        [HttpGet]
        public IActionResult GetClassgroup(int classId)
        {

            //var members = new object[]
            //{
            //    new {id=2756, name="李四", number="23320152202443"},
            //    new {id=2777, name="王五", number="23320152202433"}
            //};
            //var result = new IActionResult(new { leader = new { id = 2757, name = "张三", number = "23320152202333" }, members });

            //return result;
            try
            {
                var classes = _classService.GetClassByClassId(classId);
                return Json(new { id = classes.Id, name = classes.Name, time = classes.ClassTime, site = classes.Site, proportions = classes.PresentationPercentage });
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //班级小组组长辞职
        //PUT:/class/{classId}/classgroup/resign
        [Route("class/{classId}/classgroup/resign")]
        [HttpPut]
        public IActionResult ResignLeader(long groupId, long userId,[FromBody]dynamic json)
        {
            //IActionResult response = new IActionResult(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //return response;
            try
            {
                _seminarGroupService.ResignLeaderById(groupId,userId);
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(403, new { msg = "权限不足" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return NoContent();
        }

        //成为班级小组组长
        //PUT:/class/{classId}/classgroup/assign
        [Route("class/{classId}/classgroup/asssign")]
        [HttpPut]
        public IActionResult BecomeLeader(long groupId, long userId,[FromBody]dynamic json)
        {
            //IActionResult response = new IActionResult(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //return response;
            try
            {
                _seminarGroupService.AssignLeaderById(groupId,userId);
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(403, new { msg = "权限不足" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            //catch (LeaderAlreadyExistsException)
            //{
            //    return StatusCode(409, new { msg = "已经有组长了" });
            //}
            return NoContent();
        }

        //添加班级小组成员
        //PUT:/class/{classId}/classgroup/add
        //[Route("class/{classId}/classgroup/add")]
        //[HttpPut]
        //public IActionResult AddMember([FromBody]dynamic json)
        //{
        //    IActionResult response = new IActionResult(HttpStatusCode.NoContent);
        //    response.Content = new StringContent("成功", Encoding.UTF8);
        //    return response;
        //}

        //移除成员
        //PUT:/class/{classId}/classgroup/remove
        //[Route("class/{classId}/classgroup/remove")]
        //[HttpPut]
        //public IActionResult RemoveMember([FromBody]dynamic json)
        //{
        //    IActionResult response = new IActionResult(HttpStatusCode.NoContent);
        //    response.Content = new StringContent("成功", Encoding.UTF8);
        //    return response;
        //}
    }
}