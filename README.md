# Api service

Апи сервис для работы с базой данных по EF6 (MSSQL).
Для того чтобы он начал работать добавьте файл appsettings.json и укажите в нем след данные
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MyConnection": "Строка подключения к вашей БД",
    }
}
Для запуска нужен VS 2019 (2022)
С установленными пакетами для web app
После запуска откроется вкладка с встроенным Swagger где можно будет посмотреть все команды
## Установка


### Раздел Users
#### Url Method: GET/api/Users/{userId:int}

```json
//Json Result 200 (successful)
{
  "email": "string",
  "name": "string",
  "profileImage": 
   {
    "url": "string"
   }
}

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "user not found."
}
```

#### Url Method: GET/api/Users/avatar/get?userId={userId:int}

```json
//Result 200 (successful)
Картинка аватарки

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "user not found."
}
```

#### Url Method: POST/api/Users/avatar/upload

Request
```json
//Request body
{
  "id": 0,
  "avatar": "base64string"
}
```
Response
```json
//Result 200 (successful)
{
  "status": "successful",
  "message": "avatar has been changed."
}

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "user not found."
}
```

#### Url Method: POST/api/Users/login

Request
```json
//Request body
{
  "email": "string",
  "password": "string"
}
```
Response

```json
//Json Result 200 (successful)
{
  "id" : 0,
  "email": "string",
  "name": "string",
  "profileImage": {
    "url": "stringUrl"
  }
}

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "user not found."
}
```

#### Url Method: POST/api/Users/registration

Request
```json
//Request body
{
  "email": "string",
  "password": "string",
  "name": "string"
}
```
Response

```json
//Json Result 200 (successful)
{
  "status": "successful",
  "message": "user has been created."
}

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "user with this email is already registered."
}
```

#### Url Method: POST/api/Users/password/recovery

Request
```json
//Request body
{
  "email": "string"
}
```
Response

```json
//Result 200 (successful)
{
  "status": "successful",
  "message": "password has been changed."
}

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "user not found."
}
```

#### Url Method: POST/api/Users/password/change

Request
```json
//Request body
{
  "id": 0,
  "oldPass": "string",
  "newPass": "string"
}
```
Response

```json
//Result 200 (successful)
{
  "status": "successful",
  "message": "password has been changed."
}

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "user not found."
}
```
#### Url Method: GET/api/Users/adv/get?userId={userId:int}

```json
//Json Result 200 (successful)
{
      "id": 0,
      "userId": 0,
      "title": "string",
      "description": "string",
      "cityId": 0,
      "categoryId": 0,
      "price": 0,
      "mainImageUrl": "stringUrl",
      "images": [
        {
          "id": 0,
          "advertisementId": 0,
          "imageUrl": "stringUrl"
        }
      ]
    }

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "advert not found."
}
```

### Раздел Advert
#### Url Method: GET/api/Advert/{id:int}

```json
//Result 200 (successful)

{
  "id": 1,
  "userId": int,
  "title": "string",
  "description": "string",
  "cityId": int,
  "categoryId": int,
  "price": float,
  "mainImageUrl": "URlstring",
  "images": array
}

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "advert not found."
}
```

#### Url Method: GET/api/Advert/image/get?advId={advId:int}

```json
//File Result 200 (successful)
{FileContent}
//Response headers
content-length: 251 
content-type: image/png 
date: Thu,26 Oct 2023 19:59:18 GMT 
server: Kestrel 

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "advert not found."
}
```

#### Url Method: GET/api/Advert/image/get?advId={advId:int}

```json
//File Result 200 (successful)
[
  {
    "fileContents": "base64string",
    "contentType": "image/png",
    "fileDownloadName": "",
    "lastModified": null,
    "entityTag": null,
    "enableRangeProcessing": false
  }
] 

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "advert not found."
}
```

#### Url Method: GET/api/Advert/image/list/get?advId={advId:int}&index={index:int}

```json
//File Result 200 (successful)
{
  "fileContents": "base64string",
  "contentType": "image/png",
  "fileDownloadName": "",
  "lastModified": null,
  "entityTag": null,
  "enableRangeProcessing": false
}


//Json Result 200 (failed)
{
  "status": "failed",
  "message": "image not found."
}
```

#### Url Method: POST/api/Advert/add
Request
```json
//Json 
{
  "userId": 0,
  "title": "string",
  "description": "string",
  "cityId": 0,
  "categoryId": 0,
  "price": 0,
  "mainImage": "base64string",
  "images": [
    {
      "image": "base64string"
    }
  ]
}
```
Response
```json
//File Result 200 (successful)
{
  "fileContents": "base64string",
  "contentType": "image/png",
  "fileDownloadName": "",
  "lastModified": null,
  "entityTag": null,
  "enableRangeProcessing": false
}


//Json Result 200 (failed)
{
  "status": "failed",
  "message": "user not found."
}
```

