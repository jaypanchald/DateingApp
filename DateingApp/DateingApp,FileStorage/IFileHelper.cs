using CloudinaryDotNet.Actions;
using System.Threading.Tasks;

namespace DateingApp.FileStorage
{
    public interface IFileHelper
    {
        ImageUploadResult upload(ImageUploadParams uploadParams);
        Task<bool> DeleteFile(string deletePublicId);
    }
}
