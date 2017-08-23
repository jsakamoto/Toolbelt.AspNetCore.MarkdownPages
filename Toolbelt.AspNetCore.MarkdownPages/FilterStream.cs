using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.AspNetCore.MarkdownPages
{
    internal class FilterStream : Stream
    {
        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => this.RedirectToStream?.Length ?? 0;

        public override long Position
        {
            get => this.RedirectToStream?.Position ?? 0L;
            set
            {
                if (this.RedirectToStream == null) throw new InvalidOperationException();
                this.RedirectToStream.Position = value;
            }
        }

        public Stream OriginalStream { get; private set; }

        public bool Captured => this.MemoryStream != null;

        private HttpContext HttpContext { get; set; }

        private Stream RedirectToStream { get; set; }

        private MemoryStream MemoryStream { get; set; }

        public static FilterStream InjectTo(HttpContext context)
        {
            return new FilterStream(context);
        }

        private FilterStream(HttpContext context)
        {
            this.HttpContext = context;
            this.OriginalStream = context.Response.Body;
            context.Response.Body = this;
        }

        public override void Flush()
        {
            this.RedirectToStream?.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            this.RedirectToStream?.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.RedirectToStream == null)
            {
                var contentType = this.HttpContext.Response.ContentType;
                if (contentType.Split(';').First().Trim().ToLower() == "text/markdown")
                {
                    this.MemoryStream = new MemoryStream();
                    this.RedirectToStream = this.MemoryStream;
                }
                else
                {
                    this.RedirectToStream = this.OriginalStream;
                }
            }
            this.RedirectToStream.Write(buffer, offset, count);
        }

        public string GetCapturedContent()
        {
            var encodingName = this.HttpContext.Response.ContentType.Split(';').Skip(1).FirstOrDefault() ?? "utf-8";
            var encoding = Encoding.GetEncoding(encodingName);
            this.MemoryStream.Seek(0, SeekOrigin.Begin);
            var textReader = new StreamReader(this.MemoryStream, encoding, detectEncodingFromByteOrderMarks: true);
            var content = textReader.ReadToEnd();
            return content;
        }
    }
}
