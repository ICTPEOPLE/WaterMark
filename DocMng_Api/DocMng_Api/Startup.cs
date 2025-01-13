
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace DocMng_Api
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
            //services.AddControllers();
            // CodePagesEncodingProvider 등록
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            #region swagger에서 enum값을 string으로 표현
            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    options.SerializerSettings.Converters.Add(new StringEnumConverter()));
                    //.AddMvcOptions(option =>
                    //{
                    //    options.Filters.Add<LoggingActionFilter>();
                    //})

            services.AddSwaggerGenNewtonsoftSupport(); 
            #endregion

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
            });

            // infoDign Core Migration
            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            services.AddSingleton<DocMng_Api.Configuration.IConfiguration, DocMng_Api.Configuration.Configuration>();

            //Add http client 서비스 등록
            services.AddHttpClient();

            // Api Explorer를 사용할 때, 버전 포맷도 같이 지정한다. 예시는 v2.0.1 처럼 major.minor.build 포맷을 의미한다.
            services.AddVersionedApiExplorer(options => { 
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            #region File Size
            services.Configure<IISServerOptions>(options =>
                {
                    options.MaxRequestBodySize = int.MaxValue;
                });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            }); 
            #endregion

            // 버전 지정과 관련된 옵션이다. 클라이언트에서 요청 API의 버전을 명시하지 않을때 사용할 기본버전과 어떤 방식(여기서는 헤더)으로 명시하는지 정하고 있다.
            services.AddApiVersioning(config =>
            {
                config.ReportApiVersions = true;
                config.ApiVersionReader = new HeaderApiVersionReader("api-version");
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddSwaggerGen(s =>
            {
                var scopeFactory = services
                    .BuildServiceProvider()
                    .GetRequiredService<IServiceScopeFactory>();

                using var scope = scopeFactory.CreateScope();
                var serviceProvider = scope.ServiceProvider;
                var descriptionProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in descriptionProvider.ApiVersionDescriptions)
                {
                    s.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Version = description.GroupName,
                        Title = "Documenet Management API " + description.GroupName.ToUpperInvariant(),
                        Description = " Documenet Management API Swagger Surface @@@",
                        Contact = new OpenApiContact
                        {
                            Name = "ICTPEOPLE",
                            Email = "service@ictpeople.co.kr",
                            Url = new Uri("http://ictpeople.co.kr/")
                        },
                    });
                }
                
                #region Swagger Enum Default Value
                s.UseInlineDefinitionsForEnums(); 
                #endregion

                #region Swagger Summary Display
                // project.csproj 추가
                //< PropertyGroup >
                //  < GenerateDocumentationFile > true </ GenerateDocumentationFile >
                //  < NoWarn >$(NoWarn); 1591 </ NoWarn >
                //</ PropertyGroup >
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                s.IncludeXmlComments(xmlPath);
                #endregion

                #region Schema에 Enum Summary 표시
                s.SchemaFilter<Swagger.EnumTypesSchemaFilter>(xmlPath); 
                #endregion
                s.OperationFilter<Swagger.SwaggerParameterAttributeFilter>();
                s.OperationFilter<Swagger.SwaggerDefaultValues>();

                s.ResolveConflictingActions(a => a.First());
            });

            services.AddGrpc(option =>
            {
                option.MaxSendMessageSize = null;
                option.MaxReceiveMessageSize = null;
            });

            services.AddCors(o => o.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
            }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logger ,IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartEnter_Api v1"));
            }
            logger.AddLog4Net("log4net.config");

            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartEnter_Api v1");
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", "DocMng_Api " + description.GroupName.ToUpperInvariant());
                    //c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", "SmartEnter_Api " + description.GroupName.ToUpperInvariant());
                    //c.RoutePrefix = "swagger/"+ description.GroupName;
                }
            });


            #region
            app.UseStatusCodePages();
            #endregion

            app.UseSession();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseGrpcWeb();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
