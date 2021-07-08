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

Avant de continuer, assurez-vous que vous avez bien copié, **Le client ID de l'application**, **Le Secret de l'application**, **Le numéro de locataire Azure Active Directory**, les réutiliserons à l'étape 2.

### Etapes 2 : Mise à jour du code

1. .NET

    1.1 Dans le fichier [appsettings.json](./aspnet.core/appsettings.json) modifiez les paramètres **Audience**, **TenantId**, **ClientId** et **ClientSecret** de la section **AzureAd** avec les valeurs obtenues à l'étape 1.

    1.2 Dans le fichier [authConfig.js](./aspnet.core/scripts/authConfig.js) , modifiez la propriété **clientId** avec la valeur obtenue à l'étape 4.1 lors de l'inscription de l'application dans Azure Active Directory

2. Node.js

    2.1 Dans le fichier [server.js](./node.js/server.js) modifiez les paramètres **clientId**, **authority**, et **clientSecret** avec les valeurs obtenues à l'étape 1.

    2.2 Dans le fichier [authConfig.js](./node.js/scripts/authConfig.js) , modifiez la propriété **clientId** avec la valeur obtenue à l'étape 4.1 lors de l'inscription de l'application dans Azure Active Directory

### Etape 3 : Mise à jour du fichier [Manifest de Teams ](./manifestteams/manifest.json)
    
1. Créez un nouveau GUID d'application et remplacez le parametre **"id":"[APPLICATION ID]"**

2. Pour la section **staticTabs** remplacez toutes les propriétés **"contentUrl":** avec votre FQDN, ou alors si vous testez en locale avec ngrok par un FQDN du type XXXXX.ngrock.io.

3. Enfin à la section **webApplicationInfo** indiquez le Client id et l'uri de la ressource qui doit être de la forme api://FQDN/[CLIENT ID].


## Etape 3 : Test en local

1. Clonez le code

    git clone https://github.com/EricVernie/AuthentificationInTeams.git

2. pour .

