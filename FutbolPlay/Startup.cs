using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Newtonsoft.Json;
using Owin;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;

namespace FutbolPlay
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            ConfigureOAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            var issuer = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["currentApiSecurity"]];
            var AppUser = TextEncodings.Base64Url.Decode("4e686af7bdcc5ae005a247624fd8c7283257c2514f6b3ad2ff5d4cb6d95196e6");
            var AppAdmin = TextEncodings.Base64Url.Decode("d4f0bc5a29de06b510f9aa428f1eedba926012b591fef7a518e776a7c9bd1824");
            var AdminWeb = TextEncodings.Base64Url.Decode("d4f0bc5a29de06b512389nsvdisvyr89qriojfsdiow32r98q4e776a7c9bd1824");

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { "099153c2625149bc8ecb3e85e03f0022", "cdb59355f3ba293977fc0945fb85f118", "cdb59355f3ba293977fc0945fb85aiop" },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, AppUser),
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, AppAdmin),
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, AdminWeb)
                    }
                });

        }
    }
}