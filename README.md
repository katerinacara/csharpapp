# C# Accepted Assessment App

A .NET web application that integrates with third-party APIs to provide product, category, and authentication functionality.

## Overview

This project was developed as part of the Accepted C# technical assessment.

The assignment focused on improving the existing application architecture, implementing new functionality, improving authentication and observability, adding testing and Docker support, and applying CQRS to the Product feature.

## Implemented Features

### 1. HTTP Client Refactoring

Refactored the HTTP client implementation to provide a more structured and reliable approach for communicating with the third-party API.

The application uses the configured HTTP client infrastructure and Polly-based resilience support where applicable.

### 2. Products

Implemented the required Product operations:

* Get all products
* Get a single product by ID
* Create a product

The Product endpoints are exposed through versioned API routes.

### 3. CQRS for Products

Implemented the CQRS pattern using MediatR for the Product feature.

#### Commands

* `CreateProductCommand`
* `CreateProductCommandHandler`

#### Queries

* `GetProductQuery`
* `GetProductQueryHandler`
* `GetProductsQuery`
* `GetProductsQueryHandler`

The API endpoints communicate with MediatR instead of directly calling the Product service:

```text
HTTP Request
    ↓
Product Endpoint
    ↓
MediatR
    ↓
Command / Query
    ↓
Handler
    ↓
IProductsService
    ↓
Third-party API
```

This separates request handling from the underlying business/service operations and keeps the API layer focused on HTTP concerns.

### 4. Categories

Added support for category operations, including the corresponding API endpoints, services, DTOs, and tests.

### 5. JWT Authentication

Implemented JWT authentication using the credentials and configuration provided by the application settings.

The application supports authenticated requests and provides an authenticated profile endpoint.

### 6. Request Performance Logging

Added middleware to measure and log request performance.

The middleware records request execution information, allowing the application to monitor the duration of incoming HTTP requests.

### 7. Unit Testing

Added unit tests covering the main application functionality, including:

* Products
* Categories
* Authentication
* Middleware

Current test status:

```text
Total:   21
Passed:  21
Failed:  0
Skipped: 0
```

### 8. Docker Support

Added Docker support for running the application in a containerized environment.

Included:

* `Dockerfile`
* `.dockerignore`

## Architecture

The solution follows a layered architecture:

```text
CSharpApp.Api
    ↓
CSharpApp.Application
    ↓
CSharpApp.Core

CSharpApp.Infrastructure
    ↓
External API / Infrastructure concerns
```

The main projects are:

| Project                    | Responsibility                                                |
| -------------------------- | ------------------------------------------------------------- |
| `CSharpApp.Api`            | API endpoints, middleware and application startup             |
| `CSharpApp.Application`    | Application services, CQRS commands, queries and handlers     |
| `CSharpApp.Core`           | DTOs, interfaces and shared domain contracts                  |
| `CSharpApp.Infrastructure` | HTTP clients, authentication and infrastructure configuration |
| `CSharpApp.Tests`          | Unit tests                                                    |

## API

The application uses versioned API endpoints.

Example Product endpoints:

```text
GET  /api/v1/products
GET  /api/v1/products/{id}
POST /api/v1/products
```

Category and authentication endpoints are also available through the API.

## Running the Application

### Requirements

* .NET 9 SDK
* Docker (optional)

### Run locally

```bash
dotnet restore
dotnet build
dotnet run --project src/CSharpApp.Api
```

### Run tests

```bash
dotnet test
```

## Docker

Build the Docker image:

```bash
docker build -t csharpapp .
```

Run the container:

```bash
docker run -p 5225:5225 csharpapp
```

## Validation

The implementation has been validated with:

```bash
dotnet build
dotnet test
```

The current test suite passes with **21/21 tests successful**.

## Development Notes

The implementation preserves the existing service-based architecture while introducing CQRS through MediatR for Products.

The CQRS implementation separates read and write operations into dedicated requests and handlers without unnecessarily changing the existing infrastructure and service layer.
