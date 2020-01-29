using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityHeaders.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Response.Headers.All(x => x.Key != "Feature-Policy"))
                    context.Response.Headers.Add("Feature-Policy", new[] { "accelerometer 'none'; camera 'none'; geolocation 'self'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'" });

                await next();
            });
            app.UseCsp(options =>
            {
                options.BlockAllMixedContent()
                .ScriptSources(s => s.Self())
                .StyleSources(s => s.Self())
                .StyleSources(s => s.UnsafeInline())
                .FontSources(s => s.Self())
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self());
            });
            app.UseXfo(option =>
            {
                option.Deny();
            });
            app.UseXXssProtection(option =>
            {
                option.EnabledWithBlockMode();
            });
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
        }
        public static void UseHsts(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHsts(options =>
            {
                options.MaxAge(days: 365).IncludeSubdomains().Preload();
            });
        }
    }
}
