const express = require('express');
const msal = require('@azure/msal-node');
const jwt = require('jsonwebtoken')
const jwksClient = require('jwks-rsa');


const config = {
  auth: {
      clientId: "[AAD CLIENT ID]",
      authority: "https://login.microsoftonline.com/38afde78-8c0b-41f8-b6a7-1f145a83aa9f",
       /* Attention ne jamais laisser de secret dans du code*/
      clientSecret: "[SECRET]",
  }
};

const cca = new msal.ConfidentialClientApplication(config);
const app = express();



var port = normalizePort(process.env.PORT || '3000');
app.set('port', port);

// Valide le jeton qui est passer dans l'entète Authorization
const validateJwt = (req, res, next) => {
  const authHeader = req.headers.authorization;
  if (authHeader) {
      const token = authHeader.split(' ')[1];

      const validationOptions = {
          audience: config.auth.clientId,
          issuer: config.auth.authority + "/v2.0"
      }

      jwt.verify(token, getSigningKeys, validationOptions, (err, payload) => {
          if (err) {
              console.log(err);
              return res.sendStatus(403);
          }

          next();
      });
  } else {
      res.sendStatus(401);
  }
};

const getSigningKeys = (header, callback) => {
  var client = jwksClient({
      jwksUri: 'https://login.microsoftonline.com/common/discovery/keys'
  });

  client.getSigningKey(header.kid, function (err, key) {
      var signingKey = key.publicKey || key.rsaPublicKey;
      callback(null, signingKey);
  });
}
//middleware
// app.use((req, res, next) => {

//   console.log("Validation du jeton");
//   ValidateTheToken(req,res,next);
// })

app.use(express.static('public'));

app.get('/', (req, res) => res.send('Bonjour et Bienvenue v0.0.2!'));

// 
app.get('/obotoken',validateJwt, (req,res) => {
  const authHeader = req.headers.authorization;

  const oboRequest = {
      // Recupère le jeton
      oboAssertion: authHeader.split(' ')[1],
      scopes: [".default"],
  }

  cca.acquireTokenOnBehalfOf(oboRequest).then((response) => {
      console.log(response);
      res.send(response.accessToken);      
  }).catch((error) => {      
      res.status(401).send(error);
  });

});


// Démarre le serveur
app.listen(port, () => console.log(`Ecoute sur ${port}!`));


function normalizePort(val) {
  var port = parseInt(val, 10);

  if (isNaN(port)) {
    // named pipe
    return val;
  }

  if (port >= 0) {
    // port number
    return port;
  }

  return false;
}