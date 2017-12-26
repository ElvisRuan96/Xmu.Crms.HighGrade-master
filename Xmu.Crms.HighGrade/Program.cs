using System.Reflection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Xmu.Crms.Shared;
using Xmu.Crms.Services.HighGrade;

namespace Xmu.Crms.HighGrade
{
    public class Program
    {
       
            public static void Main(string[] args)
            {
                var host = BuildWebHost(args);
                host.Run();
            }


            public static IWebHost BuildWebHost(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                    .UseIISIntegration()
                    .UseStartup<Startup>()
                    .ConfigureServices(services => services.AddHighGradeSeminarService().AddHighGradeTimerService().AddHighGradeSchoolService().AddHighGradeLoginService().AddHighGradeClassService().AddHighGradeCourseService().AddHighGradeSeminarGroupService().AddHighGradeFixedGroupService().AddHighGradeGradeService().AddHighGradeUserService().AddHighGradeTopicService().AddHighGradeClassDao().AddHighGradeCourseDao().AddHighGradeGradeDao().AddHighGradeTopicDao().AddHighGradeUserDao().AddCrmsView("Mobile.HighGrade"))
                    .Build();
        }
    }