# Task Manager

Technical test for **Ballast Lane Applications**. This is a full-stack task management web application using .NET 10, React 19 / TypeScript and MongoDB.

## User Story

> As a tech student working full-time and pursuing an MBA, I want to be able to track my tasks with clear statuses regarding their progress and due dates, so that I can organize my schedule daily and prioritize my work.

The application is a Task Management CRUD, including a user feature with both login and registration.

## Pre-requirements

- **Docker Desktop** (Windows / macOS) or **Docker Engine + Docker Compose** (Linux)
- **WSL 2** (Windows only) required by Docker Desktop. Install with `wsl --install` in PowerShell as admin, then restart.

## Installation, Running & Login

### 1. Clone and start

```bash
git clone https://github.com/andrepalhares/task-manager.git
cd task-manager
docker compose up -d
```

After a few minutes, all services will be up.

- **Frontend**: http://localhost:3000
- **API documentation (Swagger)**: http://localhost:5000/swagger

### 2. Log in with seeded credentials

```
Email:    admin@taskmanager.com
Password: Test@1234
```

The user above already exists in the database and is seeded with 10 sample tasks for testing, but a new user can be created at any time.

### 3. Stop

```bash
docker compose down
```

## Backend Architecture

The backend follows **Clean Architecture** with dependency inversion applied. The Domain layer has no external dependencies. Instead, outer layers depend inward.

```
┌─────────────────────────────────────────┐
│         TaskManager.WebApi              │  Configures DI and
│  (Controllers, Middleware, JWT setup)   │  HTTP routing
└──────────────────┬──────────────────────┘
                   │ depends on
                   ▼
┌─────────────────────────────────────────┐
│       TaskManager.Application           │  Orchestrates business flows and
│  (Use Cases, Validators, Interfaces)    │  defines repository contracts
└──────────────────┬──────────────────────┘
                   │ depends on
                   ▼
┌─────────────────────────────────────────┐
│          TaskManager.Domain             │  Business rules
│   (Entities, Domain Exceptions)         │  and entities
└─────────────────────────────────────────┘
                   ▲
                   │ implements interfaces from Application
                   │
┌─────────────────────────────────────────┐
│      TaskManager.Infrastructure         │  MongoDB driver, BCrypt,
│  (Repositories, Security, Persistence)  │  JWT issuer
└─────────────────────────────────────────┘
```

The projects in the solution are mapped to the assignment's required layers as follows:

- **API layer** → `WebApi`
- **Business logic layer** → `Application` + `Domain`
- **Data layer** → `Infrastructure`

### Key design choices

- **DDD aggregates**: `TaskEntity` and `User` enforce invariants in the constructor, expose private setters, and use static factory methods (`Create`, `Rehydrate`) instead of public constructors, giving more control to the entity itself. Whenever the state of an entity needs to be changed, it can be done by smaller methods like `MarkAsCompleted`, `Rename`, `Reschedule`, with an `Update()` method serving as the orchestrator for the update use case. For future implementations, each of the smaller methods could be used independently if needed.
- **Use case pattern**: A generic `IUseCase<TInput, TOutput>` interface is being used for each use case that implements a single method called `ExecuteAsync`. Replaces MediatR-style handlers without the framework dependency.
- **FluentValidation**: Every input is validated in its specific use case, at the start of the `ExecuteAsync` method.
- **Centralized exception handling**: `GlobalExceptionHandler` middleware is responsible for mapping domain exceptions, validation failures, and auth errors to RFC 7807 `ProblemDetails` responses with appropriate HTTP status codes.
- **Authentication**: Users can be authenticated via `Microsoft.AspNetCore.Authentication.JwtBearer`; passwords hashed with BCrypt.
- **MongoDB driver**: The official `MongoDB.Driver` is used directly, no use of Entity Framework or Dapper.
- **`TaskStatus` as enum, not value object**: A value object could be used to define the status of the task if I wanted to use DDD strictly, but for this application I decided to go with the simplicity of an enum, as the status doesn't have behavior.

