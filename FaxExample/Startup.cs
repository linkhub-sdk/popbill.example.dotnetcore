﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popbill.Fax;


public class FaxInstance
{
    //파트너 신청 후 메일로 발급받은 링크아이디(LinkID)와 비밀키(SecretKey)값 으로 변경하시기 바랍니다.
    private string linkID = "TESTER";
    private string secretKey = "SwWxqU+0TErBXy/9TVjIPEnI0VTUMMSQZtJf3Ed8q3I=";

    public FaxService faxService;

    public FaxInstance()
    {
        //팩스 서비스 객체 초기화
        faxService = new FaxService(linkID, secretKey);

        //연동환경 설정값, 개발용(true), 상업용(false)
        faxService.IsTest = true;

        //인증토큰의 IP제한기능 사용여부, 권장(true)
        faxService.IPRestrictOnOff = true;

        // 팝빌 API 서비스 고정 IP 사용여부, true-사용, false-미사용, 기본값(false)
        faxService.UseStaticIP = false;

        // 로컬 시스템시간 사용 여부, true(사용) - 기본값, fasle(미사용)
        faxService.UseLocalTimeYN = true;
    }
}

namespace FaxExample
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

            //팩스 서비스 객체 의존성 주입
            services.AddSingleton<FaxInstance>();
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
                    template: "{controller=Fax}/{action=Index}");
            });
        }
    }
}