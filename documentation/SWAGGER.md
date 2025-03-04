# Описание Swagger UI

## Аутентификация
После получения токена из `/auth/sign-up` или `/auth/sign-in` нужно нажать на кнопку `Authorize` в правом верхнем углу.
Во всплывшем меню нужно написать "Bearer" + пробел + токен. Например, для токена "aaa" нужно написать "Bearer aaa".

## Примеры запросов
### `POST /book/{id}`
```json
{
  "from": "2025-03-04T14:00:00.000Z",
  "to": "2025-03-04T15:00:00.000Z",
  "description": "string"
}
```

### `PATCH /confirm-qr`
```json
{
  "code": "1122334455"
}
```

### `PATCH /book/{id}/reschedule`
```json
{
  "from": "2025-03-04T14:00:00.000Z",
  "to": "2025-03-04T15:00:00.000Z"
}
```

### `POST /decorations`
Иконка:
```json
{
  "type": "Icon",
  "x": 12,
  "y": 25,
  "name": "toilet"
}
```
Прямоугольник:
```json
{
  "type": "Rectangle",
  "x": 12,
  "y": 24,
  "name": "pub",
  "width": 100,
  "height": 200
}
```

### `POST /zone/office/{id}/seat`
```json
{
  "x": 25,
  "y": 24,
  "innerNumber": "3"
}
```

### `POST /zone/office/{id}/seats`
```json
[
  {
    "x": 0,
    "y": 0,
    "innerNumber": "1"
  },
  {
    "x": 25,
    "y": 24,
    "innerNumber": "2"
  }
]
```

### `POST /request`
```json
{
  "text": "DOOR STUCK",
  "actionNumber": 0,
  "status": 0,
  "additionalInfo": "bruh",
  "bookId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### `POST /user/{id}/passport`
```json
{
  "serial": "1111",
  "number": "123455",
  "firstname": "Pablo",
  "lastname": "Kaktycovich",
  "middlename": "4erny",
  "birthday": "11.09.2001"
}
```

### `POST /user/{id}/verification-photo`
Multipart-файл.

### `POST /zone`
```json
{
  "name": "string",
  "description": "string",
  "capacity": 20,
  "isPublic": true,
  "xCoordinate": 0,
  "yCoordinate": 0,
  "width": 1000,
  "height": 2000,
  "tags": [0]
}
```
