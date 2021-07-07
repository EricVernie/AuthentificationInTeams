# Inscrire une application Azure Active Directory

## Authentification application à page unique

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

    >Note : Pour l'uri de redirection, il faudra rentrer votre nom de domaine, l'hôte sur lequel l'application tourne.
    Si vous utilisez un service de tunneling tel que ngrok, vous devez mettre à jour cette valeur chaque fois que votre sous-domaine ngrok change.

    5.4 Cliquez le bouton **configurer**

## Authentification SSO

Si vous souhaitez utiliser la même application que pour l'application à page unique vous pouvez sauter directement à la rubrique 5.

1. Connectez-vous au portail [Azure Active Directory](https://aad.portal.azure.com)
2. Selectionnez dans le panneau à gauche **Azure Active Directory**
3. Puis sélectionnez **Inscriptions d'applications**

    3.1 Dans le menu cliquez sur **+ Nouvelle inscription**

    3.2 Donnez un nom à votre application

    3.3 Cochez la case **Comptes dans un annuaire d'organisation (tout annuaire Azure AD - Multilocataire) et comptes Microsoft personnels (par exemple, Skype, Xbox)**

    3.4  Cliquez en bas sur le bouton **S'inscrire**

4. Sélectionnez **Vue d'ensemble**

    4.1 Copiez et sauvegardez le GUID **ID d'application (client)**

    4.2 Copiez et sauvegardez le GUID **ID de l'annuaire (locataire)**.

5. Sélectionnez **Authentification**

    5.1 Cliquez sur **+ Ajouter une plateforme**

    5.2 Choisissez  **Web**

    5.3 Dans le champ **URI de redirection** entrez l'uri suivante : https://FQDN/auth-end
    
    >Note : Pour l'uri de redirection, il faudra rentrer votre nom de domaine, l'hôte sur lequel l'application tourne.
    Si vous utilisez un service de tunneling tel que ngrok, vous devez mettre à jour cette valeur chaque fois que votre sous-domaine ngrok change.
    
    5.4 Cliquez le bouton **configurer**

6. Sélectionnez **Certificats & Secrets**
    6.1 Cliquez sur **+ Nouveau Secret**
    Copiez le secret et sauvegardez le pour une utilisation future.

7. Sélectionnez *Exposer une API**
    7.1 Sélectionnez URI ID d'application **Définir**
    7.2 Entrez exactement api://FQDN/{Client ID} : Exemple api://auth.demozonex.com/92254939-524f-4d4a-b670-31c56ede810d
    Si vous utilisez un service de tunneling tel que ngrok, vous devez mettre à jour cette valeur chaque fois que votre sous-domaine ngrok change.

    7.1 Sélectionnez **Ajouter une étendue**. Dans le panneau qui s’ouvre, entrez **access_as_user** comme nom d’étendue.

    7.2 A la section **Qui peut accepter**, sélectionnez **Administrateurs et Utilisateurs**

    7.3 Entrez les détails dans les zones de configuration des invites de consentement de l’administrateur et de l’utilisateur avec des valeurs appropriées pour l'étendue **access_as_user**.
    | Titre| Description |
    | ------------- |:-------------|
    |**Titre du consentement de l’administrateur :**| Teams peut accéder au profil de l’utilisateur.|
    |**Description du consentement de l’administrateur :**| Teams peut appeler les API web de l’application en tant qu’utilisateur actuel.|
    |**Titre de consentement utilisateur :**| Teams peut accéder à votre profil et effectuer des demandes en votre nom.|
    |**Description du consentement de l’utilisateur :**| Teams peut appeler les API de cette application avec les mêmes droits que vous.|

    7.4 Vérifiez que **Etat** est défini comme **Activé**.

    7.5 Cliquez sur **Ajouter une étendue**

    L'étendue **_api://FQDN/{Client ID}/access_as_user_** est ajoutée.

    7.5 Dans la section **Applications clientes autorisées**, sélectionnez **+ Ajouter une application cliente**

    7.6 Ajoutez les deux applications suivantes :

    | ID Client| Description |
    | ------------- |:-------------|
    |1fec8e78-bce4-4aaf-ab1b-5451cc387264| pour Teams mobile ou de bureau.|
    |5e3ce6c0-2b1f-4285-8d4b-75ee78787346| pour Teams web.|

    En n'oubliant pas de cocher **Etendues autorisées**

8. Sélectionnez **API autorisées**. Sélectionnez **+ Ajouter une autorisation Microsoft** puis  > **Microsoft Graph** > **Autorisations déléguées**, puis ajoutez les   autorisations suivantes à partir de l'API Graph :
    User.Read activé par défaut, email, offline_access, OpenId, profil

Si vous souhaitez obtenir plus d'informations voir la [Documentation Officielle](https://docs.microsoft.com/fr-fr/microsoftteams/platform/tabs/how-to/authentication/auth-aad-sso)