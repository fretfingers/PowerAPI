using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;

using PowerAPI.Data.IRepository;
using PowerAPI.Service.Helper;
using PowerAPI.Service.Clients;
using PowerAPI.Helper;
using PowerAPI.Data.Models;
using PowerAPI.Data.POCO;
using PowerAPI.Hubs;
using PowerAPI.Middleware;
using PowerAPI.Service.IdentityLibrary;
using PowerAPI.SubscribeTableDependencies;
using PowerAPI.MiddlewareExtensions;

namespace PowerAPI
{
    public class Startup
    {
        readonly CustomConfigurationProvider configurationProvider;
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            this.configurationProvider = new CustomConfigurationProvider(hostingEnvironment);
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddIdentityCore<CustomIdentityUser>()
                .AddDefaultTokenProviders();


            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://192.168.2.105", "http://103.231.46.128/26", "https://103.231.46.128/26", "https://192.168.2.105", "http://localhost:4200", "https://localhost:3000", "http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });


                options.AddPolicy("AllowAll",builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed(hostName => true);
                });
            });


            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddControllers();
            services.AddSerilog((services, lc) => lc
                                                    .ReadFrom.Configuration(Configuration)
                                                    .ReadFrom.Services(services)
                                                    .Enrich.FromLogContext()
                                                    .WriteTo.Console()
                                                    );

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSignalR();

            services.AddDevExpressControls();
            //services.AddScoped<ReportStorageWebExtension, Reports>();





            services.ConfigureReportingServices(configurator =>
            {
                configurator.ConfigureReportDesigner(reportDesigner =>
                {
                    reportDesigner.RegisterDataSourceWizardConnectionStringsProvider<CustomSqlDataSourceWizardConnectionStringsProvider>();
                    reportDesigner.RegisterDataSourceWizardConfigurationConnectionStringsProvider(configurationProvider.GetReportDesignerWizardConfigurationSection());

                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
                {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
                configurator.UseAsyncEngine();
            });

            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(x => x.First());
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "PowerApi",
                    Description = "RESTful service for powerful integration with Power Enterprise ERP",
                    TermsOfService = new Uri("https://powersoft-solutions.org"),
                    Contact = new OpenApiContact
                    {
                        Name = "Powersoft",
                        Email = "sales@powersoft-solutions.org",
                        Url = new Uri("https://powersoft-solutions.org"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Open Api",
                        Url = new Uri("https://powersoft-solutions.org"),
                    }
                });


                var filePath = Path.Combine(System.AppContext.BaseDirectory, "PowerAPI.xml");
                c.IncludeXmlComments(filePath);

                c.AddSignalRSwaggerGen(_ =>
                    {
                        _.UseHubXmlCommentsSummaryAsTagDescription = true;
                        _.UseHubXmlCommentsSummaryAsTag = true;
                        _.UseXmlComments(filePath);
                    });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter 'Bearer [jwt]'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                var scheme = new OpenApiSecurityScheme { 
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, Array.Empty<string>() } });
            });


            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            var baseImageUrlSection = Configuration.GetSection("BaseImageUrl");

            services.Configure<AppSettings>(appSettingsSection);
            services.Configure<BaseImageUrl>(baseImageUrlSection);


            var secret = Configuration["AppSettings:Secret"] ?? throw new InvalidOperationException("Secret not configured");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = new TimeSpan(0, 0, 5)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });


            services.AddSingleton<IIdGenerator, IdGenerator>();
            services.AddSingleton<DashboardHub>();
            services.AddSingleton<SubscribeOrderHeaderDependency>();

            services.AddScoped<IAppraisal, Appraisal>();
            services.AddScoped<IAttendance, Attendance>();
            services.AddScoped<IEmployee, Employee>();
            services.AddScoped<ILeave, Leave>();
            services.AddScoped<ILoan, Loan>();
            services.AddScoped<IRequisition, Requisition>();
            services.AddScoped<IReports, Reports>();
            services.AddScoped<ISetup, Setup>();
            services.AddScoped<ICustomer, Customer>();
            services.AddScoped<ISales, Sales>();
            services.AddScoped<IPos, Pos>();
            services.AddScoped<IPos, Pos>();
            services.AddScoped<IApiAuthService, ApiAuthService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAppShellService, AppShellService>();
            services.AddScoped<IBanks, Banks>();
            services.AddScoped<IInventory, Inventory>();
            services.AddScoped<IStock, Stock>();
            services.AddScoped<IFixedAssets, Service.Clients.FixedAssets>();
            services.AddScoped<IProjects, Service.Clients.Projects> ();
            services.AddScoped<IGeneralLedger, GeneralLedger>();

            services.AddTransient<IPasswordHasher<CustomIdentityUser>, CustomPasswordHasher>();
            services.AddTransient<IUserStore<CustomIdentityUser>, CustomUserStore>();


            //services.AddDbContext<ReportDbContext>(options
            //    => options.UseSqlServer(Configuration.GetConnectionString("Enterprise")));

            services.AddDbContext<EnterpriseContext>(options
                => options.UseSqlServer(Configuration.GetConnectionString("Enterprise")));


        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        ///public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ReportDbContext db)
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var connectionString = Configuration.GetConnectionString("Enterprise");
            //db.InitializeDatabase();
            //var contentDirectoryAllowRule = DirectoryAccessRule.Allow(new DirectoryInfo(Path.Combine(env.ContentRootPath,"..","Content")).FullName);
            //AccessSettings.ReportingSpecificResources.TrySetRules(contentDirectoryAllowRule, UrlAccessRule.Allow());
            //DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.DataBindingMode = DevExpress.XtraReports.UI.DataBindingMode.Expressions;


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./v1/swagger.json", "PowerApi v1");
            });


            app.UseSerilogRequestLogging();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            // Shows UseCors with CorsPolicyBuilder.
            app.UseCors("AllowAll");

            app.UseDevExpressControls();
            DevExpress.DataAccess.DefaultConnectionStringProvider.AssignConnectionStrings(() => configurationProvider.GetGlobalConnectionStrings(connectionString));

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ApiTokenValidateMiddleware>();
            app.UseMiddleware<TenantCredentialsMiddleware>();

            // custom jwt auth middleware
            //app.UseMiddleware<JwtMiddleware>();
            //var authorizationProvider = new AuthorizationProvider();


            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<DashboardHub>("/hubs/DashboardHub");
            });
 

            app.UseSqlTableDependency<SubscribeOrderHeaderDependency>(connectionString);

        }
    }
}
