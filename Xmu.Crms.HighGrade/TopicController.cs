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


namespace Xmu.Crms.HighGrade.Controllers
{
    [Route("")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        private readonly ISeminarGroupService _seminarGroupService;
        private readonly JwtHeader _header;
        public TopicController(ITopicService topicService,ISeminarGroupService seminarGroupService, JwtHeader header)
        {
            _topicService = topicService;
            _seminarGroupService = seminarGroupService;
        }

        //按ID获取话题
        //GET: /topic/{topicId}
        [Route("topic/{topicId}")]
        [HttpGet]
        public IActionResult GetTopic(int topicId)
        {
            //var result = new IActionResult(new { id = 257, serial = "A", name = "领域模型与模块", description = "Domain model与模块划分", groupLimit = 5, groupMemberLimit = 6, groupLeft = 2 });
            //return result;
            try
            {
                var topic = _topicService.GetTopicByTopicId(topicId);
                return Json(new { id = topic.Id, name = topic.Name, serial = topic.Serial,description =topic.Description,groupLimit=topic.GroupNumberLimit, groupMemberLimit=topic.GroupStudentLimit});
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到话题" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }

        //按ID修改话题
        //PUT:/topic/{topicId}
        [Route("topic/{topicId}")]
        [HttpPut]
        public IActionResult ModifyTopic(int topicId, [FromBody]Topic topic)
        {
            //IActionResult response = new IActionResult(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //return response;
            try
            {
                _topicService.UpdateTopicByTopicId(topicId,topic);
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到话题" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            //catch (ArgumentException)
            //{
            //    return StatusCode(403, new { msg = "用户权限不足" });
            //}
            return NoContent();
        }

        //按Id删除话题
        //DELETE:/topic/{topicId}
        [Route("topic/{topicId}")]
        [HttpDelete]
        public IActionResult DeleteTopic(int topicId)
        {
            //IActionResult response = new IActionResult(HttpStatusCode.NoContent);
            //response.Content = new StringContent("成功", Encoding.UTF8);
            //return response;
            try
            {
                _topicService.DeleteTopicByTopicId(topicId);
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到话题" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
            //catch (ArgumentException)
            //{
            //    return StatusCode(403, new { msg = "用户权限不足" });
            //}
            return NoContent();
        }

        //按话题ID获取选择了该话题的小组
        //GET: /topic/{topicId}/group
        [Route("topic/{topicId}/group")]
        [HttpGet]
        public IActionResult GetGroup(int topicId)
        {
            //var result = new IActionResult(new object[]
            //{
            //    new {id =23,name ="1A1"},
            //    new {id =26,name ="2A2"}
            //});

            //return result;
            try
            {
                var group = _seminarGroupService.ListGroupByTopicId(topicId);
                return Json(group.Select(c => new { id = c.Id, name = c.Name, }));
            }
            catch (ClassNotFoundException)
            {
                return StatusCode(404, new { msg = "未找到讨论课" });
            }
            catch (ArgumentException)
            {
                return StatusCode(400, new { msg = "错误的ID格式" });
            }
        }
    }
}