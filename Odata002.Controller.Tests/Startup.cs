using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(biz.dfch.CS.Examples.Odata.Tests.Startup))]

namespace biz.dfch.CS.Examples.Odata.Tests
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            app.UseErrorPage();

            app.Run(context =>
            {
                if (context.Request.Path.ToString().Equals("/fail"))
                {
                    throw new Exception("Random exception");
                }

                context.Response.ContentType = "text/plain";
                return context.Response.WriteAsync("Hello, world.");
            });
        }
    }
}
