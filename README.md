# E-Commerce Brand API (.NET) -- Cashlock

This project is a full-stack e-commerce backend system developed using **ASP.NET Core Web API**.  
 served as my **graduation project at the ITI**.

The system simulates a real-world shopping experience with cart handling, checkout, shipping, authentication, and payment integration via **Paymob**.

---

##  Features

- 🔹 **Product Management**
  - Support for **expirable** and **shippable** products
  - Stock control and quantity validation
- 🛒 **Cart Management**
  - Add products with quantity checks
  - Remove or update items
- 💳 **Checkout Process**
  - Subtotal calculation
  - Shipping fees based on weight
  - Balance deduction from customer
  - Integration with a `ShippingService` for shippable items
- 🔐 **Authentication**
  - JWT-based login/register system
- 💰 **Payment Integration**
  - Fully integrated with **Paymob** sandbox API
- 📦 **Order Management**
  - Admin dashboard logic to track and manage orders


---

## 🧰 Tech Stack

- ASP.NET Core 8 Web API
- Entity Framework Core
- SQL Server
- JWT Authentication
- Paymob Payment Gateway
- Clean Code Architecture (Layered Structure)

---
## ▶️ How to Run

Open the project in Visual Studio or VS Code

Configure your appsettings.json with your:

SQL Server connection string

Paymob API key

Run the application

Use Postman or Swagger to test the endpoints



