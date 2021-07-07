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

    4.1 Copiez et sauvegardez le GUID **ID d'application (client)**


5. Sélectionnez **Authentification**

    5.1 Cliquez sur **+ Ajouter une plateforme**

    5.2 Choisissez  **Application à page unique**

    5.3 Dans le champ **URI de redirection** entrez l'uri suivante : https://[votre nom domaine]/Tab/tabauthenticationend.html
    >**Ne pas cocher les cases Jetons d'accès, ni Jetons d'ID**

    >Note : Pour l'uri de redirection, il faudra rentrer votre nom de domaine, l'hôte sur lequel l'application tourne.
    Peut être ngrok

    5.4 Cliquez le bouton **configurer**

    


## Authentification SSO







