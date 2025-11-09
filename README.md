# OpenSourceHub 🚀

A platform that helps developers discover and track open source contributions. Built with .NET 9, React, and PostgreSQL.

## 🎯 Features

### ✅ Phase 1 (Completed)

- **GitHub OAuth Authentication** - Secure login with GitHub
- **JWT Token Authentication** - Stateless API authentication
- **Issue Discovery** - Search GitHub issues with advanced filters
  - Filter by programming language
  - Filter by labels (good-first-issue, bug, etc.)
  - Minimum stars filter
  - Pagination support
- **Redis Caching** - Efficient caching to avoid GitHub rate limits
- **Repository Management** - Automatic storage of repository metadata

### 🚧 Coming Soon

- Contribution Tracker (track your PRs across repos)
- Repository Bookmarks with notes
- Background sync jobs (Hangfire)
- Real-time notifications (SignalR)
- AI-powered issue difficulty estimation
- React frontend with TypeScript

## 🏗️ Architecture

- **Clean Architecture** with CQRS pattern
- **Domain-Driven Design** principles
- **MediatR** for command/query handling
- **Entity Framework Core** with PostgreSQL
- **Redis** for distributed caching
- **JWT** for authentication

## 🛠️ Tech Stack

**Backend:**

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Redis
- Octokit (GitHub API)
- MediatR
- Swagger/OpenAPI

**Planned Frontend:**

- React 18
- TypeScript
- Vite
- Tailwind CSS
- Tanstack Query

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 14+](https://www.postgresql.org/download/)
- [Redis](https://redis.io/download)
- GitHub OAuth App (for authentication)

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/OpenSourceHub.git
cd OpenSourceHub
```

### 2. Setup Database

```bash
# Create PostgreSQL database
createdb opensourcehub

# Apply migrations
cd src/OpenSourceHub.API
dotnet ef database update --project ../OpenSourceHub.Infrastructure
```

### 3. Setup GitHub OAuth App

1. Go to [GitHub Developer Settings](https://github.com/settings/developers)
2. Create a new OAuth App
3. Set Authorization callback URL to: `http://localhost:5062/api/auth/github/callback`
4. Copy Client ID and Client Secret

### 4. Configure Application

Create `src/OpenSourceHub.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=opensourcehub;Username=postgres;Password=YOUR_PASSWORD",
    "Redis": "localhost:6379"
  },
  "GitHub": {
    "ClientId": "YOUR_GITHUB_CLIENT_ID",
    "ClientSecret": "YOUR_GITHUB_CLIENT_SECRET",
    "RedirectUri": "http://localhost:5062/api/auth/github/callback",
    "Scope": "read:user user:email"
  },
  "Jwt": {
    "Key": "GENERATE_A_SECURE_KEY_AT_LEAST_32_CHARACTERS",
    "Issuer": "OpenSourceHub",
    "Audience": "OpenSourceHub",
    "ExpiryInMinutes": 1440
  }
}
```

### 5. Start Redis

```bash
# Using Docker
docker run -d --name redis -p 6379:6379 redis:latest

# Or using Homebrew (Mac)
brew services start redis
```

### 6. Run the Application

```bash
cd src/OpenSourceHub.API
dotnet run
```

API will be available at: `http://localhost:5062`

Swagger UI: `http://localhost:5062/swagger`

## 📚 API Documentation

### Authentication

**Login with GitHub**

```
GET /api/auth/github/login
```

**OAuth Callback**

```
GET /api/auth/github/callback?code={code}
```

### User Endpoints (Protected)

**Get Current User**

```
GET /api/user/me
Authorization: Bearer {token}
```

**Get User Profile**

```
GET /api/user/profile
Authorization: Bearer {token}
```

### Issue Discovery (Protected)

**Search Issues**

```
GET /api/issues/search?language=javascript&labels=good-first-issue&minimumStars=100&page=1&pageSize=20
Authorization: Bearer {token}
```

## 🗂️ Project Structure

```
OpenSourceHub/
├── src/
│   ├── OpenSourceHub.API/              # API Layer (Controllers, Middleware)
│   ├── OpenSourceHub.Application/      # Business Logic (CQRS, Use Cases)
│   ├── OpenSourceHub.Domain/           # Domain Entities & Business Rules
│   └── OpenSourceHub.Infrastructure/   # External Concerns (DB, Services)
├── tests/
│   └── OpenSourceHub.Tests/
└── README.md
```

## 🤝 Contributing

This is a personal project built for learning and portfolio purposes. However, suggestions and feedback are welcome!

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

## 👨‍💻 Author

**Karan Chadha**

- GitHub: [@KaranChadha10](https://github.com/KaranChadha10)
- LinkedIn: [Your LinkedIn](https://www.linkedin.com/in/your-profile)
- Email: karan10chadha@gmail.com

## 🙏 Acknowledgments

- Built as part of learning Clean Architecture and Domain-Driven Design
- Uses GitHub's REST API via Octokit
- Inspired by the need to make open source contributions more discoverable

---

⭐ Star this repo if you find it helpful!
