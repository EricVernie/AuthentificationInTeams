# Authentifier les utilisateurs dans Microsoft Teams

Les mécanismes d'authentification et d'autorisation que nous décrirons dans cet article sont basés essentiellement sur le standard OAuth 2.0 utilisé en conjonction avec le fournisseur d'identité Azure Active Directory.

Si vous n'êtes pas familié avec ce standard, je ne peux que vous conseiller la lecture de cet article [Protocoles OAuth 2.0 et OpenID Connect sur la plateforme Microsoft Identity](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/active-directory-v2-protocols) et la documentation générale sur la plateforme [Microsoft Identity](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/), si vous souhaitez aller plus loin.

Au dela de l'authentification de l'utilisateur, le but essentiel étant d'obtenir un jeton de type [JSON Web Token](https://fr.wikipedia.org/wiki/JSON_Web_Token) (JWT pour faire court), qui serait porteur d'attributs permettant d'être authentifié et autorisé à accèder à des ressources.

Dans ces différents exemples, nous allons démontrer, comment obtenir un Jeton JWT afin d'accèder aux ressources de [l'API Microsoft Graph](https://docs.microsoft.com/fr-fr/graph/api/overview?view=graph-rest-1.0), mais gardez à l'esprit que cela pourrait être vos propres API protégées par Azure Active Directory. (L'exemple sur SSO vous explique comment protéger une API via Azure Active Directory)

Curieusement, il existe différentes manières d'obtenir ce jeton

* [Authentification dans les onglets](#Authentification-dans-les-onglets)
* [Authentification silencieuse](#Authentification-en-mode-silencieux)
* [Authentification SSO](#Authentification-SSO)

Ceci est du au faite de l'évolution de Microsoft Teams dans le temps.

## Authentification dans les onglets

Ce flux d'authentification originel, utilise des méthodes du [SDK client Teams](https://docs.microsoft.com/fr-fr/javascript/api/overview/msteams-client?view=msteams-client-js-latest), en conjonction avec les points d'entrées d'Azure Active Directory d'authentification et d'authorisation.

En d'autres termes, il se suffit à lui même, il n'utilise pas de librairie tier pour l'acquisition de Jeton, mais exécute des requêtes HTTP directement sur les points d'entrées 'https://login.microsoftonline.com/common/oauth2/v2.0/authorize' et 'https://login.microsoftonline.com/common/oauth2/v2.0/token'

Retrouvez directement les exemples de code ici

    /aspnet.core
            /wwwroot
                /Tab            
    /node.js
        /public
               /Tab    

Ou alors continuez la lecture [Démarrer avec l'authentification dans les onglets](./Tab.md).

Si vous souhaitez aller plus loin voir la [Documentation officielle](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-flow-tab).

## Authentification en mode silencieux

Ce flux d'authentification utilise la librairie [MSAL.js v2 pour navigateur](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-browser), et on l'appel mode silencieux, car elle va permettre de mettre en cache local le compte de l'utilisateur authentifié, afin d'obtenir lors des appels suivants un jeton de maniere silencieuse, c'est à dire sans intéraction avec l'utilisateur.

Retrouvez directement les exemples de code ici

    /aspnet.core
            /wwwroot
                /Silent            
    /node.js
        /public
               /Silent    

Ou alors continuez la lecture [Démarrez avec l'authentification en mode silencieux](./Silent.md).

Si vous souhaitez aller plus loin voir la [Documentation officielle](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-silent-aad)

## Authentification SSO

Arrivée plus récement, elle permet de récupèrer en un seul appel de fonction le jeton de l'utilisateur authentifié. 

Néanmoins ce jeton n'étant porteur que d'un ensemble limité d'autorisations (email, profile,offline_access and OpenId ), il faudra l'utiliser en conjonction avec le flux oauth2 [on-behalf-of](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow) afin d'obtenir des autorisations plus importantes pour l'API Microsoft Graph.

Du du faite que nous ayons à implémenter le flux on-behalf-of, nous avons besoin d'utiliser un middleware une API Backend, c'est pour cette raison que vous retrouverez deux projets avec leur middleware associé.

    /aspnet.core
            /wwwroot
                /SSO
            /Controllers
                /authcontroller.cs

    /node.js
        /public
                /SSO
    server.js


[Démarrez avec l'authentification SSO](./SSO.md).

Si vous souhaitez aller plus loin voir la [Documentation officielle](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-aad-sso) 