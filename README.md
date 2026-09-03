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

| Method | Endpoint								   | Description									   |
|--------|-----------------------------------------|---------------------------------------------------------|
| POST   | /api/auth/signup						   | Registers a new user									 |
| POST   | /api/auth/login						   | Logs in an user										 |
| POST   | /api/auth/logout          			   | Logs out an user										 |
| POST   | /api/auth/refresh        		       | Refreshes access token for an authenticated user		 |
| POST   | /api/user/address         			   | Adds a new address for an user							 |
| PATCH  | /api/user/address         			   | Updates an existing address of an user					 |
| PATCH  | /api/user                 			   | Updates the details of an existing user				 |
| PATCH  | /api/user/deactivate      			   | Deactivates an existing user							 |
| PATCH  | /api/user/password        			   | Changes password for an existing user					 |
| POST   | /api/order                			   | Place a new order										 |
| GET    | /api/order/details/{orderId:guid}       | Retrieves details of a specific order					 |
| POST   | /api/order/cancel/{orderId:guid}        | Cancels a specific order								 |
| GET    | /api/restaurant/				           | Retrieves list of all available restaurants  			 |
| GET    | /api/restaurant/menu/{restaurantId:guid}| Retrieves menu of a specific restaurant				 |
| POST   | /api/restaurant/create                  | Created a new restaurant								 |
| POST   | /api/restaurant/onboard                 | Onboards a new restaurant owner to a restaurant		 |
| POST   | /api/order/update                       | Updates the order status of an order					 |
| POST   | /api/order/get		                   | Fetches list of orders of an owner's restaurants		 |
| GET    | /api/report/top-items	               | Generates a report of top 10 ordered items				 |
| GET    | /api/report/frequently-bought           | Generates a report of items frequently bought together  |


## Author

Dev Kumar Singh
