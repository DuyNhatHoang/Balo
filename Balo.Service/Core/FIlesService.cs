using Balo.Data.DataAccess;
using Balo.Data.ViewModels;
using Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Minio;
using Minio.Exceptions;
using System.Reactive.Linq;
using Volo.Abp.BlobStoring;

namespace Balo.Service.Core
{

    public interface IFilesService
    {
        Task UploadFileMinIoAsync(IFormFile file, MinioClient minioClient);
        Task<ResultModel>  GetFileMinIoAsync(MinioClient minioClient);
    }
    public class FilesService : IFilesService
    {
        public async Task<ResultModel> GetFileMinIoAsync(MinioClient minioClient)
        {
            var resultModel = new ResultModel();
            try
            {
                var beArgs = new BucketExistsArgs()
                  .WithBucket("bucket");
                bool found = await minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found) throw new Exception($"Buckect  doesn't exists");
                var files = await minioClient.ListObjectsAsync("bucket", "").ToList();
                //return Newtonsoft.Json.JsonConvert.SerializeObject(files);
                resultModel.Succeed = true;
                resultModel.Data = files;

            }
            catch ( Exception e)
            {
                resultModel.Succeed = false;
                resultModel.ErrorMessage = e.Message;
            }
            return resultModel;
           
            
        }

        public async Task UploadFileMinIoAsync(IFormFile file, MinioClient minioClient)
        {
            var bucketName = "bucket";
            var location = "us-east-1";
            var objectName = "Vocabulary.docx";
            var filePath = "C:\\data\\ielts\\Vocabulary.docx";
            var contentType = "application/docx";
     
            {

                //FileUpload.Run(minio).Wait();

                // Make a bucket on the server, if not already present.
                var beArgs = new BucketExistsArgs()
                  .WithBucket(bucketName);
                bool found = await minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }
                // Upload a file to bucket.
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithFileName(filePath)
                    .WithContentType(contentType);
                await minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                Console.WriteLine("Successfully uploaded " + objectName);
            }
            
        }
    }

}
