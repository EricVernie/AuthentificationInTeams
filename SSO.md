# [L'authentification SSO](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-aad-sso)

Le SSO ou authentification unique en Français permet à l’utilisateur de se connecter une seule fois et d’accéder aux services sans être obligé de ré-entrer ces informations d'identification.

Accéder aux services dans notre cas serait de récupèrer un jeton oauth2 d'accès, pour effectuer par exemple une requête sur [l'API Microsoft Graph](https://docs.microsoft.com/fr-fr/graph/api/overview?view=graph-rest-1.0)

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

Néanmoins, pour l'API Graph, ce jeton n'est porteur que de peut d'autorisations (email, profile, offline_access and OpenId) ce qui n'est pas suffisant lorsqu'on souhaite accéder à d'autres ressources proposées par l'API Graph.

On va donc utiliser le flux d'autorisation [on-behalf-of](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow), qui va nous permettre d'obtenir un jeton valide au nom de l'utilisateur authentifié sur Microsoft Teams.

Comment ça marche ?

Il nous faut un middleware (une API Backend) qui va nous permettre de mettre en place ce flux on-behalf-of.

qui va valider le Jeton obtenu par la méthode **_getAuthToken()_**. Même si cela n'est pas indispensable pour mettre en place le flux on-behalf-of, c'est une bonne pratique à utiliser en terme de sécurité.
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

Configuration pour MSAL.JS

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

Une fois l'API Backend mis en place, il faut l'appeler en lui passant dans son entête **_authorization_**" le jeton obtenu par la méthode **_microsoftTeams.authentication.getAuthToken()_**

```JS
function GetServerSideToken() {
    $("#btnConsent").hide();
    var teamsToken = $('#TeamsTokens').text();
    $.ajax({
        url: window.location.origin.toLowerCase() + "/token",
        headers: {
            'Authorization': 'bearer ' + teamsToken
        },
        type: "get",
        success: function (result, status) {
            $('#AccessToken').text(result);            
        },
        error: function (result, status, error) {
            let resultObject=JSON.parse(result.responseText);
            $('#Error').text(error + ":" + resultObject.errorCode);                    
            if (resultObject.errorCode  === "invalid_grant" || resultObject.errorCode  === "unauthorized_client") {
                $("#btnConsent").show();
                $("#btnServerSideToken").hide();
            }
        }
    });
}
```

Si l'appel de l'API backend réussi, on affiche le jeton d'accès

Néanmoins la 1ere fois que l'application est utilisée par l'utilisateur, il y a de grandes chances qu'elle échoue comme illustré sur la figure suivante :

![invalidgrant](https://github.com/EricVernie/AuthentificationInTeams/blob/main/images/SSOInvalidGrant.png)

Le code d'erreur **invalid_grant** signifie que l'utilisateur doit consentir à l'utilisation de l'application.

Dans ce cas la nous allons déclencher la méthode **_MSALRequestConsent()_**

```JS
function MSALRequestConsent() {            
    microsoftTeams.authentication.authenticate({
        url: window.location.origin + "/Popup/authPopupRedirect.html",
        width: 1024,
        height: 1024,
        successCallback: (result) => {
            $('#AccessToken').text(result);
            $("#btnServerSideToken").show();
        },
        failureCallback: (reason) => {
            $('#Error').text(reason);
        }
    });
}
```

La méthode **_microsoftTeams.authentication.authenticate()_** va permettre de charger la page **_authPopupRedirect_.html_** dans une **Popup**.

En fonction de la réussite ou de l'échec de l'authentification, on affiche le jeton ou l'erreur.
Lorsque la page **_authPopupRedirect_** se charge elle exécute le code suivant :

```JS
 $(document).ready(function () {
   microsoftTeams.initialize(window);
   const msalPopupConfig = {
       auth: {
           clientId: msalConfig.auth.clientId,
           authority: msalConfig.auth.authority,
           redirectUri: window.location.origin + "/Popup/authPopupRedirect.html"
       },
       cache: {
           cacheLocation: "sessionStorage", 
           storeAuthStateInCookie: false, 
       }
   };
   const msalClient = new msal.PublicClientApplication(msalPopupConfig);
   microsoftTeams.getContext((context) => {
       msalClient.handleRedirectPromise().then((tokenResponse) => {
           if (tokenResponse) {
               microsoftTeams.authentication.notifySuccess(tokenResponse);
           }
       }).catch((error) => {
           console.log(error);
           microsoftTeams.authentication.notifyFailure(error);
       });
       
       msalClient.loginRedirect({
           scopes: ["User.Read", "Mail.Read"],
           loginHint: context.loginHint
       });
   });
        });
```

C'est la méthode **_msaClient.LoginRedirect()_** qui affichera la page d'authentification et de consentement à l'utilisateur.

![consent](https://github.com/EricVernie/AuthentificationInTeams/blob/main/images/SSOConsentement.png)

Si vous êtes sur le client Teams de Bureau ou Mobile, Il est possible que vous ayez une page qui vous demande de vous authentifier.

![Credential](https://github.com/EricVernie/AuthentificationInTeams/blob/main/images/SSOCredentiels.png)

Si une erreur survient, la méthode **_microsoftTeams.authentication.notifyFailure(error)_** est invoquée et renvoie l'erreur à la page **_/SSO/SSO.html_** qui sera traitée par la méthode **_failureCallback_**

 Si la demande de jeton réussie, la méthode **_microsoftTeams.authentication.notifySuccess(tokenResponse)_** est invoquée et renvoie le résultat à la page **_/Silent/tabsilentauthenticationstart.html_** qui sera traité par la méthode **_successCallback_**

 Vous devriez obtenir une page comme illustré sur la figure suivante : 

![Token](https://github.com/EricVernie/AuthentificationInTeams/blob/main/images/SSOToken.png)

 Il est possible de décoder le Jeton avec le site jwt.ms
