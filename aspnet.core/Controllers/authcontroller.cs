using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System.Linq;
using System.Threading.Tasks;

namespace AuhtInTeams.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class authcontroller : ControllerBase
    {
        IConfidentialClientApplication app;
        IConfiguration Configuration;
        private ITokenAcquisition _tokenAcquisition;
        public authcontroller(IConfiguration configuration, 
                              ITokenAcquisition tokenAcquisition)
        {
            Configuration = configuration;
            _tokenAcquisition = tokenAcquisition;
        }
        [HttpGet]
        [Route("token")]
        public async Task<IActionResult> Token()
        {
            var scopeClaim = User.FindFirst("http://schemas.microsoft.com/identity/claims/scope");
            // Le jeton est celui récupèré par la méthode microsoftTeams.authentication.getAuthToken
            // et doit contenir le scope access_as_user
            if (scopeClaim == null || !scopeClaim.Value.Contains("access_as_user"))
            {
                return new UnauthorizedResult();
            }

            // 1- Méthode simplifiée pour optenir un jeton avec le flux on-behalf-of
            // La classe 
            string[] scopes = new string[] { "https://graph.microsoft.com/.default" };
            string token = null;
            try
            {
                
               token= await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
            }
            catch (MicrosoftIdentityWebChallengeUserException webex)
            {
                return Unauthorized(webex.MsalUiRequiredException.ErrorCode);
            }


            return Ok(token);
        }
        [HttpGet]
        [Route("obotoken")]
        public async Task<IActionResult> OboToken()
        {
            // 2 - Méthode utilisant MSAL.NET V2 pour acquisition de Jeton
            //https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow#:~:text=Azure%20Active%20Directory%20can%20provide%20a%20SAML%20assertion,web%20service%20API%20endpoints%20that%20consume%20SAML%20tokens.

            // Si j'arrive jusqu'ici, c'est que le jeton passé dans l'entête 
            // Authorization est valide.

            string clientId = Configuration["AzureAd:ClientId"];
            string secret = Configuration["AzureAd:ClientSecret"];
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
