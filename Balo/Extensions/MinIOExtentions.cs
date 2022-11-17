using Minio;

namespace Balo.Extensions
{
    public class MinIOExtentions
    {
        public static MinioClient Instance()
        {
            var endpoint = "202.78.227.48";
            var accessKey = "5oHAVJpZY39VfJQW";
            var secretKey = "uDV7M3qjruSeepoVRtayM2Mj1JHxPnnP";
            var bucketName = "bucket";
            MinioClient minio = new MinioClient()
                                        .WithEndpoint(endpoint)
                                        .WithCredentials(accessKey, secretKey)
                                        .WithSSL(false)
                                        .Build();
            return minio;
        }
    }
}
