# Configuration de l'application

1. Clonez le code

    git clone https://github.com/EricVernie/AuthentificationInTeams.git

2. Enregistrez l'application sur Azure Active Directory

    [Inscription dans Azure Active Directory](./InscriptionAAD.md)

3. Mettez à jour le code avec les informations obtenues lors de l'enregistrement de l'application :

    3.1 Pour .NET à la section **AzureAD** dans le fichier **appsettings.json**.
    
    ```JSON
        {
        "AzureAd": {
            "Instance": "https://login.microsoftonline.com/",
            "Domain":   "[NOM DE DOMAINE]",
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
            // Les GUID ici correspondent aux Id de Tenants Azure Active Directory
            return new string[] { 
                "https://login.microsoftonline.com/[TENANT ID]/v2.0"};
        }
    ```

    3.2  Pour node.js la section **const config** du fichier **server.js**.

    ```JS
        const config = {
            auth: {
                clientId: "[CLIENT ID]"
                authority: "https://login.microsoftonline.com/[TENANT ID]", 
                clientSecret: "[CLIENT SECRET]",
            }
        };
    ```

    3.3 Ouvrez le fichier **\scripts\authConfig.js** et copiez l'**ID d'application (client)** obtenu à l'étape 4.1 lors de l'inscription de l'application dans le champ **clientId**

    ```JS
    const msalConfig = {
        auth: {
        clientId: "[CLIENT ID]", //Le client ID de l'application enregistrée sur Azure Active Directory  
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