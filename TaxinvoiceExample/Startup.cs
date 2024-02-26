/*
업데이트 일자 : 2024-02-26
연동 기술지원 연락처 : 1600 - 9854
연동 기술지원 이메일 : code @linkhubcorp.com

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
using Popbill.Taxinvoice;

public class TaxinvoiceInstance
{
    // 링크아이디
    private string linkID = "TESTER";
    // 비밀키
    private string secretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

    public TaxinvoiceService taxinvoiceService;

    public TaxinvoiceInstance()
    {
        // 세금계산서 서비스 객체 초기화
        taxinvoiceService = new TaxinvoiceService(linkID, secretKey);

        // 연동환경 설정, true-테스트, false-운영(Production), (기본값: true)
        taxinvoiceService.IsTest = true;

        // 인증토큰 IP 검증 설정, ture-사용, false-미사용, (기본값: true)
        taxinvoiceService.IPRestrictOnOff = true;

        // 통신 고정 IP, true-사용, false-미사용, (기본값: false)
        taxinvoiceService.UseStaticIP = false;

        // 로컬시스템 시간 사용여부, true-사용, false-미사용, (기본값: true)
        taxinvoiceService.UseLocalTimeYN = true;

    }
}

namespace TaxinvoiceExample
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

            //세금계산서 서비스 객체 의존성 주입
            services.AddSingleton<TaxinvoiceInstance>();
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
                    template: "{controller=Taxinvoice}/{action=Index}");
            });
        }
    }
}