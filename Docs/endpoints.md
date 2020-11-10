# Kraken API endpoints

This document describes the various endpoints of the two Kraken API's.

## Main API
The main API handles data not related to sensor measurements. All endpoints are prepended by `/api`, e.g. `http://localhost:5000/api/area` for areas.

### Areas

**GET /area**
Displays a list of areas.

_Upon success, returns status code 200 with data:_

```
[
    {
        "id": string,
        "zip_code": string
    },
    ...   
]
```

**GET /area/{id:string}**
Display information about a specific area.

_Upon success, returns status code 200 with data:_

```
{
    "id": string,
    "zip_code": string
}
```

If record is not found, status code 404 is returned.

**POST /area**
Adds a new area to the database.

_Requires the following data object:_

```
{
    "zip_code": string
}
```

_Upon success, returns status code 201 with data:_

```
{
    "id": string,
    "titzip_codele": string
}
```

If the user is not authenticated, then status code 401 is returned.

**PUT /area/{id:string}**
Updates an existing area.

_Requires the following data object:_

```
{
    "zip_code": string
}
```

Returns status code 204. If the record was not found, then status code 404 is returned. If the user is not authenticated, then status code 401 is returned.

**DELETE /area/{id:string}**
Deletes an area from the database.

Returns status code 204 succesful. If the record was not found, then status code 404 is returned. If the user is not authenticated, then status code 401 is returned.

### Intersections

**GET /intersection**
Displays a list of intersections.

_Upon success, returns status code 200 with data:_

```
[
    {
        "id": string,
        "streets": array[string]
    },
    ...   
]
```

**GET /intersection/{id:string}**
Display information about a specific intersection.

_Upon success, returns status code 200 with data:_

```
{
    "id": string,
    "streets": array[string]
}
```

If record is not found, status code 404 is returned.

**POST /intersection**
Adds a new intersection to the database.

_Requires the following data object:_

```
{
    "streets": array[string]
}
```

_Upon success, returns status code 201 with data:_

```
{
    "id": string,
    "streets": array[string]
}
```

If the user is not authenticated, then status code 401 is returned.

**PUT /intersection/{id:string}**
Updates an existing intersection.

_Requires the following data object:_

```
{
    "streets": array[string]
}
```

Returns status code 204. If the record was not found, then status code 404 is returned. If the user is not authenticated, then status code 401 is returned.

**DELETE /intersection/{id:string}**
Deletes an intersection from the database.

Returns status code 204 succesful. If the record was not found, then status code 404 is returned. If the user is not authenticated, then status code 401 is returned.

## Authentication

**POST /auth/login**
Creates an authentication token for the user.

_Requires the following data object:_

```
{
    "email": string,
    "password": string
}
```

Returns status code 200 with the following data object upon success:

```
{
    "token_value": string
}
```

If credentials are incorrect, then the status code 401 is returned, along with the following data object:

```
{
    "error": string
}
```

**POST /auth/register**
Creates a new user account.

_Requires the following data object:_
```
{
    "username": string,
    "email": string,
    "password": string,
    "passwordConfirmation": string
}
```

If successful, returns 200 along with a login token (see `GET /auth/login`; the user does not have to log in again). If user exists or passwords do not match, then status code 400 is returned, along with the following data object:

```
{
    "error": string
}
```

## Sensor API
The sensor API handles data related to sensor measurements. All endpoints are prepended by `/api/measurements`, e.g. `http://localhost:5002/api/measurements/house` for house measurements.

### House measurements
**GET /house**
Displays a list of house measurements (all of them, might be a lot of data. With might, I mean probably. With probably, I mean definitely.).

_Upon success, returns status code 200 with data:_

```
[
    {
        "id": string,
        "house_id": string,
        "consumption": float,
        "created_at": DateTime
    },
    ...   
]
```

**GET /house/{id:string}**
Displays a single house measurement (not sure why you would want to, but hey).

_Upon success, returns status code 200 with data:_

```
{
    "id": string,
    "house_id": string,
    "consumption": float,
    "created_at": DateTime
}
```

If record is not found, status code 404 is returned.

**GET /house/latest**
Displays the most recent measurement per house id. You probably want to use this one instead of `GET /house`.

_Upon success, returns status code 200 with data:_

```
[
    {
        "id": string,
        "house_id": string,
        "consumption": float,
        "created_at": DateTime
    },
    ...   
]
```

**POST /house**
Adds a new house measurement to the database.

_Requires the following data object (creation date is set automatically):_

```
{
    "house_id": string,
    "consumption": float
}
```

_Upon success, returns status code 201 with data:_

```
{
    "id": string,
    "house_id": string,
    "consumption": float,
    "created_at": DateTime
}
```

**DELETE /house/{id:string}**
Deletes a house measurement from the database.

Returns status code 204 succesful. If the record was not found, then status code 404 is returned.