using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace DateingApp.FileStorage
{
    public class FileHelper : IFileHelper
    {
        private readonly IOptions<CloudinarySetting> _config;
        private Cloudinary _cloudinary;

        public FileHelper(IOptions<CloudinarySetting> config)
        {
            _config = config;

            Account acc = new Account(
                cloud: _config.Value.CloudName,
                apiKey: _config.Value.ApiKey,
                apiSecret: _config.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(acc);
        }


        public ImageUploadResult upload(ImageUploadParams uploadParams)
        {
            return _cloudinary.Upload(uploadParams);
        }

        public async Task<bool> DeleteFile(string deletePublicId)
        {
            var deleteParams = new DeletionParams(deletePublicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result != null && !string.IsNullOrEmpty(result.Result) && result.Result.ToLower() == "ok";
        }
    }
}
