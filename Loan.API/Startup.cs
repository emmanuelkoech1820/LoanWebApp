using Apps.Core.Abstract;
using Apps.Core.Core;
using Apps.Core.Proxy;
using Apps.Core.Proxy.Abstract;
using Apps.Core.Utils;
using Apps.Data.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebApi.Helpers;
using WebApi.Middleware;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // add services to the DI container
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<HttpClientUtil>();
            services.AddDbContext<DataContext>();
            services.AddCors();
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            // services.AddSwaggerGen();
            services.AddControllers();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBankTransferManager, BankTransferManager>();
            services.AddScoped<ITransferProxy, TransferManager>();
            services.AddScoped<INyumbaniManager, NyumbaniManager>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        // configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext context)
        {
            // migrate database changes on startup (includes initial db creation)
            context.Database.Migrate();

            // generated swagger json and swagger ui middleware
            // app.UseSwagger();
            // app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "ASP.NET Core Sign-up and Verification API"));
            app.UseHttpsRedirection();
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(x => x.MapControllers());
        }
    }
}
