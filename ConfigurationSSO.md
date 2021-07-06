# Authentifier les utilisateurs dans Microsoft Teams

L'authentification de l'utilisateur n'est qu'une étape, ce qui nous interesse c'est de récupèrer un Jeton d'accès oauth2, afin de pouvoir accèder à des ressources, des APIs protégées par Azure Active Directory, exemple, pouvoir accèder à l'API [Microsoft Graph](https://docs.microsoft.com/fr-fr/graph/api/overview?view=graph-rest-1.0) ou vos propres API.

On passe ce jeton dans l'entête **authorization** d'une requête http, l'API à l'autre bout le valide (ou le rejete) et exécute la méthode demandée.

C'est tout l'enjeu de ce repos, de vous montrer les différentes manières d'obtenir un jeton valide.


Dans la [documentation](https://docs.microsoft.com/fr-fr/microsoftteams/platform/concepts/authentication/authentication) il est précisé qu'il existe 3 manières de s'authentifier et d'obtenir un Jeton d'accès.

• [Le flux d'authentification dans les onglets](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-tab-aad) : Utilise des fonctions du [SDK Client](https://docs.microsoft.com/fr-fr/javascript/api/overview/msteams-client?view=msteams-client-js-latest) Microsoft Teams en conjonction avec les points d'entrés d'Azure Active Directory.

• [L'authentification en mode silencieux](https://docs.microsoft.com/fr-fr/microsoftteams/platform/concepts/authentication/authentication) : Utilise la librairie [MSAL.js v2 pour navigateur](https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-browser)

• [L'authentification SSO](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-aad-sso) : 
Arrivée plus récement, elle permet de récupèrer en un seul appel de fonction le Jeton de l'utilsateur authentifié. Néanmoins ce jeton n'étant pas porteur d'autorisations, il faudra l'utiliser en conjonction avec le flux oauth2 [on-behalf-of](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow)

L'application Teams ici est une application de type SPA, mais du faite que nous ayons à implémenter le flux on-behalf-of, nous avons besoin d'utiliser un middleware, c'est pour cette raison que vous retrouverez deux projets avec leur middleware associé


    /aspnet.core
            /wwwroot
                /Tab
                /Silent
                /SSO
            /Controllers
                /authcontroller.cs



    /node.js
        /public
                /Tab
                /Silent
                /SSO
    server.js

Vous trouverez donc dans ce repos le code des trois manières de faire sous leur arborescence respective et pour des raisons de simplicité et de compréhension le code Javascript est inclus directement dans des pages html (que les pros du javascript ne m'en veuillent pas trop).

[Le flux d'authentification dans les onglets](https://github.com/EricVernie/AuthentificationInTeams/blob/main/Tab.md)

[L'authentification en mode silencieux](https://github.com/EricVernie/AuthentificationInTeams/blob/main/Silent.md)

[L'authentification SSO](https://github.com/EricVernie/AuthentificationInTeams/blob/main/SSO.md)
