using System.Diagnostics;
using Foundation;
using LinkedIn.Hakawai;
using UIKit;

namespace SpeakLink.Controls.MaciOS;

public static class UIPasteboardExtensions
{
    public const string GifSelector = "com.compuserve.gif";
    public static nfloat DefaultCompressionQuality { get; set; } = 85 / 100f;

    public static async Task<string?> GetImageFromPasteboardAsync(this UIPasteboard pasteboard)
    {
        try
        {
            var gif = pasteboard.DataForPasteboardType(GifSelector);
            if (gif?.Length > 0)
            {
                var gifFilePath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache),
                        Guid.NewGuid() + ".gif");
                var gifFilePathUrl = new NSUrl(gifFilePath);
                gif.Save(gifFilePathUrl, false);
                return gifFilePath;
            }

            var img = pasteboard.Image;
            if (img == null)
                return null;

            using (img)
            {
                var data = img.AsPNG() ?? img.AsJPEG(DefaultCompressionQuality);
                if (data != null)
                {
                    await using var stream = data.AsStream();
                    var newFilePath =
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            Guid.NewGuid() + ".JPG");

                    await using var fileStream = File.Create(newFilePath);
                    stream.Seek(0, SeekOrigin.Begin);
                    await stream.CopyToAsync(fileStream);

                    return newFilePath;
                }
            }
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine("Can't copy image from pasteboard");
                Debug.WriteLine(ex);
            }
        }

        return null;
    }
}

public static class RichTextViewExtensions
{
    public static void TransformTextAtRange(this SpeakLinkRichTextView richTextView, NSRange range,
        Func<NSAttributedString, NSAttributedString> transform)
    {
        HKWTextView_TextTransformation.TransformTextAtRange(richTextView, range, transform);
        richTextView.RaiseFormattedTextChanged();
    }
}