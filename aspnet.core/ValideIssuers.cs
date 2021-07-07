using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuhtInTeams
{
    public class ValideIssuers
    {
        // Recupère une liste de Issuers (i.e les tenants qui ont accès à l'application)
        // Cette liste doit être sauvegardée en dehors de l'application
        // Pour des raisons de démonstrations ici, elle est codée en dur

        public static string[] GetListIssuers()
        {
            // Les GUID ici correspondent aux Id de Tenants Azure Active Directory
            return new string[] { 
                "https://login.microsoftonline.com/[TENANT ID]/v2.0"};
        }
    }
}
