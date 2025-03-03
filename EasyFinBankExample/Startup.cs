﻿/*
업데이트 일자 : 2025-01-23
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
using Popbill.EasyFin;

public class EasyFinBankInstance
{
    // 링크아이디
    private string linkID = "TESTER";
    // 비밀키
    private string secretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

    public EasyFinBankService easyFinBankService;

    public EasyFinBankInstance()
    {
        // 계좌조회 서비스 객체 초기화
        easyFinBankService = new EasyFinBankService(linkID, secretKey);

        // 연동환경 설정, true-테스트, false-운영(Production), (기본값: true)
        easyFinBankService.IsTest = true;

        // 인증토큰 IP 검증 설정, ture-사용, false-미사용, (기본값: true)
        easyFinBankService.IPRestrictOnOff = true;

        // 통신 고정 IP, true-사용, false-미사용, (기본값: false)
        easyFinBankService.UseStaticIP = false;

        // 로컬시스템 시간 사용여부, true-사용, false-미사용, (기본값: true)
        easyFinBankService.UseLocalTimeYN = true;
    }
}


namespace EasyFinBankExample
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
            // Add framework services.
            services.AddMvc();

            // 계좌조회 서비스 Singleton Instance dependency injection
            services.AddSingleton<EasyFinBankInstance >();
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
                    template: "{controller=EasyFinBank}/{action=Index}");
            });
        }
    }
}
