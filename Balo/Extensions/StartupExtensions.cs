using Balo.Data.DataAccess;
using Balo.Service.Core;
using MongoDB.Driver;
using Volo.Abp.BlobStoring.Minio;

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
           services.AddScoped<IFilesService, FilesService>();
           services.AddScoped<IColumnService, ColumnService>();
        }

        public static void ConfigCors(this IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("MyAllowSpecificOrigins",
                      builder =>
                      {
                          builder.WithOrigins("https://localhost:3000",
                              "https://myawesomesite")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      }));
        }

        public static void ConfigMongoDb(this IServiceCollection services, string connectionString, string databaseName)
        {
            services.AddSingleton<IMongoClient>(s => new MongoClient(connectionString));
            services.AddScoped(s => new AppDbContext(s.GetRequiredService<IMongoClient>(), databaseName));
        }

        public static void ConfigMinIo(this IServiceCollection services)
        {
            var endpoint = "localhost:9000";
            var accessKey = "RIJ780uH4QkOytqA";
            var secretKey = "TVDvle9tnX6C03n2YTuNFXKB4FgVDIh2";
            var bucketName = "bucket";
            services.Configure<Volo.Abp.BlobStoring.AbpBlobStoringOptions>(options =>
            {
                options.Containers.ConfigureDefault(container =>
                {
                    container.UseMinio(minio =>
                    {
                        minio.EndPoint = endpoint;
                        minio.AccessKey = accessKey;
                        minio.SecretKey = secretKey;
                        minio.BucketName = bucketName;
                    });
                });
            });
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
