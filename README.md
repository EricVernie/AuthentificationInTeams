# Authentifier les utilisateurs dans Microsoft Teams

L'authentification de l'utilisateur n'est qu'une étape, ce qui nous interesse c'est de récupèrer un Jeton d'accès de type JWT, afin de pouvoir accèder à des ressources, des APIs protégées par Azure Active Directory, par exemple, pouvoir accèder à l'API [Microsoft Graph](https://docs.microsoft.com/fr-fr/graph/api/overview?view=graph-rest-1.0) ou vos propres API.

Dans la [documentation](https://docs.microsoft.com/fr-fr/microsoftteams/platform/concepts/authentication/authentication) il est précisé qu'il existe 3 manières de s'authentifier et d'obtenir un Jeton d'accès.

• [Le flux d'authentification dans les onglets](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-tab-aad) : Utilise des fonctions du [SDK Client](https://docs.microsoft.com/fr-fr/javascript/api/overview/msteams-client?view=msteams-client-js-latest) Microsoft Teams en conjonction avec les points d'entrés d'Azure Active Directory.

• [L'authentification en mode silencieux](https://docs.microsoft.com/fr-fr/microsoftteams/platform/concepts/authentication/authentication) : Utilise la librairie [MSAL.js v2 pour navigateur](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-browser)

• [L'authentification SSO](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-aad-sso) : 
Arrivée plus récement, elle permet de récupèrer en un seul appel de fonction 

L'application Teams ici est une application de type SPA, mais il y a un cas ou nous aurons besoin d'utiliser un middleware, c'est pour cette raison que vous retrouverez une arborescence du style.

Middleware aspnet core

    /aspnet.core
            /wwwroot
                /Tab
                /Silent
                /SSO

middleware node.js

    /node.js
        /public
                /Tab
                /Silent
                /SSO

Vous trouverez donc dans ce repos le code des trois manières de faire sous leur arborescence respective et pour des raisons de simplicité et de compréhension le code est inclus directement dans des pages html (que les pros du javascript ne m'en veuillent pas trop).

# [Le flux d'authentification dans les onglets](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-flow-tab)

Cette authentification ce fait selon les étapes suivantes : 


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
    ```

    La méthode **_microsoftTeams.authentication.authenticate()_** va permettre de charger la page **_tabauthenticationstart.html_** dans une **Popup**.

    En fonction de la réussite ou de l'échec de l'authentification, on affiche le jeton ou l'erreur.

2. La page **_tab/tabauthenticationstart.html_** est invoquée par le client Teams, c'est elle qui va présenter la
page de consentements

    ![consent](https://github.com/EricVernie/AuthentificationInTeams/blob/main/images/TabConsentement.png)

    >Note : Si vous êtes sur le client Teams de Bureau ou Mobile, Il est possible que la toute 1ere fois, vous ayez une page qui vous demande de vous authentifier.

    ![Credential](https://github.com/EricVernie/AuthentificationInTeams/blob/main/images/TabCredentiels.png)

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

Quelques explications s'imposent ici.

En lieu et place du flux [d'authentification implicite](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/v2-oauth2-implicit-grant-flow), j'utilise le flux [d'authenfication par code](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/v2-oauth2-auth-code-flow) et ceci 


>Remarques : Ici c'est le point d'entré v2.0 /oauth2/v2.0/authorize avec le flux [d'authenfication par code](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/v2-oauth2-auth-code-flow) qui sont utilisés en lieu et place du point d'entré v1.0 et du flux [d'authentification implicite](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/v2-oauth2-implicit-grant-flow).
