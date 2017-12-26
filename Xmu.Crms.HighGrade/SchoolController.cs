using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Service;
using Xmu.Crms.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Xmu.Crms.Mobile.Controllers.vo;


namespace Xmu.Crms.HighGrade.Controllers
{
    [Route("")]
    [Produces("application/json")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SchoolController : Controller
    {
        ISchoolService _schoolService;
        public SchoolController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        //获取学校列表（按照城市查找学校）
        //GET: /school?city={city}
        [Route("/school/city={city}")]
        [HttpGet]
        public IActionResult GetSchool([FromRoute] string city)
        {  
            try
            {
                var schools = _schoolService.ListSchoolByCity(city);
                return Json(schools.Select(s => new { id = s.Id, name = s.Name, province = s.Province, city = s.City }));
            }
            catch(SchoolNotFoundException es)
            {
                return StatusCode(404, new { msg = "学校不存在" });
            }
            catch(ArgumentException ea)
            {
                return StatusCode(400, new { msg = "城市输入有误" });
            }
        }

        //添加学校
        //POST: /school
        [Route("/school")]
        [HttpPost]       
        public IActionResult AddSchool([FromBody] School school)
        {
            try
            {
                var sid = _schoolService.InsertSchool(school);
                return Json(new { id = sid });
            }
            catch(Exception e)
            {
                return StatusCode(409, new { msg = "学校重复" });
            }
        }

        //获取省份列表
        //GET: /school/province
        [Route("/school/province")]
        [HttpGet]
        public IActionResult GetProvince()
        {
            try
            {
                var provinces = _schoolService.ListProvince();
                return Json(provinces);
            }
            catch(Exception e)
            {
                return StatusCode(404, new { msg = "err" });
            }
            
        }

        //获取城市列表
        //GET: /school/city?province={province}
        [Route("/school/city/province={province}")]
        [HttpGet]
        public IActionResult GetCity([FromRoute] string province)
        {
            try
            {
                var cities = _schoolService.ListCity(province);
                return Json(cities);
            }
            catch(Exception e)
            {
                return StatusCode(404, new { msg = "err" });
            }
        }
     
    }
}
