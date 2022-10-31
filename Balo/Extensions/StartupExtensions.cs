using Balo.Data.DataAccess;
using Balo.Service.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Balo.Extensions
{
    public static class StartupExtensions
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
           services.AddScoped<IBoardService, BoardService>();
           services.AddScoped<IUserService, UserService>();
           services.AddScoped<IBoardInvitationService, BoardInvitationService>();
           services.AddScoped<IGroupService, GroupService>();
           services.AddScoped<ITaskService, TaskService>();
        }

        public static void ConfigCors(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("AllowAll", builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
        }

        public static void ConfigMongoDb(this IServiceCollection services, string connectionString, string databaseName)
        {
            services.AddSingleton<IMongoClient>(s => new MongoClient(connectionString));
            services.AddScoped(s => new AppDbContext(s.GetRequiredService<IMongoClient>(), databaseName));
        }

        public static void ConfigJwt(this IServiceCollection services, string key, string issuer, string audience)
        {
            //services.AddAuthentication(x =>
            //    {
            //        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    })
            //    .AddJwtBearer(jwtconfig =>
            //    {
            //        jwtconfig.SaveToken = true;
            //        jwtconfig.TokenValidationParameters = new TokenValidationParameters()
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = false,
            //            RequireSignedTokens = true,
            //            ValidIssuer = issuer,
            //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            //            ValidAudience = string.IsNullOrEmpty(audience) ? issuer : audience,
            //        };

            //    });
        }
    }
}
