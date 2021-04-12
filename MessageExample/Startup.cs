using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popbill.Message;


public class MessageInstance
{
    //파트너 신청 후 메일로 발급받은 링크아이디(LinkID)와 비밀키(SecretKey)값 으로 변경하시기 바랍니다.
    private string linkID = "TESTER";
    private string secretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

    public MessageService messageService;

    public MessageInstance()
    {
        //문자 서비스 객체 초기화
        messageService = new MessageService(linkID, secretKey);

        //연동환경 설정값, 개발용(true), 상업용(false)
        messageService.IsTest = true;

        //인증토큰의 IP제한기능 사용여부, 권장(true)
        messageService.IPRestrictOnOff = true;

        // 팝빌 API 서비스 고정 IP 사용여부(GA), true-사용, false-미사용, 기본값(false)
        messageService.UseStaticIP = false;

        // 로컬 시스템시간 사용 여부, true(사용), fasle(미사용) - 기본값
        messageService.UseLocalTimeYN = false;
    }
}

namespace MessageExample
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

            //문자 서비스 객체 의존성 주입
            services.AddSingleton<MessageInstance>();
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
                    template: "{controller=Message}/{action=Index}");
            });
        }
    }
}