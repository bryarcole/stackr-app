# Stackr API

A .NET 8 Web API for managing ranking lists and items. This API allows users to create, manage, and aggregate rankings for various items.

## Features

- User authentication and authorization
- CRUD operations for ranking lists
- Item management
- Ranking aggregation
- In-memory database for development and testing

## Prerequisites

- .NET 8 SDK
- An IDE (Visual Studio, VS Code, or Rider)

## Getting Started

1. Clone the repository:
```bash
git clone <repository-url>
cd stackr-api
```

2. Build the solution:
```bash
dotnet build
```

3. Run the API:
```bash
dotnet run --project src/Stackr.Api
```

The API will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

## API Endpoints

### Authentication

#### Register User
```
POST /api/auth/register
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "your_password"
}
```

#### Login
```
POST /api/auth/login
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "your_password"
}
```

### Ranking Lists

#### Create Ranking List
```
POST /api/lists
Authorization: Bearer <your_token>
Content-Type: application/json

{
    "name": "My Ranking List",
    "description": "A list of my favorite items"
}
```

#### Get All Lists
```
GET /api/lists
Authorization: Bearer <your_token>
```

#### Get List by ID
```
GET /api/lists/{id}
Authorization: Bearer <your_token>
```

#### Update List
```
PUT /api/lists/{id}
Authorization: Bearer <your_token>
Content-Type: application/json

{
    "name": "Updated List Name",
    "description": "Updated description"
}
```

#### Delete List
```
DELETE /api/lists/{id}
Authorization: Bearer <your_token>
```

### Items

#### Create Item
```
POST /api/items
Authorization: Bearer <your_token>
Content-Type: application/json

{
    "name": "Item Name",
    "description": "Item description"
}
```

#### Get All Items
```
GET /api/items
Authorization: Bearer <your_token>
```

#### Get Item by ID
```
GET /api/items/{id}
Authorization: Bearer <your_token>
```

#### Update Item
```
PUT /api/items/{id}
Authorization: Bearer <your_token>
Content-Type: application/json

{
    "name": "Updated Item Name",
    "description": "Updated description"
}
```

#### Delete Item
```
DELETE /api/items/{id}
Authorization: Bearer <your_token>
```

### Rankings

#### Add Ranking to List
```
POST /api/rankings
Authorization: Bearer <your_token>
Content-Type: application/json

{
    "itemId": "item_id",
    "rankingListId": "list_id",
    "rank": 1
}
```

#### Get Rankings by List
```
GET /api/rankings/list/{listId}
Authorization: Bearer <your_token>
```

#### Get Aggregate Rankings
```
GET /api/rankings/aggregate
Authorization: Bearer <your_token>
```

## Testing

Run the tests using:
```bash
dotnet test
```

## Project Structure

```
stackr-api/
├── src/
│   └── Stackr.Api/
│       ├── Controllers/
│       ├── Endpoints/
│       ├── Models/
│       └── data/
└── tests/
    └── Stackr.Api.Tests/
```

## Development

The project uses:
- .NET 8
- Entity Framework Core
- In-memory database for development
- JWT authentication

## License

[Your License Here] 