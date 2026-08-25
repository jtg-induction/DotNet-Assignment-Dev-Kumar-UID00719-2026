# Restaurant Management System

A RESTful Web API built using ASP.NET Web API and Entity Framework 6 for managing users, restaurants, menus, and orders.

## Tech Stack

- ASP.NET Web API (.NET Framework)
- Entity Framework 6
- SQL Server
- C#
- Visual Studio 2022

## Features

- User Management
- Restaurant Management
- Menu Management
- Order Management
- User Address Management
- Restaurant Owner Mapping

## Database

Main Entities:

- User
- UserAddress
- Restaurant
- RestaurantOwner
- RefreshToken
- Menu
- Order
- OrderItem

## Setup

1. Clone the repository

```bash
git clone https://github.com/jtg-induction/DotNet-Assignment-Dev-Kumar-UID00719-2026
```

2. Open the solution in Visual Studio 2022.

3. Create `ConnectionStrings.config` file using `ConnectionStrings.template.config` and replace the placeholders with local values

4. Create `Secrets.config` file using `Secrets.template.config` and replace the placeholders with local values

5. Update the connection string in `Web.config`.

6. Run migrations:

```powershell
Update-Database
```

7. Run the project.

## Migrations

Create a new migration:

```powershell
Add-Migration MigrationName
```

Apply migration:

```powershell
Update-Database
```

## Endpoints

| Method | Endpoint                  | Description										 |
|--------|---------------------------|---------------------------------------------------|
| POST   | /api/auth/signup          | Registers a new user								 |
| POST   | /api/auth/login           | Logs in an user								     |
| POST   | /api/auth/logout          | Logs out an user								     |
| POST   | /api/auth/refresh         | Refreshes access token for an authenticated user  |
| POST   | /api/user/address         | Adds a new address for an user                    |
| PATCH  | /api/user/address         | Updates an existing address of an user            |
| PATCH  | /api/user                 | Updates the details of an existing user           |
| PATCH  | /api/user/deactivate      | Deactivates an existing user                      |
| PATCH  | /api/user/password        | Changes password for an existing user             |

## Author

Dev Kumar Singh
