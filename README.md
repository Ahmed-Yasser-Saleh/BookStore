# 📚 Bookstore API

## 📌 Overview
**Bookstore API** is a backend system built using ASP.NET Core Web API to manage bookstore operations including inventory, customer orders, and author management.

The project follows clean architecture principles and implements secure authentication using JWT and ASP.NET Core Identity.

---

## 🚀 Features

- 🔐 JWT Authentication & Authorization
- 👤 ASP.NET Core Identity Integration
- 📧 Email Verification
- 📚 Manage Books (Full CRUD Operations)
- ✍️ Manage Authors (Full CRUD Operations)
- 🛒 Manage Customer Orders
- 🏪 Inventory Management
- 🧱 Repository Pattern Implementation
- 🧩 Unit of Work Pattern Implementation
- 🛡️ Role-Based Authorization

---

## 🛠️ Technologies Used

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- ASP.NET Core Identity
- Repository & Unit of Work Patterns
- SMTP (Email Service)

---

## 🏗️ Architecture

The project is structured with:

- Clean separation of concerns
- Repository Pattern for data access abstraction
- Unit of Work for transaction handling
- Dependency Injection
- RESTful API principles

---

## 🔑 Authentication Flow

1. User registers
2. Email verification link is sent
3. User confirms email
4. User logs in
5. JWT token is generated
6. Token is used to access protected endpoints
