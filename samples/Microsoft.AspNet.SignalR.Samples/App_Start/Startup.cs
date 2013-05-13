﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

namespace Microsoft.AspNet.SignalR.Samples
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapConnection<SendingConnection>("sending-connection");
            app.MapConnection<TestConnection>("test-connection");
            app.MapConnection<RawConnection>("raw-connection");
            app.MapConnection<StreamingConnection>("streaming-connection");

            app.Use(typeof(ClaimsMiddleware));

            ConfigureSignalR(GlobalHost.DependencyResolver, GlobalHost.HubPipeline);

            var config = new HubConfiguration()
            {
                EnableDetailedErrors = true
            };

            app.MapHubs(config);

            BackgroundThread.Start();
        }

        private class ClaimsMiddleware : OwinMiddleware
        {
            public ClaimsMiddleware(OwinMiddleware next)
                : base(next)
            {
            }

            public override Task Invoke(OwinRequest request, OwinResponse response)
            {
                string username = request.GetHeader("username");

                if (!String.IsNullOrEmpty(username))
                {
                    var authenticated = username == "john" ? "true" : "false";

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Authentication, authenticated)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims);
                    request.User = new ClaimsPrincipal(claimsIdentity);
                }

                return Next.Invoke(request, response);
            }
        }
    }
}