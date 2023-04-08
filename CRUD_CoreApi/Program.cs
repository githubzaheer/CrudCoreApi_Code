using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Porduct_Management.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionstring = "Data Source=coreapidb1.c4tvlfdjcf2e.us-east-1.rds.amazonaws.com,1433;Initial Catalog=coreapidb;User ID=admin;Password=admin123456;TrustServerCertificate=True;";
builder.Services.AddDbContext<ApplicationDbContext>
    (options => options.UseSqlServer(connectionstring));//(builder.Configuration.GetConnectionString("ConnectionStrings:"))) ;
//ConfigurationManager configuration = builder.Configuration;
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})

//// Adding Jwt Bearer
//.AddJwtBearer(options =>
//{
//    options.SaveToken = true;
//    options.RequireHttpsMetadata = false;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidAudience = configuration["JWT:Audience"],
//        ValidIssuer = configuration["JWT:Issuer"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
//    };
//});
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
                          policy =>
                          {
                              policy.WithOrigins("http://crudcoreapi-env.eba-iwg8pybw.us-east-1.elasticbeanstalk.com/")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod();
                          });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
     .AddJwtBearer(options => {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = "http://localhost:34355",//builder.Configuration["Jwt:Issuer"],
             ValidAudience = "http://localhost:34355",//builder.Configuration["Jwt:Audience"],
             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DhftOS5uphK3vmCJQrexST1RsyjZBjXWRgJMFPU4")) //builder.Configuration["Jwt:Key"]
         };
     });
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders(); // optional (clear providers already added)
    logging.AddFile("Logs/Log-{Date}.txt");
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//using (var scope = app.Services.CreateScope())

//{

//    var loggerFactory = scope.ServiceProvider.GetRequiredService(typeof(ILoggerFactory)); 
//    loggerFactory.AddFile("Logs/mylog-{Date}.txt");

//}

app.MapGet("/", () => "Hello World!");

app.Run();
