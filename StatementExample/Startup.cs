using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popbill.Statement;
using ControllerDI.Services;


namespace ControllerDI.Services
{
    public class StatementInstance
    {
        //파트너 신청 후 메일로 발급받은 링크아이디(LinkID)와 비밀키(SecretKey)값 으로 변경하시기 바랍니다.
        private string linkID = "TESTER";
        private string secretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

        public StatementService statementService;

        public StatementInstance()
        {
            //전자명세서 서비스 객체 초기화
            statementService = new StatementService(linkID, secretKey);

            //연동환경 설정값, 개발용(true), 상업용(false)
            statementService.IsTest = true;
        }
    }
}

namespace StatementExample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            //전자명세서 서비스 객체 의존성 주입
            services.AddSingleton<StatementInstance>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Statement}/{action=Index}");
            });
        }
    }
}