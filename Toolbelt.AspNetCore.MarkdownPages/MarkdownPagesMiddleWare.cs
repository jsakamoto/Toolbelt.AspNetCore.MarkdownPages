using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeyRed.MarkdownSharp;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.AspNetCore.MarkdownPages
{
    internal class MarkdownPagesMiddleWare
    {
        private static Markdown Markdown { get; } = new Markdown();

        private readonly RequestDelegate _next;

        public MarkdownPagesOptions Options { get; }

        private string HtmlCssLink { get; set; }

        public MarkdownPagesMiddleWare(MarkdownPagesOptions options, RequestDelegate next)
        {
            this.Options = options;
            _next = next;

            var cssLinks = options.CssLinks.Select(url => $"<link rel=\"stylesheet\" href=\"{url}\" />");
            this.HtmlCssLink = string.Join("\n", cssLinks);
        }

        public async Task Invoke(HttpContext context)
        {
            var filterStream = FilterStream.InjectTo(context);

            await _next.Invoke(context);

            if (filterStream.Captured && context.Response.StatusCode == 200)
            {
                var markdownText = filterStream.GetCapturedContent();
                var htmlMainContents = Markdown.Transform(markdownText);

                var htmlTextBuilder = new StringBuilder();
                htmlTextBuilder.AppendLine("<!DOCTYPE html>\n<html>\n<head>\n<meta charset=\"utf-8\"/>");
                if (this.HtmlCssLink != "")
                {
                    htmlTextBuilder.AppendLine(this.HtmlCssLink);
                }
                htmlTextBuilder.Append("</head>\n<body>\n" + htmlMainContents + "</body>\n</html>");

                var htmlBytes = Encoding.UTF8.GetBytes(htmlTextBuilder.ToString());

                context.Response.ContentType = "text/html; utf-8";
                context.Response.ContentLength = htmlBytes.Length;
                await filterStream.OriginalStream.WriteAsync(htmlBytes, 0, htmlBytes.Length);
            }
        }
    }
}
