/*
업데이트 일자 : 2024-10-31
연동 기술지원 연락처 : 1600 - 9854
연동 기술지원 이메일 : code@linkhubcorp.com

<테스트 연동개발 준비사항>
1) API Key 변경 (연동신청 시 메일로 전달된 정보)
- LinkID : 링크허브에서 발급한 링크아이디.
- SecretKey : 링크허브에서 발급한 비밀키.

2) SDK 환경설정 옵션 설정
- IsTest : 연동환경 설정, true-테스트, false-운영(Production), (기본값: true)
- IPRestrictOnOff : 인증토큰 IP 검증 설정, ture-사용, false-미사용, (기본값: true)
- UseStaticIP : 통신 고정 IP, true-사용, false-미사용, (기본값: false)
- UseLocalTimeYN : 로컬시스템 시간 사용여부, true-사용, false-미사용, (기본값: true)
*/
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popbill.BizInfoCheck;

public class BizInfoCheckInstance
{
    // 링크아이디
    private string linkID = "TESTER";
    // 비밀키
    private string secretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

    public BizInfoCheckService bizInfoCheckService;

    public BizInfoCheckInstance()
    {
        // 기업정보조회 서비스 객체 초기화
        bizInfoCheckService = new BizInfoCheckService(linkID, secretKey);

        // 연동환경 설정, true-테스트, false-운영(Production), (기본값: true)
        bizInfoCheckService.IsTest = true;

        // 인증토큰 IP 검증 설정, ture-사용, false-미사용, (기본값: true)
        bizInfoCheckService.IPRestrictOnOff = true;

        // 통신 고정 IP, true-사용, false-미사용, (기본값: false)
        bizInfoCheckService.UseStaticIP = false;

        // 로컬시스템 시간 사용여부, true-사용, false-미사용, (기본값: true)
        bizInfoCheckService.UseLocalTimeYN = true;
    }
}

namespace BizInfoCheckExample
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

            //기업정보조회 서비스 객체 의존성 주입
            services.AddSingleton<BizInfoCheckInstance>();
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
                    template: "{controller=BizInfoCheck}/{action=Index}");
            });
        }
    }
}
