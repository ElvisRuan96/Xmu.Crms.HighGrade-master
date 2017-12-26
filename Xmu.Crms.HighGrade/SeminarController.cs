using Microsoft.AspNetCore.Mvc;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using Xmu.Crms.Shared.Exceptions;
using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using static Xmu.Crms.HighGrade.Utils;
using Xmu.Crms.Mobile.Controllers.vo;
using System.Security.Claims;

namespace Xmu.Crms.HighGrade.Controllers
{
    [Route("")]
    [Produces("application/json")]
   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SeminarController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly ISeminarService _seminarService;
        private readonly ISeminarGroupService _seminarGroupService;
        private readonly IUserService _userService;
        private readonly JwtHeader _header;

        public SeminarController(ITopicService topicService, ISeminarService seminarService, ISeminarGroupService seminarGroupService, IUserService userService, JwtHeader header)
        {
            _topicService = topicService;
            _seminarService = seminarService;
            _seminarGroupService = seminarGroupService;
            _userService = userService;
            _header = header;
        }

        //按ID获取讨论课
        // GET: /seminar/{seminarId}
        [Route("/seminar/{seminarId}")]
        [HttpGet]
        public IActionResult GetSeminarById(int seminarId)
        {
            try
            {
                Seminar seminar = _seminarService.GetSeminarBySeminarId(seminarId);
                List<Topic> topics = new List<Topic>(_topicService.ListTopicBySeminarId(seminarId));
                return Json(new
                {
                    id = seminar.Id,
                    name = seminar.Name,
                    description = seminar.Description,
                    groupingMethod = seminar.IsFixed,
                    startTime = seminar.StartTime,
                    endTime = seminar.EndTime,
                    topics = topics.Select(t => new { id = t.Id, name = t.Name })
                });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //按ID修改讨论课
        // PUT: /seminar/{seminarId}
        [Route("seminar/{seminarId}")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult PutSeminarById(int seminarId, [FromBody]Seminar seminar)
        {
            //IActionResult response = new IActionResult(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //IActionResult response2 = new IActionResult(HttpStatusCode.BadRequest);
            //response2.Content = new StringContent("错误的ID格式", Encoding.UTF8);
            //IActionResult response3 = new IActionResult(HttpStatusCode.NotFound);
            //response3.Content = new StringContent("未找到讨论课", Encoding.UTF8);
            //return response;
            try
            {
                _seminarService.UpdateSeminarBySeminarId(seminarId, seminar);
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return NoContent();
        }

        //按ID删除讨论课
        // DELETE: /seminar/{seminarId}
        [Route("seminar/{seminarId}")]
        [HttpDelete]
        public IActionResult DeleteSeminarById(int seminarId)
        {
            //IActionResult response = new IActionResult(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //IActionResult response2 = new IActionResult(HttpStatusCode.BadRequest);
            //response2.Content = new StringContent("错误的ID格式", Encoding.UTF8);
            //IActionResult response3 = new IActionResult(HttpStatusCode.Forbidden);
            //response3.Content = new StringContent("权限不足", Encoding.UTF8);
            //IActionResult response4 = new IActionResult(HttpStatusCode.NotFound);
            //response3.Content = new StringContent("未找到讨论课", Encoding.UTF8);
            //return response;
            try
            {
                _seminarService.DeleteSeminarBySeminarId(seminarId);
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return NoContent();
        }

        //按ID获取与学生有关的讨论课信息
        // GET: /seminar/{seminarId}/my
        [Route("seminar/{seminarId}/my")]
        [HttpGet]
        public IActionResult GetSeminarMy(int seminarId, int userId)
        {
            //var seminar = new object[] { new { id = 32, name = "概要设计", groupingMethod = "random",
            //    courseName="OOAD",startTime = "2017-10-10", endTime = "2017-10-24",
            //     classCalling =23,isLeader=true,areTopicSelected=true } };
            //var result = new IActionResult();
            //result.Data = seminar;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                var seminar = _seminarService.GetSeminarBySeminarId(seminarId);
                return Json(new
                {
                    id = seminar.Id,
                    name = seminar.Name,
                    groupingMethod = seminar.IsFixed,
                    startTime = seminar.StartTime,
                    endTime = seminar.EndTime,
                    courseName = seminar.Course.Name,
                });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //按ID获取讨论课详情
        // GET: /seminar/{seminarId}/detail
        [Route("seminar/{seminarId}/detail")]
        [HttpGet]
        public IActionResult GetSeminarDetail(int seminarId)
        {
            //var seminar = new object[] { new { id = 32, name = "概要设计", groupingMethod = "random",
            //	courseName="OOAD",startTime = "2017-10-10", endTime = "2017-10-24",
            //	 site="海韵201",teacherName="邱明",teacherEmail="mingqiu@xmu.edu.cn" } };
            //var result = new IActionResult();
            //result.Data = seminar;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;

            try
            {
                var seminar = _seminarService.GetSeminarBySeminarId(seminarId);
                return Json(new
                {
                    id = seminar.Id,
                    name = seminar.Name,
                    groupingMethod = seminar.IsFixed,
                    courseName = seminar.Course.Name,
                    startTime = seminar.StartTime,
                    endTime = seminar.EndTime,
                    //site = ...获取不到
                    teacherName = seminar.Course.Teacher.Name,
                    teacherEmail = seminar.Course.Teacher.Email,
                });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //按ID获取讨论课的话题
        // GET: /seminar/{seminarId}/topic
        [Route("seminar/{seminarId}/topic")]
        [HttpGet]
        public IActionResult GetSeminarTopic(int seminarId)
        {
            //var seminar = new object[] { new { id= 257,serial="A",name="领域模型与模块",description= "Domain model与模块划分",
            // groupLimit= 5,groupMemberLimit= 6,groupLeft=2 }, new { id= 257,serial="B",name="数据库设计",description= "数据库逻辑与物理结构设计",
            // groupLimit= 5,groupMemberLimit= 6,groupLeft=1 } , new { id= 257,serial="C",name="概要设计",description= "类图以及模块划分",
            // groupLimit= 5,groupMemberLimit= 6,groupLeft=0 }};
            //var result = new IActionResult();
            //result.Data = seminar;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                var topics = _topicService.ListTopicBySeminarId(seminarId);
                return Json(topics.Select
                    (t => new {
                        id = t.Id,
                        serial = t.Name,
                        name = t.Name,
                        description = t.Description,
                        groupLimit = t.GroupNumberLimit,
                        groupMemberLimit = t.GroupNumberLimit,
                        //groupLeft = 0 怎么获取？？？
                    }));
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //在指定ID的讨论课创建话题
        //POST: /seminar/{seminarId}/topic
        [Route("seminar/{seminarId}/topic")]
        [HttpPost]
        public IActionResult PostSeminarTopic(int seminarId, [FromBody]Topic topic)
        {
            //var seminar = new { id = 257 };
            //var result = new IActionResult();
            //result.Data = seminar;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                _topicService.InsertTopicBySeminarId(seminarId, topic);
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return NoContent();
        }

        ////////////////////////////////////////////////////////////////////这个不行
        //按讨论课ID查找小组
        // GET: /seminar/{seminarId}/group
        [Route("seminar/{seminarId}/group")]
        [HttpGet]
        public IActionResult GetSeminarGroup(int seminarId, Boolean gradeable, int classId)
        {
            //var group = new object[]
            //{
            //	 new { id = 28, name = "1A1", topics = new { id = 257, name = "领域模型与模块" } },
            //	 new { id = 29, name = "1A2", topics = new { id = 257, name = "领域模型与模块" } }
            //};
            //var result = new IActionResult();
            //result.Data = group;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                var seminarGroups = new List<SeminarGroup>(_seminarGroupService.ListSeminarGroupBySeminarId(seminarId));
                return Json(seminarGroups.Select(
                    g => new
                    {
                        id = g.Id,
                        //name = g.Name  小组现在根本没有名字吧？
                        //topic =		根本拿不到吧？
                    }));
            }
            //catch
            //{

            //}
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }

        }

        ////////////////////////////////////////////////////////////////////这个不行
        //按讨论课ID获取学生所在小组详情
        // GET: /seminar/{seminarId}/group/my
        [Route("seminar/{seminarId}/group/my")]
        [HttpGet]
        public IActionResult GetSeminarGroupMy(int seminarId)
        {
            //var group = new
            //{
            //	id = 28,
            //	name = 28,
            //	leader = new { id = 8888, name = "张三" },
            //	members = new object[] { new { id = 5354, name = "李四" } },
            //	topics = new object[] { new { id = 257, name = "领域模型与模块" } }
            //};
            //var result = new IActionResult();
            //result.Data = group;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                //没有userId怎么查my
                return Json(0);
            }
            //catch
            //{

            //}
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        ////////////////////////////////////////////////////////////////////这个不行
        //按ID获取讨论课班级签到、分组状态
        // GET: /seminar/{seminarId}/class/{classId}/attendance
        [Route("seminar/{seminarId}/class/{classId}/attendance")]
        [HttpGet]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetClassAttendance(int seminarId, int classId, UserInfo user)
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
            //var attendance = new { numPresent = 40, numStudent = 60, status = "calling", group = "grouping" };
            //var result = new IActionResult();
            //result.Data = attendance;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                var attendance = _userService.ListAttendanceById(classId, seminarId);
                var present = attendance.Where(a => a.AttendanceStatus == AttendanceStatus.Present);
                return Json(new
                {
                    numPresent = present.Count(),
                    numStudent = attendance.Count(),
                    //status = 不存在的，根本没有地方有这个信息吧
                });
            }
            //catch
            //{

            //}
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //按ID获取讨论课班级已签到名单
        // GET: /seminar/{seminarId}/class/{classId}/attendance/present
       // [Route("seminar/{seminarId}/class/{classId}/attendance/present")]
        [HttpGet]
        public IActionResult GetClassAttendancePresent(int seminarId, int classId)
        {
            //var attendance = new object[] { new { id = 2357, name = "张三" }, new { id = 8232, name = "李四" } };
            //var result = new IActionResult();
            //result.Data = attendance;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                var attendance = _userService.ListAttendanceById(classId, seminarId);
                var present = attendance.Where(a => a.AttendanceStatus == AttendanceStatus.Present);
                return Json(present.Select
                    (p => new {
                        id = p.Id,
                        name = p.Student.Name
                    }));
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //按ID获取讨论课班级迟到签到名单
        // GET: /seminar/{seminarId}/class/{classId}/attendance/late
        [Route("seminar/{seminarId}/class/{classId}/attendance/late")]
        [HttpGet]
        public IActionResult GetClassAttendanceLate(int seminarId, int classId)
        {
            //var attendance = new object[] { new { id = 3412, name = "王五" }, new { id = 5234, name = "王七九" } };
            //var result = new IActionResult();
            //result.Data = attendance;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                var attendance = _userService.ListAttendanceById(classId, seminarId);
                var late = attendance.Where(a => a.AttendanceStatus == AttendanceStatus.Late);
                return Json(late.Select
                    (p => new {
                        id = p.Id,
                        name = p.Student.Name
                    }));
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //按ID获取讨论课班级缺勤名单
        // GET: /seminar/{seminarId}/class/{classId}/attendance/absent
        [Route("seminar/{seminarId}/class/{classId}/attendance/absent")]
        [HttpGet]
        public IActionResult GetClassAttendanceAbsent(int seminarId, int classId)
        {
            //var attendance = new object[] { new { id = 34, name = "张六" } };
            //var result = new IActionResult();
            //result.Data = attendance;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            //return result;
            try
            {
                var attendance = _userService.ListAttendanceById(classId, seminarId);
                var absent = attendance.Where(a => a.AttendanceStatus == AttendanceStatus.Absent);
                return Json(absent.Select
                    (p => new {
                        id = p.Id,
                        name = p.Student.Name
                    }));
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //签到（上传位置信息）
        // PUT: /seminar/{seminarId}/class/{classId}/attendance/{studentId}
        [Route("api/seminar/{seminarId}/class/{classId}/attendance/{studentId}")]
        [HttpPut]
        public IActionResult CallTheRoll(int seminarId, int classId, string studentId)
        {
            //var status = new { status = "late" };
            //var result = new IActionResult();
            //result.Data = status;
            //result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            //IActionResult response2 = new IActionResult(HttpStatusCode.BadRequest);
            //response2.Content = new StringContent("签到失败", Encoding.UTF8);
            //IActionResult response3 = new IActionResult(HttpStatusCode.Forbidden);
            //response3.Content = new StringContent("学生无法为他人签到", Encoding.UTF8);
            //IActionResult response4 = new IActionResult(HttpStatusCode.NotFound);
            //response3.Content = new StringContent("不存在这个学生或班级、讨论课", Encoding.UTF8);
            //return result;
            try
            {
                ///////////////////////////////////////////////////////////////////////////////////////假装我传了经纬度
                _userService.InsertAttendanceById(classId, seminarId, long.Parse(studentId), 0, 0);
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到班级" });
            }
            catch (SeminarNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            return NoContent();
        }
    }
}
