using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZucoHR.Application.Interfaces
{
    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadFileAsync(IFormFile file);
    }
}
