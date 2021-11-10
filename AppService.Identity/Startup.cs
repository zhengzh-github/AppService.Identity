using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;

namespace AppService.Identity
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
            IdentityModelEventSource.ShowPII = true;
            services.AddControllers();
            services.AddAuthorization();
            services.AddAuthentication("Bearer")
            .AddCookie("Cookies")
            .AddIdentityServerAuthentication(options =>
            {
                //options.Authority = "http://slb.com:8080";    //����Identityserver����Ȩ��ַ nginx ��������ַ ���Է���
                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;           //����Ҫhttps  false  
                options.ApiName = "api";  //api��name����Ҫ��config��������ͬ
            });

            /**********************************
             * upstream slb.com {
		    server 127.0.0.1:5002;
		    server 127.0.0.1:5003;
		    }

            server {
            listen       8080;
            server_name  slb.com;

            #charset koi8-r;

            #access_log  logs/host.access.log  main;

            location / {
			    proxy_set_header Host $host:$server_port; #���ִ���������Ѷ˿ڸ㶪��
			    proxy_set_header X-Real-IP $remote_addr;
			    proxy_set_header REMOTE-HOST $remote_addr;
			    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
			
			    proxy_pass http://slb.com;
                root   html;
                index  index.html index.htm;
        }
             **********************************/

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //var fordwardedHeaderOptions = new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //};
            //fordwardedHeaderOptions.KnownNetworks.Clear();
            //fordwardedHeaderOptions.KnownProxies.Clear();

            //app.UseForwardedHeaders(fordwardedHeaderOptions);

            app.UseRouting();
           
            app.UseAuthentication();//��Ȩ����

            app.UseAuthorization();//��֤����
            app.UseCors("any");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