#### Url Method: POST/api/Advert/update
Request
```json
//Json 
{
  "id":0,
  "userId": 0,
  "title": "string",
  "description": "string",
  "cityId": 0,
  "categoryId": 0,
  "price": 0,
  "mainImage": "base64string",
  "images": [
    {
      "image": "base64string"
    }
  ]
}
```
Response
```json
//File Result 200 (successful)
{
  "fileContents": "base64string",
  "contentType": "image/png",
  "fileDownloadName": "",
  "lastModified": null,
  "entityTag": null,
  "enableRangeProcessing": false
}


//Json Result 200 (failed)
{
  "status": "failed",
  "message": "user not found."
}
```

#### Url Method: POST/api/Advert/delete?userId={userId:int}&advId={advId:int}
Response
```json
//Json Result 200 (successful)
{
  "status": "successful",
  "message": "advert is removed."
}


//Json Result 200 (failed)
{
  "status": "failed",
  "message": "permission denied."
}
```

#### Url Method: POST/api/Advert/moderate?userId={userId:int}&advId={advId:int}
Response
```json
//Json Result 200 (successful)
{
  "status": "successful",
  "message": "advert is removed."
}


//Json Result 200 (failed)
{
  "status": "failed",
  "message": "permission denied."
}
```

#### Url Method: GET/api/Advert/cities/list
Response
```json
//Json Result 200 (successful)
[
  {
    "id": 1,
    "title": "string"
  },
...
]


//Json Result 200 (failed)
{
  "status": "failed",
  "message": "cities is empty."
}
```

#### Url Method: GET/api/Advert/categories/list
Response
```json
//Json Result 200 (successful)
[
  {
    "id": 1,
    "title": "string"
  },
...
]


//Json Result 200 (failed)
{
  "status": "failed",
  "message": "categories is empty."
}
```

#### Url Method: GET/api/Advert/filter?page={page:int}&cityId={cityId:int}&categoryId={categoryId:int}
Response
```json
//Json Result 200 (successful)
[
  {
    "id": int,
    "userId": int,
    "title": "string",
    "description": "string",
    "cityId": int,
    "categoryId": int,
    "price": float,
    "mainImageUrl": "base64string",
    "images": [
    {
      "image": "base64string"
    }
    ...
    ]
  },
...
]


//Json Result 200 (failed)
{
  "status": "failed",
  "message": "adverts is empty."
}
```

#### Url Method: GET/api/Advert/search?title={title:string}

Response
```json
//Json Result 200 (successful)
[
  {
    "id": 0,
    "userId": 0,
    "title": "string",
    "description": "string",
    "cityId": 0,
    "categoryId": 0,
    "price": 0,
    "mainImageUrl": "base64string",
    "images": [
      {
        "id": 0,
        "advertisementId": 0,
        "imageUrl": "base64string"
      }
      ...
    ]
  }
...
]


//Json Result 200 (failed)
{
  "status": "failed",
  "message": "adverts is empty."
}
```

### Раздел Messages
#### Url Method: GET/api/Messages/inbox?userId={userId:int}

```json
//Result 200 (successful)
[
  {
    "id": 0,
    "senderId": 0,
    "reciverId": 0,
    "createdAt": "2023-10-26T20:32:03.513Z",
    "isReaded": true,
    "isDeleted": true,
    "title": "string",
    "body": "string"
  }
...
]

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "messages is empty."
}
```

#### Url Method: GET/api/Messages/outbox?userId={userId:int}

```json
//Result 200 (successful)
[
  {
    "id": 0,
    "senderId": 0,
    "reciverId": 0,
    "createdAt": "2023-10-26T20:32:03.513Z",
    "isReaded": true,
    "isDeleted": true,
    "title": "string",
    "body": "string"
  }
...
]

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "messages is empty."
}
```

#### Url Method: GET/api/Messages/{id:int}

```json
//Result 200 (successful)
[
  {
    "id": 0,
    "senderId": 0,
    "reciverId": 0,
    "createdAt": "2023-10-26T20:32:03.513Z",
    "isReaded": true,
    "isDeleted": true,
    "title": "string",
    "body": "string"
  }
...
]

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "message not found."
}
```


#### Url Method: POST/api/Messages/send

Request
```json
{
  "senderId": 0,
  "reciverId": 0,
  "title": "string",
  "body": "string"
}
```
Response
```json
//Result 200 (successful)
{
  "status": "successful",
  "message": "message is send."
}

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "message not send."
}
```

#### Url Method: POST/api/Messages/delete?id={id:int}&userId={userId:int}

Response
```json
//Result 200 (successful)
{
  "status": "successful",
  "message": "message is removed."
}

//Json Result 200 (failed)
{
  "status": "failed",
  "message": "message not found."
}
```
