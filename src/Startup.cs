using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenSearchDemo.Models;
using OpenSearchDemo.Services.EmployeeFts;
using OpenSearchDemo.Services.OpenSearch;

namespace OpenSearchDemo;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var connectionString = Configuration.GetConnectionString("Default");
        services
            .AddDbContext<AppDbContext>(
                options => options.UseMySql(connectionString,
                        serverVersion: ServerVersion.AutoDetect(connectionString))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
        );

        var awsConfig = Configuration.GetSection(nameof(AwsConfig)).Get<AwsConfig>();
        services.AddSingleton(awsConfig);
        services.AddScoped<IEmployeeFtsService, EmployeeFtsService>();
        services.AddScoped(typeof(IOpenSearchService<>), typeof(OpenSearchService<>));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}