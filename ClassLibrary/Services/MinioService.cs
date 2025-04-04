using ClassLibrary.Interfaces;
using Minio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Services
{
    public class MinioService : IMinioService
    {
        private readonly IMinioClient _minioClient;
        private const string BUCKET_NAME = "medical-files";

        public MinioService()
        { 
            _minioClient = new MinioClient()
                .WithCredentials("rpetric", "Renato55?")
                .WithEndpoint("localhost:9000")
                .Build();
        }
        public async Task DeleteObject(string objectId)
        {
            await _minioClient.RemoveObjectAsync(
                new Minio.DataModel.Args.RemoveObjectArgs()
                .WithBucket(BUCKET_NAME)
                                   .WithObject(objectId));   
        }

        public async Task<MinioObjectResponse> GetObjectResponse(string objectId)
        {
            MemoryStream memoryStream = new MemoryStream();
            var objectResponse = await _minioClient.GetObjectAsync(
                new Minio.DataModel.Args.GetObjectArgs()
                .WithBucket(BUCKET_NAME)
                .WithObject(objectId)
                .WithCallbackStream(stream =>
                { 
                    stream.CopyTo(memoryStream);
                })
                );
            memoryStream.Position = 0;

            return new()
            {
                Data = memoryStream,
                ContentType = objectResponse.ContentType
            };
        }

        public async Task<string> PutObject(Stream data, string fileName, string contentType, long size)
        {
            string objectId = $"{Guid.NewGuid().ToString()}_{fileName}";

            await _minioClient.PutObjectAsync(
                new Minio.DataModel.Args.PutObjectArgs()
                .WithBucket(BUCKET_NAME)
                .WithObjectSize(size)
                .WithContentType(contentType)
                .WithObject(objectId)
                .WithStreamData(data)
                );

            return objectId;
        }
    }
}
