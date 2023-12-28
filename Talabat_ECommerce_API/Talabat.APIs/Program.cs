using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.MiddleWares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;

namespace Talabat.APIs
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region Configure Service Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();//Configurition specified for Swagger
			builder.Services.AddSwaggerGen(); //Configurition specified for Swagger
			builder.Services.AddDbContext<StoreContext>(Options =>
			{
				Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});


			builder.Services.AddDbContext<AppIdentityDbContext>(Options =>
			{
				Options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
			});

			//DbContext Of Identity(Security)
			builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
			{
				var Connection = builder.Configuration.GetConnectionString("RedisConnection");
				return ConnectionMultiplexer.Connect(Connection);

			});

			builder.Services.AddApplicationServices();
			//Used To Add Identity Services
			builder.Services.AddIdentityServices(builder.Configuration);
			builder.Services.AddCors(Options =>
			{
				Options.AddPolicy("MyPolicy", options =>
				{
					options.AllowAnyHeader();
					options.AllowAnyMethod();
					options.WithOrigins(builder.Configuration["FrontBaseUrl"]);
				});
			});

			#endregion
			var app = builder.Build();
			

			#region Update-Database

			using var Scope = app.Services.CreateScope();
			// Group Of Services LifeTime Scooped
			var Services = Scope.ServiceProvider;
			// Services Its Self
			var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();//Will create object from class implement interface ILoggerFactory
			try
			{
				var dbContext = Services.GetRequiredService<StoreContext>();
				// ASK CLR For Creating Object From DbContext Explicitly
				await dbContext.Database.MigrateAsync(); // Update-Database

				// ASK CLR For Creating Object From DbContext for Identity Explicitly
				var IdentityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
				await IdentityDbContext.Database.MigrateAsync();

				var UserManager = Services.GetRequiredService<UserManager<AppUser>>();
				await AppIdentityDbContextSeed.SeedUserAsync(UserManager);
				await StoreContextSeed.SeedAsync(dbContext);

			}
			catch (Exception ex)
			{
				var Logger=LoggerFactory.CreateLogger<Program>();
				Logger.LogError(ex, "An Error Occured During Appling The Migration");
			}


			#endregion

			#region Configure-Configure the HTTP request pipeline.
			
			if (app.Environment.IsDevelopment())
			{
				app.UseMiddleware<ExceptionMiddleWare>();
				app.UseSwaggerMiddlewares();
			}
			app.UseStatusCodePagesWithRedirects("/errors/{0}");
			app.UseHttpsRedirection();
			app.UseStaticFiles();//Used To Load Images Or Videos
			app.UseCors("MyPolicy");
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers(); 
			#endregion

			app.Run();
		}
	}
}