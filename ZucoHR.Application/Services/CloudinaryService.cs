using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Size = SixLabors.ImageSharp.Size;
//using static System.Net.Mime.MediaTypeNames;
using CloudinaryDotNet;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using ZucoHR.Application.Interfaces;

namespace ZucoHR.Application.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private static readonly HashSet<string> AllowedImageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".pdf" };
        private static readonly HashSet<string> AllowedMimeTypes = new HashSet<string> { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/tiff", "image/pdf" };

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<ImageUploadResult> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0 || !AllowedImageExtensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            {
                throw new ArgumentException("Invalid file type or MIME type.");
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                using (var image = await Image.LoadAsync(memoryStream))
                {
                    int maxWidth = 800, maxHeight = 800;
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(maxWidth, maxHeight), Mode = ResizeMode.Max }));

                    using (var resizedMemoryStream = new MemoryStream())
                    {
                        await image.SaveAsJpegAsync(resizedMemoryStream);
                        resizedMemoryStream.Position = 0;

                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.FileName, resizedMemoryStream),
                            UseFilename = true,
                            UniqueFilename = false,
                            Overwrite = true
                        };

                        return await _cloudinary.UploadAsync(uploadParams);
                    }
                }
            }
        }
    }
}
