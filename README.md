# 💰 Expense Tracker
A full-featured personal expense tracker built using ASP.NET Core and Angular. This project follows Clean Architecture principles with DDD and CQRS using MediatR. It enables users to manage expenses across multiple categories, view reports, and visualize financial trends through charts and dashboards.

---

## 🚀 Features
- ✅ User authentication with JWT (cookie-based)
- ✅ Add, edit, and delete expenses with category, amount, and currency
- ✅ Dashboard with recent and yearly expense charts
- ✅ Filter and search expenses with pagination
- ✅ Weekly, monthly, and yearly reports (screen/PDF/Excel)
- ✅ Background job support with Hangfire
- ✅ Export reports using QuestPDF and ClosedXML
- ✅ Dockerized for easy local development
- ✅ Unit and integration tests using xUnit and Moq

---

## 🛠 Tech Stack

### Backend
- ASP.NET Core 9.0
- Entity Framework Core
- MediatR (CQRS)
- Hangfire (background jobs)
- QuestPDF & ClosedXML (report generation)
- SQL Server
- Serilog (logging)
- xUnit, Moq (testing)

### Frontend
- Angular 19.x.x
- Angular Material
- RxJS

### DevOps
- Docker & Docker Compose
- Swagger/OpenAPI

---

## 📦 Project Structure
<pre lang="text">
<code> 
ExpenseTracker/
├── src/
│   ├── Core/
│   │   ├── ExpenseTracker.Application      // Application layer: Use cases, DTOs, MediatR handlers
│   │   └── ExpenseTracker.Domain           // Domain layer: Entities, Value Objects, Domain Events
│   ├── Infrastructure/
│   │   ├── EExpenseTracker.Infrastructure.BackgroundJobs    // Infrastructure layer: For managing Background Jobs
│   │   └── ExpenseTracker.Infrastructure.Email              // Infrastructure layer: For sending Emails
│   │   └── ExpenseTracker.Persistence                       // Infrastructure layer: EF Core DbContext, Configurations, Migrations
│   │   └── ExpenseTracker.Infrastructure.Report             // Infrastructure layer: For generating Excel and PDF reports
│   │   └── ExpenseTracker.Infrastructure.Web.Auth           // Infrastructure layer: For handling authenctication and token generation
│   └── Presentation/
│       └── ExpenseTracker.Presentation.Api             // ASP.NET Core Web API (presentation layer)
├── front-end/                              
│   ├── expense-tracker.front-end.angular    // Angular 19 frontend app
├── tests/                                 // Unit and integration tests
│   ├── Core/
│   ├── ExpenseTracker.Application.Tests
│   └── ExpenseTracker.Domain.Tests
│   ├── Infrastructure/
│   ├── ExpenseTracker.Infrastructure.Persistence.Tests
├── docker-compose.yml
└── README.md
</code>
</pre>

---

## 🐳 Running with Docker

### Prerequisites
- Docker Desktop installed and running

### Steps
```bash
git clone https://github.com/anirbanbose/ExpenseTracker.git
cd ExpenseTracker
docker-compose up --build
```

This will:
Build and run the backend (.NET 9) API
Run SQL Server as a container
Run the Angular frontend

Access the application
API: http://localhost:5000 (Note: Swagger UI will not be available because the Docker build runs in production mode)
Frontend: http://localhost:5200

Default Credentials
Initial seed data is automatically applied during the first run.
You can log in using:
```
Email: testuser@expensetracker.com
Password: 123456
```
---

## 🤝 Contributing
Feel free to fork the repo and open a PR if you find bugs or have improvements in mind. This project is for demonstration and educational purposes.

---

## 📄 License
This project is licensed under the MIT License.

---

## 👨‍💻 About Me
Anirban Bose – Freelance ASP.NET Developer
### 📫 Email: aspnetlancer@gmail.com
### 🔗 LinkedIn: [anirban-bose-programmer](https://www.linkedin.com/in/anirban-bose-programmer/)

Looking to build something similar for your business? I'm available for freelance work!
