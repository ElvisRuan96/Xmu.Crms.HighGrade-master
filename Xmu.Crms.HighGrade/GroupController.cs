using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Mobile.Controllers.vo;



namespace Xmu.Crms.HighGrade.Controllers
{
    public class GroupController : Controller
    {
        IFixGroupService _fixGroupService;
        ISeminarGroupService _seminarGroupService;
        ITopicService _topicService;
        IGradeService _gradeService;

        public GroupController(IFixGroupService fixGroupService, ISeminarGroupService seminarGroupService, ITopicService topicService, IGradeService gradeService)
        {
            _fixGroupService = fixGroupService;
            _seminarGroupService = seminarGroupService;
            _topicService = topicService;
            _gradeService = gradeService;
        }


        // //按小组ID获取小组详情
        // //GET:/group/{groupId}
        [Route("/group/{groupId}")]
        [HttpGet]
        public ActionResult GetGroupInfo([FromRoute]int groupId)
        {

            try
            {

                var groups = _seminarGroupService.GetSeminarGroupByGroupId(groupId);
                List<UserInfo> members = new List<UserInfo>(_seminarGroupService.ListSeminarGroupMemberByGroupId(groupId));

                return Json(new { members = members.Select(g => new { id = g.Id, name = g.Name }) });
            }

            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到小组" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        // //组长辞职
        // //PUT:/group/{groupId}/resign
        [Route("/group/{groupId}/resign")]
        [HttpPut]
        /*    public HttpResponseMessage LeaderResign(int groupId, string id)
         {
             HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
             response.Content = new StringContent("成功", Encoding.UTF8);
             return response;
         }   */
        public ActionResult ResignLeaderById([FromRoute]long groupId, long userId)
        {


            try
            {
                _seminarGroupService.ResignLeaderById(groupId, userId);
                return NoContent();
            }

            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到小组" });
            }
            catch (UserNotFoundException)
            {
                return StatusCode(403, new { msg = "不存在该学生" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(409, new { msg = "学生不是组长" });
            }

        }


        // //成为组长
        // //PUT:/group/{groupId}/assign
        [Route("/group/{groupId}/assign")]
        [HttpPut]
        //    public HttpResponseMessage BecomeLeader(int groupId, string id)
        public ActionResult AssignLeaderById([FromRoute]long groupId, long userId)
        {
            /*  HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
              response.Content = new StringContent("成功", Encoding.UTF8);
              return response;    */

            try
            {
                _seminarGroupService.AssignLeaderById(groupId, userId);
                return NoContent();
            }

            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到小组" });
            }
            catch (UserNotFoundException)
            {
                return StatusCode(403, new { msg = "不存在该学生" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(409, new { msg = "已经有组长了" });
            }

        }



        // //添加成员
        // //PUT:/group/{groupId}/add
        [Route("/group/{groupId}/add")]
        [HttpPut]
        /* public HttpResponseMessage AddMember(int groupId, string id)
         {
             HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
             response.Content = new StringContent("成功", Encoding.UTF8);
             return response;
         }       */
        public ActionResult InsertSeminarGroupMemberById(long userId, long groupId)
        {


            try
            {
                _seminarGroupService.InsertSeminarGroupMemberById( userId, groupId);
                return NoContent();
            }

            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到小组" });
            }
            catch (UserNotFoundException)
            {
                return StatusCode(403, new { msg = "不存在该学生" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(409, new { msg = "待添加学生已经在小组里了" });
            }

        }



        /**************************************************缺少controller*************************************************************/
        // //移除成员
        // //PUT:/group/{groupId}/remove
        [Route("/group/{groupId}/remove")]
        [HttpPut]
        /* public HttpResponseMessage RemoveMember(int groupId, string id)
         {
             HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
             response.Content = new StringContent("成功", Encoding.UTF8);
             return response;
        }   */
        /***************************************************controller缺少************************************************************/




        // //小组按ID选择话题
        // //POST:/group/{groupId}/topic
        [Route("/group/{groupId}/topic")]
        [HttpPost]
        /* public JsonResult ChooseTopic(int groupId, [FromBody]dynamic json)
         {
             JsonResult result = new JsonResult();
             var Topic = new { url = "/group/27/topic/23" };
             result.Data = Topic;
             result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
             return result;
         }  */

        public ActionResult InsertTopicByGroupId(long groupId, long topicId)
        {

            try
            {
                _seminarGroupService.InsertTopicByGroupId(groupId, topicId);
                return NoContent();
            }

            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "小组不存在" });
            }
            catch (UserNotFoundException)
            {
                return StatusCode(403, new { msg = "权限不足（不是该小组的组长）" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式或话题已满" });
            }

        }



        // //小组按ID取消话题
        // //DELETE:/group/{groupId}/topic/{topicId}
        [ Route("/group/{groupId}/topic/{topicId}")]
        [HttpDelete]
        // public HttpResponseMessage DeleteTopic(int groupId, int topicId)
        // {
        //     HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
        //     response.Content = new StringContent("成功", Encoding.UTF8);
        //     return response;
        // }

        public ActionResult DeleteTopicById(long groupId, long topicId)
        {

            try
            {
                _topicService.DeleteTopicById(groupId,topicId);
                return NoContent();
            }

            catch (TopicNotFoundException)
            {
                return StatusCode(404, new { msg = "未选择该话题" });
            }

            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式或话题已满" });
            }

        }

    

        /*********************************************缺少service**********************************************************/
        // //按小组ID获取小组的讨论课成绩
        // //GET:/group/{groupId}/grade
         [Route("/group/{groupId}/grade")]
         [HttpGet]
        /* public JsonResult GetGroupGrade(int groupId)
        // {

             var presentationGrade = new object[] { new { topicId = "257", grade = "4" }, new { topicId = "258", grade = "5" } };
             int reportGrade = 3;
             int grade = 4;
             var result = new JsonResult(new { presentationGrade, reportGrade, grade });

             return result;
         }      */
        /*********************************************缺少service**********************************************************/




        // //按ID设置小组的报告分数
        // //PUT:/group/{groupId}/grade/report
        [Route("/group/{groupId}/grade/report")]
        [ HttpPut]
        /* public HttpResponseMessage GradeReport(int groupId, int reportGrade)
         {
             HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
             response.Content = new StringContent("成功", Encoding.UTF8);
             return response;
         }      */

        public ActionResult UpdateGroupByGroupId(long seminarGroupId, int grade)
        {

            try
            {

                _gradeService.UpdateGroupByGroupId(seminarGroupId, grade);
                return NoContent();
            }


            catch (UserNotFoundException)
            {
                return StatusCode(403, new { msg = "权限不足（学生无法设置成绩、无法修改他人班级的小组）" });
            }
            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "小组不存在" });
            }

            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式或话题已满" });
            }
     
        }




        // //提交对其他小组的打分
        // //PUT:/group/{groupId}/grade/presentation/{studentId}
        [ Route("/group/{groupId}/grade/presentation/{studentId}")]
        [HttpPut]
        /* public HttpResponseMessage SubmitPresentationGrade(int groupId, string studentId,[FromBody]dynamic presentation)
         {
             HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.NoContent);
             response.Content = new StringContent("成功",Encoding.UTF8);
             return response;
         }      */

        public ActionResult InsertGroupGradeByUserId(long topicId, long userId, long groupId, int grade)
        {

            try
            {
                _gradeService.InsertGroupGradeByUserId( topicId,userId,groupId, grade);
                return NoContent();
            }


            catch (GroupNotFoundException)
            {
                return StatusCode(404, new { msg = "小组不存在" });
            }

            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式或话题已满" });
            }
            catch (InvalidOperationException)
            {
                return StatusCode(409, new { msg = "已评分，不能重复评分" });
            }
        }
        

    }
}
