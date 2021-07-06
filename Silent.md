#  [L'authentification en mode silencieux](https://docs.microsoft.com/fr-fr/microsoftteams/platform/concepts/authentication/authentication)

Pour cette méthode d'authentification, nous allons essentiellement utiliser la librairie [MSAL.js v2 pour navigateur](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-browser)

1. Lorsque la page **_/Silent/tabsilentauthenticationstart.html_** se charge elle déclenche le code la méthode **_MSALLoginPopup()_**

    ```JS
    function MSALLoginPopup() {
        microsoftTeams.initialize(window);
        microsoftTeams.getContext(function (context) {
        var MSALApp = new msal.PublicClientApplication(msalConfig)
        
        var currentAccount = MSALApp.getAccountByUsername(context.upn);
        const silentRequest = {
            scopes: ["User.Read", "Mail.Read"],
            account: currentAccount,
            forceRefresh: false
        };
        const request = {
            scopes: ["User.Read", "Mail.Read"],
            loginHint: context.upn
        };
        // 1 on essai d'acquierir un Token de manière silencieuse
        // 2 si jamais cela ne fonctionne pas on présente une popup à l'utilisateur
        MSALApp.acquireTokenSilent(silentRequest).then(tokenResponse => {
            $('#AccessToken').text(tokenResponse.accessToken);
        }).catch(async (error) => {
            $('#Error').text(error);
            if (context.hostClientType === 'web') {
                MSALApp.acquireTokenPopup(request).then(tokenResponse => {
                    $('#Error').text("");
                    $('#AccessToken').text(tokenResponse.accessToken);
                });
            }
            else {
                //acquireTokenPopup ne fonctionne pas dans le client Desktop
                MSALPopupRedirect(MSALApp);
            }
        }).catch(error => {
            $('#Error').text(error);
                });

            });
        }
    ```

    Tout d'abord on instancie la class **_msal.PublicClientApplication()_** en lui passant les paramètres Azure Active Directory défini de la manière suivante : 

    ```JS
    const msalConfig = {
    auth: {
	    clientId: "[CLIENT ID]",
        authority: "https://login.microsoftonline.com/common", 
        redirectUri: window.location.origin + "/Silent/tabsilentauthenticationend.html"        
    },
    cache: {
        cacheLocation: "localStorage", 
        storeAuthStateInCookie: false, 
    },
    ```
    | Paramètres| Description |
    | ------------- |:-------------|
    |**_clientId_**| Id de l'application enregistrée sur Azure Active Directory. Pour l'inscription d'une application sur Azure Active Directory se référrer à l'article [Inscription d'une application SPA](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/scenario-spa-app-registration#redirect-uri-msaljs-20-with-auth-code-flow) |
    |**_authority_**|Point d'entré Azure Active Directory|
    |**_redirectUri_**|Url qui sera rappelée par Azure Active Directory. Il est important que lors de l'enregistrement de l'application sur Azure Active Directory de bien la mentionner. Pour l'inscription d'une application sur Azure Active Directory se référrer à l'article [Inscription d'une application SPA](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/scenario-spa-app-registration#redirect-uri-msaljs-20-with-auth-code-flow) |

    Si l'utilisateur c'est déjà connecté on récupère son compte :
    ```JS
        var currentAccount = MSALApp.getAccountByUsername(context.upn);
    ```
    Ensuite on va essayer d'obtenir un Jeton de manière silencieuse et afficher le jeton
    ```JS
     MSALApp.acquireTokenSilent(silentRequest).then(tokenResponse => {
                    $('#AccessToken').text(tokenResponse.accessToken);
    ```

    Si c'est la 1ere fois que l'utilisateur utilise l'application, la méthode silencieuse retourne une erreur, c'est alors que l'on demandera à l'utilisateur de s'authentifier et d'approuver les consentements.
    ![consent](https://github.com/EricVernie/AuthentificationInTeams/blob/main/images/SilentConsentement.png)

    >Notes : Cette page de consentement, ne sera affichée qu'une seule fois. D'autre part, si vous êtes sur le client Teams de Bureau ou Mobile, Il est possible que la toute 1ere fois, vous ayez une page qui vous demande de vous authentifier.

    ![Credential](https://github.com/EricVernie/AuthentificationInTeams/blob/main/images/SilentCredentiels.png)

