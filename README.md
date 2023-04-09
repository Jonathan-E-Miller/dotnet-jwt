# dotnet-jwt

The application exposes two endpoints.
```
POST /Auth/GetToken
```
```
GET /WeatherForecast
```

When calling the `GetToken` endpoint, you must provide a username and password. These credentials would usually be checked against a database and if correct, a token would be generated. However, to keep the application as slim as possible this has been omitted. Please use the following request.
```
{
  username: "test"
  password: "password"
}
```

When calling the `/WeatherForecast` endpoint please include a custom header `jm-token` and set the value to the token generated using the `GetToken` endpoint.
