using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

namespace AuhtInTeams
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            // 1 - Méthode 

            services.AddMicrosoftIdentityWebApiAuthentication(Configuration)
               .EnableTokenAcquisitionToCallDownstreamApi()
               .AddInMemoryTokenCaches();


            // 2 - Méthode 
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));
            

            //services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            //{

            //    options.TokenValidationParameters.ValidateAudience = true;
            //    options.TokenValidationParameters.ValidateIssuer = true;
            //    // Recupère la liste des Issuers autorisés
            //    string[] issuers = ValideIssuers.GetListIssuers();
            //    options.TokenValidationParameters.ValidIssuers = issuers;
            //    options.TokenValidationParameters.ValidateLifetime = true;
            //    options.TokenValidationParameters.SaveSigninToken = true;

            //    options.Events = new JwtBearerEvents
            //    {
            //        OnAuthenticationFailed = context =>
            //        {
            //            return Task.CompletedTask;
            //        },
            //        OnTokenValidated = context =>
            //        {
            //            var accessToken = context.SecurityToken as JwtSecurityToken;
            //            if (accessToken != null)
            //            {
            //                ClaimsIdentity identity = context.Principal.Identity as ClaimsIdentity;
            //                if (identity != null)
            //                {
            //                    identity.AddClaim(new Claim("access_token", accessToken.RawData));
            //                }
            //            }
            //            return Task.CompletedTask;
            //        }

            //    };

            //});

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuhtInTeams", Version = "v1" });
            });
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuhtInTeams v1"));
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
