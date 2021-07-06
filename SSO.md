# [L'authentification SSO](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-aad-sso)


Le SSO ou authentification unique en Français permet à l’utilisateur de se connecter une seule fois et d’accéder aux services sans être obligé de ré-entrer ces informations d'identification

Accéder aux services dans notre cas serait de récupèrer un jeton oauth2 d'accès, par exemple effectuer une requête sur [l'API Microsoft Graph](https://docs.microsoft.com/fr-fr/graph/api/overview?view=graph-rest-1.0)

Avec le [SDK Client Teams](https://docs.microsoft.com/fr-fr/javascript/api/overview/msteams-client?view=msteams-client-js-latest) il est possible d'obtenir un Jeton comme illustré dans le code suivant :

```JS
function GetTeamsToken() {
    microsoftTeams.initialize(window);
    microsoftTeams.authentication.getAuthToken({
        successCallback: resultAccessToken => {
            $('#TeamsTokens').text(resultAccessToken);
            $("#btnServerSideToken").show();
        },
        failureCallback: reason => {
            $('#Error').text(reason);                   
        }
    });
}
```

Ce jeton pourrait faire l'affaire si vous le passiez à votre propre API qui pourrait valider son intégrité et exécuter la méthode demandée. 

Néanmoins, pour l'API Graph par exemple, ce jeton n'est porteur que de peut d'autorisations (email, profile, offline_access and OpenId) ce qui n'est pas suffisant lorsqu'on souhaite accéder à d'autres ressources proposées par l'API Graph.

On va donc utiliser le flux d'autorisation [on-behalf-of](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow), qui va nous permettre d'obtenir un jeton valide au nom de l'utilisateur authentifié sur Microsoft Teams.

Comment ça marche ?

1. Il nous faut tout d'abord un middleware (une API Backend) qui va valider le Jeton obtenu par la méthode **_getAuthToken()_**. Même si cela n'est pas indispensable pour mettre en place le flux on-behalf-of, c'est une bonne pratique à utiliser en terme de sécurité.
Dans notre exemple, nous avons donc deux projets séparés, l'un pour .NET et l'autre pour node.js.

>Remarque : Dans ces exemples les API Backend retournent les jetons d'accès. En conditions réels vous utiliseriez ces jetons directement dans l'API Backend afin de requêter les API Graph.

Pour .NET nous utilisons la librairie [Microsoft.Identity.Web](https://github.com/AzureAD/microsoft-identity-web) pour protéger notre API backend

```CSharp
public void ConfigureServices(IServiceCollection services)
    {
    
    services.AddMicrosoftIdentityWebApiAuthentication(Configuration)
       .EnableTokenAcquisitionToCallDownstreamApi()
       .AddInMemoryTokenCaches();
    // Code omis pour plus de clarté
  }
```

La méthode **_.EnableTokenAcquisitionToCallDownstreamApi()*_** va nous permettre par la suite d'obtenir trés facilement un jeton d'accès avec le flux on-behalf-of.

```CSharp
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

```

Le fichier appsettings.json doit contenir impérativement la section AzureAd comme illustré ici :

```JSON
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "[NOM DE DOMAINE]",
    "Audience": "[CLIENT ID]", 
    "TenantId": "[TENANT ID]", 
    "ClientId": "[CLIENT ID]", 
    "ClientSecret": "CLIENT SECRET",    
  },

```

| Paramètres| Description |
| ------------- |:-------------|
| Domain | Nom du domain Azure Active Directory |
| Audience | Dans notre exemple le client ID de l'application enregistrée sur Azure Active Directory |
| TenantId | Le tenand Id Azure Active Directory |
| ClientId | Le client ID de l'application enregistrée sur Azure Active Directory |
| ClientSecret** | Le secret de l'application enregistrée sur Azure Active Directory |

>** Il ne faut JAMAIS mettre en dur de secret dans son application. Mais pour des raisons de simplicité ici je l'accepte. Néanmoins il est préférable d'utiliser des artifices comme des coffres forts, style Azure Keyvault.

Pour node.js, nous utiliserons des librairies [jsonwebtoken](https://www.npmjs.com/package/jsonwebtoken) et [jwks-rsa](https://www.npmjs.com/package/jwks-rsa) pour valider le jeton.

```JS
const validateJwt = (req, res, next) => {
  const authHeader = req.headers.authorization;
  if (authHeader) {
      const token = authHeader.split(' ')[1];

      const validationOptions = {
          audience: config.auth.clientId,
          issuer: config.auth.authority + "/v2.0"
      }

      jwt.verify(token, getSigningKeys, validationOptions, (err, payload) => {
          if (err) {
              console.log(err);
              return res.sendStatus(403);
          }

          next();
      });
  } else {
      res.sendStatus(401);
  }
};


const getSigningKeys = (header, callback) => {
  var client = jwksClient({
      jwksUri: 'https://login.microsoftonline.com/common/discovery/keys'
  });

  client.getSigningKey(header.kid, function (err, key) {
      var signingKey = key.publicKey || key.rsaPublicKey;
      callback(null, signingKey);
  });
}
```

Puis pour obtenir le jeton d'accès avec le flux on-behalf-of, nous utiliserons la librairie [MSAL.JS V2 pour node.js](https://www.npmjs.com/package/@azure/msal-node)

```JS
app.get('/token',validateJwt, (req,res) => {
  const authHeader = req.headers.authorization;
  const oboRequest = {
      oboAssertion: authHeader.split(' ')[1],
      scopes: [".default"],
  }
  cca.acquireTokenOnBehalfOf(oboRequest).then((response) => {
      console.log(response);
      res.send(response.accessToken);      
  }).catch((error) => {
      res.status(401).send(error);
  });

});
```

Configuration pour MSAL 

```JS
const config = {
  auth: {
      clientId: "[CLIENT ID]"
      authority: "https://login.microsoftonline.com/[TENANT ID]", 
      clientSecret: "[CLIENT SECRET]",
  }
};
```

Encore une fois le secret ne doit JAMAIS être codé en dur dans le code !!!