## Frontend Architecture

React 19 + TypeScript + Vite, organized by feature.

### Stack

- **Build**: Vite
- **Routing**: React Router v7
- **HTTP**: Axios with request/response interceptors (JWT injection, 401 handling)
- **Styling**: Tailwind CSS
- **Notifications**: Sonner

### Structure

```
frontend/src/
├── features/
│   ├── auth/          # AuthContext, login/register API and its components
│   └── tasks/         # Task CRUD API, hooks, and its components
├── pages/             # Top-level pages (Landing, Tasks and a fallback NotFound)
└── shared/
    ├── api/           # Axios client and interceptors
    ├── components/    # Cross-feature UI (Navbar, Modal, Button, etc.)
    └── routes/        # ProtectedRoute guard
```

The JWT is stored in `localStorage`, decoded client-side to populate `AuthContext`, and attached to every request via an Axios interceptor. A 401 response from the backend triggers automatic logout and redirect to the landing page.

A future improvement could be the creation of an endpoint to retrieve user information, removing the need to decode user data directly from the JWT.

## MongoDB

The application uses **MongoDB 7** with two collections.

### Collections

**`users`**

| Field          | Type   | Notes                                   |
| -------------- | ------ | --------------------------------------- |
| `id`           | UUID   | Domain ID                               |
| `email`        | string | **Unique index** (lowercase-normalized) |
| `passwordHash` | string | BCrypt                                  |
| `name`         | string |                                         |

**`tasks`**

| Field         | Type     | Notes                                      |
| ------------- | -------- | ------------------------------------------ |
| `id`          | UUID     | Domain ID                                  |
| `title`       | string   |                                            |
| `description` | string?  |                                            |
| `status`      | string   | `NotStarted`, `InProgress`, or `Completed` |
| `dueDate`     | ISODate? | Nullable                                   |
| `userId`      | UUID     | Owner reference                            |
| `createdAt`   | ISODate  |                                            |

There's also a compound index on `(userId, createdAt desc)` to support the retrieval of the user's tasks sorted by creation date.

## Testing

### Backend (xUnit + NSubstitute + Shouldly)

111 tests:

| Layer          | Project                            | Coverage                                                     |
| -------------- | ---------------------------------- | ------------------------------------------------------------ |
| Domain         | `TaskManager.Domain.Tests`         | Entity creation, validation, factory rules                   |
| Application    | `TaskManager.Application.Tests`    | All use cases (mocked repositories), validators, error paths |
| Infrastructure | `TaskManager.Infrastructure.Tests` | BCrypt hasher, JWT issuer                                    |
| WebApi         | `TaskManager.WebApi.Tests`         | Controllers, exception middleware, current-user service      |

The current report shows 80.7% line coverage and 88.2% branch coverage.

To run all tests:

```bash
dotnet test
```

To reproduce the coverage report, run this inside `/backend`:

```bash
dotnet test --collect:"XPlat Code Coverage"

# one-time install
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html
```

Open file `coveragereport/index.html` to view the report.

### Frontend (Vitest + Testing Library)

21 component and page tests using snapshot testing for rendering correctness, with `useAuth`, `useNavigate`, and API modules mocked.

To run all tests:

```bash
npm test
```

## GenAI Tools

The tech test includes a prompt-engineering deliverable, separate from the application. While developing this project, I've used GitHub Copilot (with Visual Studio and VS Code) for code generation, and Claude AI for architectural decisions, design discussions, and reviewing AI-generated code.

The full documentation including prompts used, output samples and the critical evaluation of each is present in file `GENAI.md`.

## Future Improvements

Possible future improvements include:

- **Refresh tokens**: The current JWTs expire after 60 minutes with no refresh flow, so users have to re-authenticate.
- **Task filtering and search**: `GET /tasks` endpoint works with pagination, but it could be improved by adding support to filter by status or name and custom ordering.
- **Frontend UI/UX polish**: The current interface is functional but minimal, some design changes would give it a better user experience.
