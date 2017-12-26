using Xmu.Crms.Shared.Service;
using Xmu.Crms.OurServices.HighGrade;
namespace Microsoft.Extensions.DependencyInjection
{
    public static class HighGradeExtensions
    {
        // 为每一个你写的Service写一个这样的函数，把 UserService 替换为你实现的 Service
        //public static IServiceCollection AddHighGradeLoginService(this IServiceCollection serviceCollection)
        //{
        //    return serviceCollection.AddScoped<ILoginService, LoginService>();
        //}
        public static IServiceCollection AddHighGradeSeminarService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ISeminarService, SeminarService>();
        }
        public static IServiceCollection AddHighGradeLoginService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ILoginService, LoginService>();
        }
        public static IServiceCollection AddHighGradeSchoolService(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddScoped<ISchoolService, SchoolService>();
        }
    }
}