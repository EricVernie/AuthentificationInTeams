// Configuration pour msal.js v2
// https://github.com/AzureAD/microsoft-authentication-library-for-js/tree/dev/lib/msal-browser
const msalConfig = {
    auth: {
        clientId: "[CLIENT ID]", //Le client ID de l'application enregistrée sur Azure Active Directory 
        authority: "https://login.microsoftonline.com/common", 
        redirectUri: window.location.origin + "/silent/tabsilentauthenticationend.html",
        redirectPopUri: window.location.origin + "/SSO/authPopupRedirect.html"
    },
    cache: {
        cacheLocation: "localStorage", 
        storeAuthStateInCookie: false, 
    },
    system: {
        loggerOptions: {
            loggerCallback: (level, message, containsPii) => {
                if (containsPii) {
                    return;
                }
                switch (level) {
                    case msal.LogLevel.Error:
                        console.error(message);
                        return;
                    case msal.LogLevel.Info:
                        console.info(message);
                        return;
                    case msal.LogLevel.Verbose:
                        console.debug(message);
                        return;
                    case msal.LogLevel.Warning:
                        console.warn(message);
                        return;
                }
            }
        }
    }
};