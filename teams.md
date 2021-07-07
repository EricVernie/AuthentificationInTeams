# Déployez l'application Microsoft Teams



## Mise à jour du fichier manifest.json

1. Ouvrez le fichier manifest.json













Ajoutez la section
```JSON
  "webApplicationInfo": {
    "id": "[CLIENT ID]",
    "resource": "api://yyy.yyyy.com/[CLIENT ID]"
  }
```


