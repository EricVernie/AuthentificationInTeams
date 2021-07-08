# Authentifier les utilisateurs dans Microsoft Teams

Les mécanismes d'authentification et d'autorisation que nous décrirons dans cet article sont basés essentiellement sur le standard OAuth 2.0 utilisé en conjonction avec le fournisseur d'identité Azure Active Directory.

Si vous n'êtes pas familié avec ce standard, je ne peux que vous conseiller la lecture de cet article [Protocoles OAuth 2.0 et OpenID Connect sur la plateforme Microsoft Identity](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/active-directory-v2-protocols) et la documentation générale sur la plateforme [Microsoft Identity](https://docs.microsoft.com/fr-fr/azure/active-directory/develop/), si vous souhaitez aller plus loin.

Au dela de l'authentification de l'utilisateur, le but essentiel étant d'obtenir un jeton de type [JSON Web Token](https://fr.wikipedia.org/wiki/JSON_Web_Token) (JWT pour faire court), qui serait porteur d'attributs permettant d'être authentifié et autorisé à accèder à des ressources.

Dans ces différents exemples, nous allons démontrer, comment obtenir un jeton JWT afin d'accèder aux ressources de [l'API Microsoft Graph](https://docs.microsoft.com/fr-fr/graph/api/overview?view=graph-rest-1.0), mais gardez à l'esprit que cela pourrait être vos propres API protégées par Azure Active Directory.

Il existe différentes manières d'obtenir ce jeton

* [Authentification dans les onglets](#Authentification-dans-les-onglets)
* [Authentification silencieuse](#Authentification-en-mode-silencieux)
* [Authentification SSO](#Authentification-SSO)


## Authentification dans les onglets

Ce flux d'authentification originel, utilise des méthodes du [SDK client Teams](https://docs.microsoft.com/fr-fr/javascript/api/overview/msteams-client?view=msteams-client-js-latest), en conjonction avec les points d'entrées d'Azure Active Directory d'authentification et d'authorisation.

En d'autres termes, il se suffit à lui même, il n'utilise pas de librairie tier pour l'acquisition de jeton, mais exécute des requêtes HTTP directement sur les points d'entrées 'https://login.microsoftonline.com/common/oauth2/v2.0/authorize' et 'https://login.microsoftonline.com/common/oauth2/v2.0/token'

Retrouvez directement les exemples de code ici

    /aspnet.core
            /wwwroot
                /Tab            
    /node.js
        /public
               /Tab    

Sinon continuez la lecture [Démarrer avec l'authentification dans les onglets](./Tab.md).



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

Du faite que nous ayons à implémenter le flux on-behalf-of, nous avons besoin d'utiliser un middleware une API Backend, c'est pour cette raison que vous retrouverez deux projets avec leur middleware associé.

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

## Utilisation de l'application

### Etape 1: Inscrire une application Azure Active Directory

# Inscrire une application Azure Active Directory

1. Connectez-vous au portail [Azure Active Directory](https://aad.portal.azure.com)
2. Selectionnez dans le panneau à gauche **Azure Active Directory**
3. Puis sélectionnez **Inscriptions d'applications**

    3.1 Dans le menu cliquez sur **+ Nouvelle inscription**

    3.2 Donnez un nom à votre application

    3.3 Cochez la case **Comptes dans un annuaire d'organisation (tout annuaire Azure AD - Multilocataire) et comptes Microsoft personnels (par exemple, Skype, Xbox)**

    3.4  Cliquez en bas sur le bouton **S'inscrire**

4. Sélectionnez **Vue d'ensemble**

    4.1 Copiez et sauvegardez le GUID **ID d'application (client)**.

    4.2 Copiez et sauvegardez le GUID **ID de l'annuaire (locataire)**.

5. Sélectionnez **Authentification**

    5.1 Cliquez sur **+ Ajouter une plateforme**

    5.2 Choisissez  **Application à page unique**

    5.3 Dans le champ **URI de redirection** entrez l'uri suivante : https://FQDN/Tab/tabauthenticationend.html et https://FQDN/Silent/tabsilentauthenticationend.html
    >**Ne pas cocher les cases Jetons d'accès, ni Jetons d'ID**

    5.4 Cliquez le bouton **configurer**

    5.5 Cliquez sur **+ Ajouter une plateforme**

    5.6 Choisissez  **Web**

    5.7 Dans le champ **URI de redirection** entrez l'uri suivante : https://FQDN/auth-end

    >Note : Pour l'uri de redirection, il faudra rentrer votre nom de domaine, l'hôte sur lequel l'application tourne.
    Si vous utilisez un service de tunneling tel que ngrok pour la version gratuite, vous devez mettre à jour cette valeur chaque fois que votre sous-domaine ngrok change.

6. Sélectionnez **Certificats & Secrets**
    6.1 Cliquez sur **+ Nouveau Secret**
    Copiez le secret et sauvegardez le pour une utilisation future.

7. Sélectionnez *Exposer une API**
    7.1 Sélectionnez URI ID d'application **Définir**

    7.2 Entrez exactement api://FQDN/{Client ID}

    >Note : Si vous utilisez un service de tunneling tel que ngrok pour la version gratuite, vous devez mettre à jour cette valeur chaque fois que votre sous-domaine ngrok change.


    7.3 Sélectionnez **Ajouter une étendue**. Dans le panneau qui s’ouvre, entrez **access_as_user** comme nom d’étendue.

    7.4 A la section **Qui peut accepter**, sélectionnez **Administrateurs et Utilisateurs**

    7.5 Entrez les détails dans les zones de configuration des invites de consentement de l’administrateur et de l’utilisateur avec des valeurs appropriées pour l'étendue **access_as_user**.
    | Titre| Description |
    | ------------- |:-------------|
    |**Titre du consentement de l’administrateur :**| Teams peut accéder au profil de l’utilisateur.|
    |**Description du consentement de l’administrateur :**| Teams peut appeler les API web de l’application en tant qu’utilisateur actuel.|
    |**Titre de consentement utilisateur :**| Teams peut accéder à votre profil et effectuer des demandes en votre nom.|
    |**Description du consentement de l’utilisateur :**| Teams peut appeler les API de cette application avec les mêmes droits que vous.|

    7.6 Vérifiez que **Etat** est défini comme **Activé**.

    7.7 Cliquez sur **Ajouter une étendue**

    L'étendue **_api://FQDN/{Client ID}/access_as_user_** est ajoutée.

    7.8 Dans la section **Applications clientes autorisées**, sélectionnez **+ Ajouter une application cliente**

    7.9 Ajoutez les deux applications suivantes :

    | ID Client| Description |
    | ------------- |:-------------|
    |1fec8e78-bce4-4aaf-ab1b-5451cc387264| pour Teams mobile ou de bureau.|
    |5e3ce6c0-2b1f-4285-8d4b-75ee78787346| pour Teams web.|

    En n'oubliant pas de cocher **Etendues autorisées**

8. Sélectionnez **API autorisées**. Sélectionnez **+ Ajouter une autorisation Microsoft** puis  > **Microsoft Graph** > **Autorisations déléguées**, puis ajoutez les   autorisations suivantes à partir de l'API Graph :
    User.Read activé par défaut, email, offline_access, OpenId, profil

Avant de continuer, assurez-vous que vous avez bien copié, **Le client ID de l'application**, **Le Secret de l'application**, **Le numéro de locataire Azure Active Directory**

Nous les réutiliserons dans la configuration de l'application

### Etapes 2 : Mise à jour du code.

1. .NET

    1.1 Dans le fichier [appsettings.json](.\aspnet.core\appsettings.json) Modifiez les paramètres **Audience**, la section **AzureAd** du fichier  avec les informations 




>**Il ne faut **JAMAIS** laisser de secret dans son application. Mais pour des raisons de simplicité ici je l'accepte. Néanmoins il est préférable d'utiliser des services externes pour protéger les secrets, comme des coffres forts, style Azure Keyvault.

3. Mettez à jour le code avec les informations obtenues lors de l'enregistrement de l'application :

    3.1 Pour .NET à la section **AzureAD** dans le fichier **appsettings.json**.    
    ```JSON
        {
        "AzureAd": {
            "Instance": "https://login.microsoftonline.com/",            
            "Audience": "[CLIENT ID]", 
            "TenantId": "[TENANT ID]", 
            "ClientId": "[CLIENT ID]", 
            "ClientSecret": "CLIENT SECRET",    
        },
    ```
    dans le fichier **ValideIssuers.cs** le numéro du locataire
    
    ```CSHARP
     public static string[] GetListIssuers()
        {
            
            return new string[] { 
                "https://login.microsoftonline.com/[TENANT ID]/v2.0"};
        }
    ```

    3.2  Pour node.js dans le fichier **server.js**.

    ```JS
        const config = {
            auth: {
                clientId: "[CLIENT ID]"
                authority: "https://login.microsoftonline.com/[TENANT ID]", 
                clientSecret: "[CLIENT SECRET]",
            }
        };
    ```

    3.3 Dans le fichier **\scripts\authConfig.js**, copiez l'**ID d'application (client)** obtenu à l'étape 4.1 lors de l'inscription de l'application dans le champ **clientId**

    ```JS
    const msalConfig = {
        auth: {
        clientId: "[CLIENT ID]", 
            authority: "https://login.microsoftonline.com/common", 
            redirectUri: window.location.origin + "/Silent/tabsilentauthenticationend.html"        
        },
    ```

    3.4 Modifiez le fichier **Manifest\manifest.json** de microsoft teams en conséquence et déployez l'application dans Teams.
    
    Créez un nouvel ID d'application et remplacez la section **id**
    

    Remplacez **https://yyy.yyyy.com** par votre FQDN

    Et enfin à la section **webApplicationInfo** indiquez le Client id et l'uri de la ressource.

    ```JSON
        {
        "$schema": "https://developer.microsoft.com/en-us/json-schemas/teams/v1.9/MicrosoftTeams.schema.json",
        "manifestVersion": "1.9",
        "version": "1.0.0",
        "id": "[APPLICATION ID]",
        "packageName": "com.teams.ev.fdd",
        "developer": {
            "name": "[Auteur]",
            "websiteUrl": "https://yyy.yyyy.com",
            "privacyUrl": "https://yyy.yyyy.com/privacy",
            "termsOfUseUrl": "https://yyy.yyyy.com/termsofuse"
        },
        "icons": {
            "color": "bot-icon-192x192.png",
            "outline": "bot-icon-outline-32x32.png"
        },
        "name": {
            "short": "Training Auth",
            "full": "Training GSP"
        },
        "description": {
            "short": "Training Auth in Teams 2021-06",
            "full": "Demos Teams"
        },
        "accentColor": "#0EA20E",
        "staticTabs": [
            {
            "entityId": "tabsso",
            "name": "SSO",
            "contentUrl": "https://yyy.yyyy.com/sso/sso.html",
            "websiteUrl": "https://yyy.yyyy.com",
            "scopes": [
                "personal"
            ]
            },
            {
            "entityId": "tabsilent",
            "name": "Silent",
            "contentUrl": "https://yyy.yyyy.com/silent/tabsilentauthenticationstart.html",
            "websiteUrl": "https://yyy.yyyy.com",
            "scopes": [
                "personal"
            ]
            },
            {
            "entityId": "tabauthentication",
            "name": "Authentication",
            "contentUrl": "https://yyy.yyyy.com/tab/tabauthenticate.html",
            "websiteUrl": "https://yyy.yyyy.com",
            "scopes": [
                "personal"
            ]
            }
        ],
        "permissions": [
            "identity",
            "messageTeamMembers"
        ],
        "validDomains": [
            "*.yyyy.com"
        ],
        "webApplicationInfo": {
            "id": "[CLIENT ID]",
            "resource": "api://yyy.yyyy.com/[CLIENT ID]"
        }
    }
    ```

1. Clonez le code

    git clone https://github.com/EricVernie/AuthentificationInTeams.git
