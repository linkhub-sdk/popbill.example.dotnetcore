using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Popbill.Taxinvoice;
using ControllerDI.Services;


namespace ControllerDI.Services
{
    public class TaxinvoiceInstance
    {
        private static TaxinvoiceService taxinvoiceService;

        public TaxinvoiceService getInstance(string linkID, string secretKey)
        {
            if (taxinvoiceService == null)
            {
                taxinvoiceService = new TaxinvoiceService(linkID, secretKey);
            }

            return taxinvoiceService;
        }
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

            //세금계산서 서비스 객체 종속성 주입
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