using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Interfaces
{
    public interface IMinioService
    {
        public Task<MinioObjectResponse> GetObjectResponse(string objectId);
        public Task<string> PutObject(Stream data,string fileName, string contentType, long size);
        Task DeleteObject(string objectId);
    }
}
