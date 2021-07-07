# Configuration de l'application

1. Clonez le code

    git clone https://github.com/EricVernie/AuthentificationInTeams.git

2. Enregistrez l'application sur Azure Active Directory

    [Inscription dans Azure Active Directory](./InscriptionAAD.md)

3. Mettez à jour avec les informations obtenues lors de l'enregistrement de l'application :

    3.1 Pour .NET la section **AzureAD** dans le fichier **appsettings.json**.
    
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

    3.4 Modifiez le fichier **Manifest\manifest.json** de microsoft teams en conséquence et déployez l'application dans Teams
    ```JSON
    "webApplicationInfo": {
        "id": "[CLIENT ID]",
        "resource": "api://yyy.yyyy.com/[CLIENT ID]"
    }
    ```

