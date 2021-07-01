using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuhtInTeams.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ServersideAuthController : ControllerBase
    {
        IConfidentialClientApplication app;
        IConfiguration Configuration;
        public ServersideAuthController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        [HttpGet]
        [Route("obotoken")]
        public async Task<IActionResult> OboToken()
        {
            //https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow#:~:text=Azure%20Active%20Directory%20can%20provide%20a%20SAML%20assertion,web%20service%20API%20endpoints%20that%20consume%20SAML%20tokens.

            // Si j'arrive jusqu'ici, c'est que le jeton passé dans l'entête 
            // Authorization est valide.

            string clientId = Configuration["AzureAd:ClientId"];
            string secret = Configuration["Secret"];
            var scopeClaim = User.FindFirst("http://schemas.microsoft.com/identity/claims/scope");
            // Le jeton est celui récupèré par la méthode microsoftTeams.authentication.getAuthToken
            // et doit contenir le scope access_as_user
            if (scopeClaim == null || !scopeClaim.Value.Contains("access_as_user"))
            {
                return new UnauthorizedResult();                
            }
            
            // Récupère le Jeton (Sauvegardé lors de la validation, voir startup.cs)            
            var token = (string)User.Identities.First().BootstrapContext;           
            // Crée une assertion avec ce Jeton client
            UserAssertion userAssertion = new UserAssertion(token, "urn:ietf:params:oauth:grant-type:jwt-bearer");

            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
            app = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(secret)
                    .Build();
            // Obtient le nouveau Jeton, porteur de plus d'autorisations
            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenOnBehalfOf(scopes, userAssertion).ExecuteAsync();
            }
            catch(MsalUiRequiredException msalex)
            {
                return Unauthorized(msalex.ErrorCode);
            }
            
                        
            return Ok(result.AccessToken);
        }

        [HttpGet]
        [Route("headers")]
        public IActionResult Headers()
        {
            return Ok(this.HttpContext.Request.Headers);
        }
    }
}
