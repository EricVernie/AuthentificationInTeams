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
                "https://login.microsoftonline.com/38afde78-8c0b-41f8-b6a7-1f145a83aa9f/v2.0", 
                "https://login.microsoftonline.com/72f988bf-86f1-41af-91ab-2d7cd011db47/v2.0" };
        }
    }
}
