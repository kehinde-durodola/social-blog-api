namespace SocialBlogApi.Services;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SocialBlogApi.Core.Exceptions;

public class ImageUploadService
{
    private readonly Cloudinary _cloudinary;
    private readonly int _maxFileSizeBytes;
    private readonly List<string> _allowedMimeTypes;

    public ImageUploadService(IConfiguration configuration)
    {
        var cloudinarySection = configuration.GetSection("Cloudinary");
        var cloudName = cloudinarySection["CloudName"] ?? throw new InvalidOperationException("Cloudinary CloudName not configured");
        var apiKey = cloudinarySection["ApiKey"] ?? throw new InvalidOperationException("Cloudinary ApiKey not configured");
        var apiSecret = cloudinarySection["ApiSecret"] ?? throw new InvalidOperationException("Cloudinary ApiSecret not configured");

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);

        _maxFileSizeBytes = int.Parse(cloudinarySection["MaxFileSizeBytes"] ?? "5242880");
        
        var mimeTypesJson = cloudinarySection["AllowedMimeTypes"];
        _allowedMimeTypes = mimeTypesJson != null 
            ? mimeTypesJson.Split(',').Select(m => m.Trim()).ToList()
            : new List<string> { "image/jpeg", "image/png", "image/webp" };
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ApplicationException("No file provided");

        if (file.Length > _maxFileSizeBytes)
            throw new ApplicationException($"File size exceeds maximum allowed size of {_maxFileSizeBytes / (1024 * 1024)}MB");

        if (!_allowedMimeTypes.Contains(file.ContentType))
            throw new ApplicationException($"Invalid file type. Allowed types: {string.Join(", ", _allowedMimeTypes)}");

        try
        {
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "socialblog/posts"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                    throw new ApplicationException($"Upload failed: {uploadResult.Error.Message}");

                return uploadResult.SecureUrl.ToString();
            }
        }
        catch (ApplicationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Image upload failed: {ex.Message}");
        }
    }
}
