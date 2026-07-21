# Integration S3

Cette API utilise `AWSSDK.S3` et la chaine d'identifiants AWS standard. Ne stocke pas `AWS_ACCESS_KEY_ID` ou `AWS_SECRET_ACCESS_KEY` dans `appsettings.json`.

## Configuration

Ajoute la section suivante dans `appsettings.json`, dans les variables d'environnement, ou dans la configuration Elastic Beanstalk :

```json
{
  "S3": {
    "BucketName": "bucketbankaccount",
    "Region": "us-east-1",
    "Prefix": "",
    "PreSignedUrlExpirationMinutes": 15
  }
}
```

Variables d'environnement equivalentes :

```powershell
$env:S3__BucketName = "bucketbankaccount"
$env:S3__Region = "us-east-1"
$env:S3__Prefix = ""
```

Pour un deploiement AWS, prefere un role IAM attache a l'environnement. En local, utilise un profil AWS CLI ou les variables `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY` et, si besoin, `AWS_SESSION_TOKEN`.

## Endpoints

- `POST /api/S3/upload?folder=customers` avec un champ multipart `file`
- `GET /api/S3/download?key=FR.png`
- `GET /api/S3/presigned-url?key=...&expirationMinutes=15`
- `DELETE /api/S3?key=...`

## Permissions IAM minimales

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "s3:PutObject",
        "s3:GetObject",
        "s3:DeleteObject"
      ],
      "Resource": "arn:aws:s3:::bucketbankaccount/*"
    }
  ]
}
```
