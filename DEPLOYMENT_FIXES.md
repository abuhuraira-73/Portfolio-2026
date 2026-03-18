# Deployment Fixes Log - March 14, 2026

This file tracks the changes made to prepare the project for hosting via FileZilla/IIS.

## 1. appsettings.json Update
- **Status:** **COMPLETED**
- **Change:** Added `MongoDbSettings` section with connection string and database name.
- **Reason:** The `MongoDbService.cs` requires these keys to connect to the database. Without them, the app crashes on startup.

## 2. web.config Creation & Configuration
- **Status:** **COMPLETED**
- **Change:** Created `web.config` in the project root and added environment configuration.
- **Details:**
    - Configured `AspNetCoreModuleV2` for IIS.
    - Set `ASPNETCORE_ENVIRONMENT` to `Production`.
- **Reason:** Required by IIS (Windows hosting) to route traffic and ensure the app runs in the correct environment mode.

## 3. Production Build (Publish)
- **Status:** **COMPLETED**
- **Action:** Executed `dotnet publish -c Release -o ./publish`.
- **Outcome:** Created a `./publish` folder containing the compiled binaries, dependencies, and optimized static assets.

## 4. Final Deployment Instructions (FileZilla)
- **Target Folder:** `/Users/abuhuraira/Work/VS-portfolio-2026/publish`
- **Action:** Upload the **contents** of the `publish` folder (not the folder itself) to the server's web root.
- **Verification:** Once uploaded, the domain should correctly load the application and connect to the MongoDB database.

---
*Note: Do not upload the `.cs`, `.csproj`, or `obj/` folders to the server. Only the files inside the `publish` directory are needed.*
