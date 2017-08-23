using System;
using Microsoft.AspNetCore.Builder;

namespace Toolbelt.AspNetCore.MarkdownPages
{
    public static class MarkdownPagesExtensions
    {
        // <summary>
        /// This method is called to enable Markdown Pages feature in an application.
        /// </summary>
        public static IApplicationBuilder UseMarkdownPages(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MarkdownPagesMiddleWare>(new MarkdownPagesOptions());
        }

        // <summary>
        /// This method is called to enable Markdown Pages feature in an application.
        /// </summary>
        public static IApplicationBuilder UseMarkdownPages(this IApplicationBuilder app, MarkdownPagesOptions options)
        {
            return app.UseMiddleware<MarkdownPagesMiddleWare>(options);
        }
    }
}
