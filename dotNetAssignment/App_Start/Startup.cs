using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using System;
using System.Configuration;
using System.Text;

[assembly: OwinStartup(typeof(dotNetAssignment.App_Start.Startup))]

namespace dotNetAssignment.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureJwt(app);
        }

        private void ConfigureJwt(IAppBuilder app)
        {
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,

                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = ConfigurationManager.AppSettings["JwtIssuer"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                ConfigurationManager.AppSettings["JwtKey"])),

                        ClockSkew = TimeSpan.Zero
                    }
                });
        }
    }
}
