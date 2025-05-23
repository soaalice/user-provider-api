# User Provider Api

A robust RESTful API for user authentication and management built with ASP.NET Core and PostgreSQL.

## Features

- User registration with email validation
- Secure login with JWT authentication
- Password hashing using BCrypt
- PostgreSQL database integration
- Swagger documentation

## API Endpoints

### Authentication

- `POST /api/user/register` - Register a new user
- `POST /api/user/login` - Login and receive JWT token
- `POST /api/user/logout` - Logout (requires authentication)

### Request Examples

**Register User**
```json
POST /api/user/register
{
    "username": "johndoe",
    "email": "john@example.com",
    "password": "securePassword123"
}
```

**Login**
```json
POST /api/user/login
{
    "username": "johndoe",
    "password": "securePassword123"
}
```
