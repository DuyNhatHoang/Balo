
using Balo.Data.DataAccess;
using Balo.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using NSwag;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.ConfigMongoDb(builder.Configuration["AppDatabaseSettings:ConnectionString"], builder.Configuration["AppDatabaseSettings:DatabaseName"]);

builder.Services.AddControllers();

builder.Services.AddOpenApiDocument(document =>
{
    document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}."
    });

    document.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});


builder.Services.AddBusinessServices();


// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

// Register the Swagger services




var app = builder.Build();

// Configure the HTTP request pipeline.


// Create collections in Database
var appDbContext = builder.Services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<AppDbContext>();

if (appDbContext != null)
{
    appDbContext.CreateCollectionsIfNotExists();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseOpenApi();
app.UseSwaggerUi3();



app.Run();


