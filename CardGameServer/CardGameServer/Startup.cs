using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGameServer.Controllers;
using CardGameServer.Hubs;
using CardGameServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace CardGameServer
{
    public class Startup
    {
        readonly string AllowAllCorsOrigins = "_allowAllCorsOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowAllCorsOrigins,
                    builder =>
                    {
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowAnyOrigin();
                    });
            });

            services.AddControllers();
            services.AddSignalR();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "CardGameServer", Version = "v1"});
            });

            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<IRoomService, RoomService>();
            services.AddSingleton<MouselGameService, MouselGameService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CardGameServer v1"));
            }

            app.UseRouting();

            app.UseCors(AllowAllCorsOrigins);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<LobbyHub>("/lobbyhub");
                endpoints.MapHub<IndexHub>("/indexhub");
                endpoints.MapHub<MouselHub>("/mouselhub");
            });
        }
    }
}