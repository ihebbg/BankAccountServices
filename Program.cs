using BankAccountServices.Data;
using BankAccountServices.Configuration;
using Microsoft.EntityFrameworkCore;
using BankAccountServices.Services;
using BankAccountServices.Repositories;
using BankAccountServices.Repositories.Interfaces;
using BankAccountServices.Services.Interfaces;
using BankAccountServices.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Serilog;
using Amazon;
using Amazon.S3;
var builder = WebApplication.CreateBuilder(args);

// Autoriser CORS
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAngularClient",
		policy =>
		{
		    policy.AllowAnyOrigin()
				  .AllowAnyHeader()
				  .AllowAnyMethod();
		});
});

// Lecture des paramètres JWT
var jwtSettings = JwtSettings.FromConfiguration(builder.Configuration);
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "DigitalBankAccount", Version = "v1" });

	// Ajout de la définition de sécurité pour le Bearer token
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "Entrez le token JWT avec le préfixe 'Bearer '",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
	{
		new OpenApiSecurityScheme
		{
			Reference = new OpenApiReference
			{
				Type = ReferenceType.SecurityScheme,
				Id = "Bearer"   // doit correspondre au nom défini dans AddSecurityDefinition
            },
			Scheme = "Bearer",
			Name = "Authorization",
			In = ParameterLocation.Header,
		},
		new List<string>() // scopes (vide pour JWT simple)
    }
});
});
// Ajout de l'authentification avec JWT
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtSettings.Issuer,
		ValidAudience = jwtSettings.Audience,
		IssuerSigningKey = new SymmetricSecurityKey(key)
	};
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Configuration de Serilog avec un fichier de logs
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Information()                       // niveau minimum
	.WriteTo.File(
		path: "Logs/log.txt",                         // chemin du fichier
		rollingInterval: RollingInterval.Day,        // un fichier par jour
		outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}" // format
	)
	.CreateLogger();



builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseMySql(builder.Configuration.GetConnectionString("DBConnection"),
		new MySqlServerVersion(new Version(8, 0, 36)),
		mySqlOptions => mySqlOptions.EnableRetryOnFailure()));
//builder.Services.AddDbContext<AppDbContext>(options =>
//	options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IBankAccountService, BankAccountService>();
builder.Services.AddScoped<IOperationRepository, OperationRepository>();

builder.Services.AddScoped<IOperationService, OperationService>();
builder.Services.AddScoped<IJwtRepository, JwtRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton(sp => S3Settings.FromConfiguration(sp.GetRequiredService<IConfiguration>()));
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
	var s3Settings = sp.GetRequiredService<S3Settings>();
	var s3Config = new AmazonS3Config();

	if (!string.IsNullOrWhiteSpace(s3Settings.ServiceUrl))
	{
		s3Config.ServiceURL = s3Settings.ServiceUrl;
		s3Config.ForcePathStyle = s3Settings.ForcePathStyle;
	}

	if (!string.IsNullOrWhiteSpace(s3Settings.Region))
	{
		s3Config.RegionEndpoint = RegionEndpoint.GetBySystemName(s3Settings.Region);
	}

	return new AmazonS3Client(s3Config);
});
builder.Services.AddScoped<IS3StorageService, S3StorageService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Active le logging debug
builder.Logging.AddDebug();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors("AllowAngularClient");

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
	.AllowAnonymous();


try
{
	Log.Information("Starting BankAccountServices API");
	app.Run();
}
finally
{
	Log.CloseAndFlush();
}
