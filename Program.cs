namespace backend
{
    using backend.Authorization;
    using backend.Data;
    using DbUp;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            // Add custom dependency injection
            builder.Services.AddScoped<IDataRepository, DataRepository>();
            builder.Services.AddSingleton<IQuestionCache, QuestionCache>();
            builder.Services.AddMemoryCache(options =>
            {
                options.SizeLimit = 100;
            });

            // Add JWT authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["Auth0:Authority"];
                options.Audience = builder.Configuration["Auth0:Audience"];
            });

            // Add HttpClient dependency injection for making HTTP requests
            builder.Services.AddHttpClient();

            // Add authorization policy
            builder.Services.AddAuthorization(options =>
                options.AddPolicy("MustBeQuestionAuthor", policy =>
                    policy.Requirements.Add(new MustBeQuestionAuthorRequirement())));
            builder.Services.AddScoped<IAuthorizationHandler, MustBeQuestionAuthorHandler>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Apply DbUp migrations during startup
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString, null)
                .WithScriptsEmbeddedInAssembly(
                    System.Reflection.Assembly.GetExecutingAssembly()
                )
                .WithTransaction()
                .Build();

            if (upgrader.IsUpgradeRequired())
            {
                upgrader.PerformUpgrade();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
