# Authentifier les utilisateurs dans Microsoft Teams

Il existe 3 manières d'authentifier dans son application Microsoft Teams les utilisateurs protégés par Azure Active Directory, comme expliqué dans la [documentation](https://docs.microsoft.com/fr-fr/microsoftteams/platform/concepts/authentication/authentication) (hors authentification des bots, fera l'objet d'un autre exemple)
 
 

•	[Le flux d'authentification dans les onglets](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-flow-tab)


•	[L'authentification en mode silencieux](https://docs.microsoft.com/fr-fr/microsoftteams/platform/concepts/authentication/authentication)

•	[L'authentification SSO ](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-aad-sso)


L'authentification de l'utilisateur n'est qu'une étape, ce qui nous interesse c'est de récupèrer un Jeton d'accès de type JWT, afin de pouvoir accèder à des ressources, des APIs protégées par Azure Active Dictory.

Par exemple, pouvoir accèder à l'API [Microsoft Graph](https://docs.microsoft.com/fr-fr/graph/api/overview?view=graph-rest-1.0), afin d'avoir accès aux ressources de l'utilisteur authentifié, comme ses messages, ses fichiers, ses calendriers, ou toutes autres ressources exposées.


# [Le flux d'authentification dans les onglets](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-flow-tab)

1. Lorsque la page **_/tab/tabauthenticate.html_** se charge, elle déclenche la méthode **_Authenticate()_**

```JS
function Authenticate() {
            microsoftTeams.initialize();
            microsoftTeams.getContext((context) => {
                microsoftTeams.authentication.authenticate({
                    url: window.location.origin + "/tab/tabauthenticationstart.html",
                    width: 1024,
                    height: 1024,
                    successCallback: function (result) {
                        console.log(result);
                        $('#AccessToken').text(result.access_token);
                    },
                    failureCallback: function (reason) {
                        $('#Error').text("Login failed: " + reason);
                    }
                });
            });            
        }
```

Vous noterez ici que nous utilisons le [SDK Client Teams](https://docs.microsoft.com/fr-fr/javascript/api/overview/msteams-client?view=msteams-client-js-latest) (**microsoftTeams**) afin de déclencher l'authentification et la récupèration d'un Jeton d'accès.

2. La page **_tab/tabauthenticationstart.html_** est invoquée par le client Teams, c'est elle qui va présenter la
page de consentements
![consent](https://github.com/EricVernie/CloudNativeAppForAzureDev/blob/main/images/TabConsentement.png)

```JS
microsoftTeams.initialize(window)
microsoftTeams.getContext(async function (context) {
    let state = _guid(); 
    localStorage.setItem("state", state);
    var originalCode = _guid();
    var codeChallenge = await pkceChallengeFromVerifier(originalCode);
    localStorage.removeItem("codeVerifier");
    localStorage.setItem("codeVerifier", originalCode);

    var queryParams = {
        client_id: msalConfig.auth.clientId,
        response_type: "code", 
        response_mode: "fragment",
        scope: ["User.Read", "Mail.Read"],
        redirect_uri: window.location.origin.toLowerCase() + "/Tab/tabauthenticationend.html",
        nonce: _guid(),
        state: state,
        login_hint: context.upn,
        code_challenge: codeChallenge,
        code_challenge_method: "S256",
    };
    let params = toQueryString(queryParams);
    let authorizeEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize?" + params;
    window.location.assign(authorizeEndpoint);
});
```