using System.Diagnostics;
using System.Windows.Input;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using AndroidX.Core.View;
using IOnReceiveContentListener = AndroidX.Core.View.IOnReceiveContentListener;
using Uri = Android.Net.Uri;
using View = Android.Views.View;

namespace SpeakLink.Controls.Android;

public class MediaContentReceiver : Java.Lang.Object, IOnReceiveContentListener
{
    public ICommand? ImageInputCommand { get; set; }
    public static readonly string[] VideoAndImageMimeTypes = ["image/*", "video/*"];
    public static readonly string[] ImageMimeTypes = [ "image/*" ];
    private readonly Context context;

    public MediaContentReceiver(IntPtr handle, JniHandleOwnership transfer) 
        : base(handle, transfer)
    {
    }

    public MediaContentReceiver(Context context)
    {
        this.context = context;
    }

    public ContentInfoCompat? OnReceiveContent(View view, ContentInfoCompat payload)
    {
        var partition = payload.Partition(new ClipPredicate());
        var processed = partition.First as ContentInfoCompat;
        var second = partition.Second as ContentInfoCompat;
        for (int i = 0; i < (processed?.Clip?.ItemCount ?? 0); i++)
        {
            var clipItem = processed!.Clip!.GetItemAt(i);
            if (clipItem?.Uri != null)
            {
                ContentReceived(clipItem.Uri);
            }
        }

        return second;
    }

    private async void ContentReceived(Uri clipItemUri)
    {
        try
        {
            var filePath = await CopyFileToTmpAsync(clipItemUri);
            var originalMimeType = GetMimeTypeIfFilePath(clipItemUri.Path);

            if(ImageInputCommand?.CanExecute(filePath) ?? false)
                ImageInputCommand.Execute(filePath);
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine("Error for getting file from clipboard");
                Debug.WriteLine(ex);
            }
        }
    }

    private async Task<string?> CopyFileToTmpAsync(Uri uri)
    {
        await using var inputStream = context.ContentResolver?.OpenInputStream(uri);
        if ((inputStream?.Length ?? 0) == 0)
            return null;
        
        var newFilePath = await CopyFileForUploadAsync(inputStream, uri.Path);

        return newFilePath;
    }

    private class ClipPredicate : Java.Lang.Object, AndroidX.Core.Util.IPredicate
    {
        public bool Test(Java.Lang.Object? t)
        {
            if (t is ClipData.Item clipDataItem)
                return clipDataItem.Uri != null;
            return false;
        }
    }
    
    public static string? GetMimeTypeIfFilePath(string path)
    {
        var fileExtension = Path.GetExtension(path);
        if (!string.IsNullOrEmpty(fileExtension))
        {
            var mimeTypeMap = MimeTypeMap.Singleton;
            var extensionWithoutDot = new string(fileExtension.Skip(1).ToArray());
            var mimeType = mimeTypeMap?.GetMimeTypeFromExtension(extensionWithoutDot);
            return mimeType;
        }

        return null;
    }
    
    public static async Task<string> CopyFileForUploadAsync(Stream inputStream, string originalFilePath)
    {
        if (originalFilePath == null) throw new ArgumentNullException(nameof(originalFilePath));

        originalFilePath = Path.GetFileName(originalFilePath);
        var documents = FileSystem.CacheDirectory;
        var newFilePath = Path.Combine(documents, "upload", originalFilePath);

        var filePathDirectory = Path.GetDirectoryName(newFilePath);
        if (!Directory.Exists(filePathDirectory))
            Directory.CreateDirectory(filePathDirectory);

        if (File.Exists(newFilePath))
            File.Delete(newFilePath);

        using var writeStream = File.OpenWrite(newFilePath);
        await inputStream.CopyToAsync(writeStream);

        return newFilePath;
    }

    public static Task<string> CopyFileForUploadAsync(string originalFilePath)
    {
        var inputStream = File.OpenRead(originalFilePath);
        return CopyFileForUploadAsync(inputStream, originalFilePath);
    }
}

