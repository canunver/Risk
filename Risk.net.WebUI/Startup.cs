using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using Risk.net.Services.Extensions;
using Risk.net.WebUI.Classes;
using Risk.net.WebUI.Models;
using System.Collections.Generic;
using System.Globalization;
using Risk.net.WebUI.Quartz;
using Risk.net.Utilities.Functions;

namespace Risk.net.WebUI
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

            services.AddQuartz(q =>
            {
                // base quartz scheduler, job and trigger configuration
                q.UseMicrosoftDependencyInjectionScopedJobFactory();

                q.AddJobAndTrigger<BildirimSistemiMailGonderJob>(Configuration);

                //// Create a "key" for the job
                //var jobKey = new JobKey("BildirimSistemiMailGonderJob");

                //// Register the job with the DI container
                //q.AddJob<BildirimSistemiMailGonderJob>(opts => opts.WithIdentity(jobKey));

                //var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                //string BildirimSistemiMailGonder = config["Genel:BildirimSistemiMailGonderJob"];

                //// Create a trigger for the job
                //q.AddTrigger(opts => opts
                //    .ForJob(jobKey) // link to the HelloWorldJob
                //    .WithIdentity("BildirimSistemiMailGonderJob-trigger") // give the trigger a unique name
                //    .WithCronSchedule("0 0/1 * * * ?")); // run every 5 seconds

            });

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            services.AddAuthentication(IISDefaults.AuthenticationScheme);
            
            //Yardim gösterilen ekranında Html.Raw çalıştırılması için
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //Form's MultipartBodyLengthLimit
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = long.MaxValue; // <-- !!! long.MaxValue
                o.MultipartBoundaryLengthLimit = int.MaxValue;
                o.MultipartHeadersCountLimit = int.MaxValue;
                o.MultipartHeadersLengthLimit = int.MaxValue;
            });

            //Theme ayarları
            services.Configure<SmartSettings>(Arac.ConfigSectionOku("SmartSettings"));

            // Note: This line is for demonstration purposes only, I would not recommend using this as a shorthand approach for accessing settings
            // While having to type '.Value' everywhere is driving me nuts (>_<), using this method means reloaded appSettings.json from disk will not work
            services.AddSingleton(s => s.GetRequiredService<IOptions<SmartSettings>>().Value);

            //Cookie
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Database
            //services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();

            //RazorRuntimeCompilation
            services.AddRazorPages().AddRazorRuntimeCompilation();

            //Cors
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options
                 .AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader().WithExposedHeaders("Content-Disposition"));
            });

            services.AddControllersWithViews().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            //Authentication
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            //{
            //    options.LoginPath = "/Account/Login";
            //    options.LogoutPath = "/Account/Logout";
            //    options.AccessDeniedPath = "/Account/AccessDenied";
            //});

            //services.AddRazorPages();

            //Diller
            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
            services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en"),
                        new CultureInfo("tr"),
                    };
                    options.DefaultRequestCulture = new RequestCulture("tr");
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;

                });

            ////Swagger
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Risk.net.WebApi", Version = "v1" });
            //});

            //Controller construstordan User bilgisini almak için
            services.AddHttpContextAccessor();

            //Servisler
            services.LoadMyServices();

            services.AddTransient<IClaimsTransformation, ClaimsLoader>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Risk.net.WebApi v1"));
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Account/_{0}");

            app.UseCors("AllowOrigin");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization(app.ApplicationServices.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}");
                //endpoints.MapRazorPages();
            });
        }
    }
}
