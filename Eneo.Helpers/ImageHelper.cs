using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Eneo.Helpers
{
    public static class ImageHelper
    {
        public static HttpResponseMessage GetImage(string id, string path)
        {
            Image img = Image.FromFile(path + id);
            MemoryStream stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(stream.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");

            return result;
        }

        public static byte[] GetImage2(string id, string path)
        {
            Image img = Image.FromFile(path + id);
            MemoryStream stream = new MemoryStream();
            img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(stream.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return stream.ToArray();
        }
    }
}