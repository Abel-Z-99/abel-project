using Application;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.OpenApi.Models;

namespace WebApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(options =>
			{
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Description = "在此输入 JWT：直接粘贴 token，或 \"Bearer \" 前缀 + token。",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header
				});
				options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						Array.Empty<string>()
					}
				});
			});

			// Custom services (Application + Infrastructure)
			builder.Services
				.AddInfrastructure(builder.Configuration)
				.AddApplication();

			var app = builder.Build();

			var seedEnabled = builder.Configuration.GetValue<bool>("Seed:Enabled");
			if (seedEnabled)
			{
				using var scope = app.Services.CreateScope();
				var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				DbSeeder.SeedAsync(db).GetAwaiter().GetResult();
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI(options =>
				{
					options.EnablePersistAuthorization();
				});
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseAuthentication();
			app.UseAuthorization();

			
			app.MapControllers();

			app.Run();
		}
	}
}
