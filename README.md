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
- Menu
- Order
- OrderItem

## Setup

1. Clone the repository

```bash
git clone <repository-url>
```

2. Open the solution in Visual Studio 2022.

3. Update the connection string in `Web.config`.

4. Run migrations:

```powershell
Update-Database
```

5. Run the project.

## Migrations

Create a new migration:

```powershell
Add-Migration MigrationName
```

Apply migration:

```powershell
Update-Database
```

## Author

Dev Kumar Singh