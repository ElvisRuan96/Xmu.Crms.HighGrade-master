using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using static Xmu.Crms.HighGrade.Utils;
using Xmu.Crms.Mobile.Controllers.vo;


namespace Xmu.Crms.HighGrade
{
    
    [Route("")]
    [Produces("application/json")]
    public class MeController : Controller
    {
        long UserID = 1;

        private readonly CrmsContext _db;
        private IClassService classService;
        private ICourseService courseService;
        private IFixGroupService fixGroupService;
        private IGradeService gradeService;
        private ILoginService loginService;
        private ISchoolService schoolService;
        private ISeminarGroupService seminarGroupService;
        private ISeminarService seminarService;
        private ITimerService timerService;
        private ITopicService topicService;
        private IUserService userService;
        private readonly JwtHeader _header;
        private readonly ILoginService _loginService;
        private readonly IUserService _userService;

        public MeController(JwtHeader header, ILoginService loginService, IUserService userService)
        {
            _header = header;
            _loginService = loginService;
            _userService = userService;
        }

        [HttpGet("/me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetCurrentUser(UserInfo user)
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
            UserInfo me1 = _userService.GetUserByUserId(User.Id());
            Dperson me = new Dperson();
            me.Id = (int)me1.Id;
            if (me1.Type == Xmu.Crms.Shared.Models.Type.Student)
                me.Type = "student";
            if (me1.Type == Xmu.Crms.Shared.Models.Type.Teacher)
                me.Type = "teacher";
            me.Number = me1.Number;
            me.Phone = me1.Phone;
            me.Email = me1.Email;
            me.Name = me1.Name;
            if (me1.Gender == Xmu.Crms.Shared.Models.Gender.Male)
                me.Gender = "male";
            if (me1.Gender == Xmu.Crms.Shared.Models.Gender.Female)
                me.Type = "female";
            Dschool dschool = new Dschool();
            dschool.ID = (int)me1.School.Id;
            dschool.Name = me1.School.Name;
            dschool.Province = me1.School.Province;
            if (me1.Avatar != null)
                me.Avatar = me1.Avatar;
            me.School = dschool;
            return Json(me);

        }

        [HttpPut("/me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateCurrentUser([FromBody] Dperson updated)
        {
            UserInfo me1 = _userService.GetUserByUserId(User.Id());
            me1.Number = updated.Number;
            me1.Name = updated.Name;
            me1.School.Id = updated.School.ID;
            me1.School.Name = updated.School.Name;
            Console.WriteLine("\n\n" + updated.School.Name + "\n\n"); try
            {
                _userService.UpdateUserByUserId(UserID, me1);
                return NoContent();
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
        }


        [HttpPost("/signin")]
        public IActionResult SigninPassword([FromBody] UsernameAndPassword uap)
        {
            try
            {
                UserInfo user = _loginService.SignInPhone(new UserInfo { Phone = uap.Phone, Password = uap.Password });
                HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal());
                return Json(CreateSigninResult(user));
            }
            catch (PasswordErrorException)
            {
                return StatusCode(401, new { msg = "用户名或密码错误" });
            }
            catch (UserNotFoundException)
            {
                return StatusCode(404, new { msg = "用户不存在" });
            }
        }

        [HttpPost("/register")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult RegisterPassword([FromBody] Dregister my)
        {
            string phone = my.Phone;
            string password = my.Password;
            string password_confirm = my.Passwordconfirm;

            try
            {
                string temp = "";
                if (phone.Length != 11)
                    temp = "请输入11位手机号";
                else if (!password.Equals(password_confirm))
                    temp = "前后密码不一致";
                else {
                    _loginService.SignUpPhone(new UserInfo { Phone = my.Phone, Password = my.Password });
                    temp = "success";
                    
                }
                return Json(new { Message = temp });

            }
            catch (PhoneAlreadyExistsException)
            {
                return StatusCode(409, new { msg = "手机已注册" });
            }
        }

        private SigninResult CreateSigninResult(UserInfo user) => new SigninResult
        {
            Id = user.Id,
            Name = user.Name,
            Type = user.Type.ToString().ToLower(),
            Jwt = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_header,
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
                )))
        };

        [HttpPost("/upload/avatar")]
        public IActionResult UploadAvatar(IFormFile file) =>
            Created("/upload/avatar.png", new { url = "/upload/avatar.png" });

        public class UsernameAndPassword
        {
            public string Phone { get; set; }
            public string Password { get; set; }
        }

        public class SigninResult
        {
            public long Id { get; set; }

            public string Name { get; set; }

            public string Type { get; set; }

            public string Jwt { get; set; }
        }
    }
}
