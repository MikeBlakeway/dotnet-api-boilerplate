# DotNet Boilerplate API

A ready-to-use template for building secure, production-grade .NET Web APIs using:

- **.NET 8+**
- **PostgreSQL** (with Entity Framework Core)
- **JWT authentication** (with role-based authorization)
- **User/admin seeding endpoints** (secure by design)
- **Swagger/OpenAPI docs** (fully integrated)
- **Modern best practices** (environment-based secrets, validation, modular structure)
- Ready for **Azure** or container deployment

---

## Features

- **JWT Auth** with roles (admin, manager, user, etc.)
- **User management** endpoints
- **Seeding** via secure endpoints (with “bootstrap” admin workflow)
- **Live API docs** at `/swagger`
- **.env** file support for secrets and connection strings
- **EF Core migrations**
- Azure- and Docker-ready

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [PostgreSQL](https://www.postgresql.org/download/) running locally or remotely
- (Optional) [pgAdmin 4](https://www.pgadmin.org/) for DB UI
- (Recommended) [VS Code](https://code.visualstudio.com/)

---

### 1. Clone the Repo

```bash
git clone <your-repo-url>
cd dotnet-api-boilerplate
```

---

### 2. Set Up Your Environment Variables

Copy `.env.example` to `.env`:

```bash
cp .env.example .env
```

Edit `.env` with your values:

```env
Jwt__Key=YourSuperSecretJWTSigningKey
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=dotnetapi;Username=postgres;Password=yourpassword
```

- `Jwt__Key`: Any long, random string. **Do not commit** your real secret.
- `ConnectionStrings__DefaultConnection`: Your Postgres connection string. **This variable is required and will override any value set in `appsettings.json`.**

---

### 3. Apply EF Core Migrations

```bash
dotnet ef database update
```

---

### 4. Seed Your First Admin User (Bootstrap Step)

This operation is **only available in Development** mode and only if the **database is empty**.

1. Start the app:

```bash
dotnet run
```

2. In Swagger (`/swagger`), or using `curl`/Postman:
   - POST to `/api/seed/admin`
   - Body:

   ```json
   { "password": "YourAdminPassword!" }
   ```

   - Response:

   ```json
   { "message": "Admin user created.", "username": "admin" }
   ```

3. Save your admin password!

---

### 5. Authenticate as Admin

POST to `/api/auth/login` with:

```json
{ "username": "admin", "password": "YourAdminPassword!" }
```

Copy the returned JWT token.

---

### 6. Seed Additional Users (Optional)

Requires admin JWT in header or “Authorize” button in Swagger.

POST to `/api/seed/users` with:

```json
{ "count": 10, "roles": ["User", "Manager"] }
```

Response:

```json
{ "message": "10 users seeded." }
```

---

### 7. Explore and Use the API

Visit `/swagger` for full, interactive API docs.

Example endpoints:

- `GET /api/User` – list all users (auth required)
- `POST /api/auth/login` – authenticate and get a JWT

---

## Security Notes

- Seeding endpoints are only available in Development mode.
- Bulk seeding **always** requires an Admin token.
- Never expose these endpoints in production.
- Always use strong, unique passwords.

---

## Useful Commands

```bash
# Run the app
dotnet run

# Apply migrations
dotnet ef database update

# Add a migration
dotnet ef migrations add MigrationName

# Install a package
dotnet add package <package-name>
```

---

## Project Structure

```
Api/
 ├── Controllers/         # API endpoints
 ├── Data/                # EF DbContext and seeders
 ├── Models/              # DTOs and data models
 ├── appsettings.json     # Base config (safe defaults)
 ├── .env                 # Local secrets (never commit)
 ├── Program.cs           # App entry/config
 └── ...
```

---

## Extending This Template

- Add new endpoints by creating controllers in `Controllers/`
- Protect routes with `[Authorize]` or `[Authorize(Roles="Admin")]`
- Create new models + migrations as needed
- Use Swagger to test and view schemas

---

## Future Enhancements

- Password reset and forgotten password flow
- Magic link / OTP login support
- Refresh tokens and session management
- External auth support (Google, Microsoft, etc.)
- Docusaurus-powered docs site

---

## Documentation Site (Optional)

Want beautiful developer docs?

- Use **Docusaurus**, **MkDocs**, or **docsify**
- Export the Swagger spec from `/swagger/v1/swagger.json`
- Use tools like [Swagger UI](https://swagger.io/tools/swagger-ui/) or [Redoc](https://redocly.com/)

---

## Contributing

Pull requests and suggestions are welcome!
Open an issue to start a conversation.

---

## License

[MIT](./LICENSE)
