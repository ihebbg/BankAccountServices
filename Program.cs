using BankAccountServices.Data;
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
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Lecture des paramètres JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
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
		ValidIssuer = jwtSettings["Issuer"],
		ValidAudience = jwtSettings["Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(key)
	};
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseMySql(builder.Configuration.GetConnectionString("DBConnection"),
		new MySqlServerVersion(new Version(8, 0, 36))));


builder.Services.AddScoped<ICustomerRepository,CustomerRepository>();
builder.Services.AddScoped<ICustomerService,CustomerService>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<IBankAccountService, BankAccountService>();
builder.Services.AddScoped<IOperationRepository, OperationRepository>();

builder.Services.AddScoped<IOperationService, OperationService>();
builder.Services.AddScoped<IJwtService, JwtService>();
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
