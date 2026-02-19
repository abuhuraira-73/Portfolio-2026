# Report: Backend and Admin Panel Setup

This document provides a detailed overview of the backend architecture and admin panel functionality implemented for the portfolio website.

## 1. Project Overview

The goal was to create a secure, functional backend and admin panel to manage portfolio content dynamically. The key features implemented are:
- **MongoDB Integration:** For storing manageable content, starting with the portfolio resume/CV.
- **Admin Panel:** A secure, single-page dashboard for the site owner to manage content without touching the code.

---

## 2. Backend Setup: MongoDB Integration

The backend is configured to use MongoDB as the database.

### 2.1. Configuration (`appsettings.json`)

The `appsettings.json` file was updated to include two new configuration sections:

-   **`MongoDbSettings`**: This section holds the connection details for the database.
    -   `ConnectionString`: The full URI to connect to the MongoDB instance.
    -   `DatabaseName`: The name of the database to use (e.g., `portfolio-2026`).
    -   `ResumeCollectionName`: The name of the collection to store the CV file (e.g., `index-page`).

-   **`AdminCredentials`**: This section stores the login details for the admin panel.
    -   `Username`: The administrator's username.
    -   `Password`: The administrator's password.

### 2.2. Service Configuration (`Program.cs`)

To make the database connection available throughout the application, dependency injection was configured in `Program.cs`:
-   The official `MongoDB.Driver` NuGet package was added to the project.
-   **`IMongoClient`** is registered as a singleton service, creating a single, persistent connection to the database based on the `ConnectionString`.
-   **`IMongoDatabase`** is registered as a scoped service, providing access to the specific database instance for each web request.

### 2.3. Data Models (`/Models`)

Several C# classes (models) were created to define the structure of our data:
-   `ResumeFile.cs`: Defines the structure for storing the CV in MongoDB, including properties for the filename, content type, and the binary content of the file itself (`byte[]`).
-   `LoginViewModel.cs`: Represents the data required for the admin login form (Username, Password).
-   `AdminDashboardViewModel.cs`: A dedicated model for the single-page admin dashboard, used to pass data (like the current CV filename and the new upload file) between the controller and the view.

---

## 3. Admin Panel Implementation

The admin panel was designed as a secure, single-page dashboard for easy content management.

### 3.1. Authentication (`Program.cs` & `AdminController.cs`)

A standard and secure cookie-based authentication system was implemented:
-   **In `Program.cs`**: ASP.NET Core's Cookie Authentication service was added. It's configured to automatically redirect any unauthorized access attempts on protected pages to the `/Admin/Login` URL.
-   **In `AdminController.cs`**:
    -   The `Login` (POST) action verifies the submitted username and password against the credentials in `appsettings.json`.
    -   On successful validation, it creates an encrypted authentication cookie and sends it to the user's browser, officially signing them in.
    -   The `Logout` action clears this cookie, signing the user out.
    -   The entire `AdminController` is decorated with the `[Authorize]` attribute, ensuring that only authenticated users can access its actions (except for the `Login` page, which is marked with `[AllowAnonymous]`).

### 3.2. Layouts and Views (`/Views`)

-   **`_AdminLayout.cshtml`**: This is the foundational layout for the admin panel. It's a minimal "shell" that contains all the theme's required CSS, JavaScript, and core HTML wrappers (`preloader`, `smooth-wrapper`, etc.) to ensure the design is consistent. It does **not** contain the public site's header or footer. It simply renders the content of the view within its structure.
-   **`Admin/Login.cshtml`**: This is a self-contained page (`Layout = null;`) that includes the full HTML structure and assets needed for the theme, ensuring it looks correct. It contains the functional Razor form for logging in.
-   **`Admin/Index.cshtml`**: This is the single-page dashboard.
    -   It uses the `_AdminLayout.cshtml`.
    -   It contains the HTML for a **tabbed interface** based on the `profile-dark.html` template.
    -   The side navigation tabs have been renamed to correspond with the main pages of the portfolio (Home, About, Blog, etc.).
    -   The first tab, "Home", contains the functionality to manage the CV, including displaying the current CV and a form to upload a new one.
    -   Other tabs contain placeholder content for future development.

### 3.3. Controller Logic (`AdminController.cs`)

The `AdminController` was refactored to support the single-page dashboard:
-   The `[HttpGet] Index` action now prepares the `AdminDashboardViewModel`. It checks the database for the currently uploaded CV and passes its filename to the view.
-   The `[HttpPost] Index` action handles the form submission for the CV upload. When a new PDF is submitted, this action validates it, replaces the old file in the database with the new one, and then re-displays the dashboard with a success message.

---
## 4. How to Use The Admin Panel

1.  Navigate to your site and click the "Admin" link in the footer, or go directly to `/Admin/Login`.
2.  Log in using the credentials you have set in `appsettings.json`.
3.  You will be redirected to the tabbed admin dashboard.
4.  The "Home" tab is active by default and contains the CV management section.
5.  To upload a new CV, click "Choose File", select your new PDF, and click "Upload & Replace CV". The page will refresh and show the new filename.

---
## Update 1: Database-Driven Admin Authentication

To enhance security and flexibility, the admin authentication process was migrated from hardcoded credentials in `appsettings.json` to the MongoDB database.

### 5.1. New Data Model (`Admin.cs`)
A new model was created at `Models/Admin.cs` to represent an administrator in the database. This class includes properties for `Id`, `Username`, and `Password`, with appropriate BSON attributes for MongoDB mapping.

### 5.2. `Admins` Collection
A new collection named `Admins` was created in the `portfolio-2026` database. This collection stores the admin user documents. A seeding script was used to insert the initial admin user with credentials provided by the site owner.

### 5.3. Refactored Login Logic (`AdminController.cs`)
The `[HttpPost] Login` action in the `AdminController` was significantly modified:
- It no longer reads credentials from `IConfiguration` (`appsettings.json`).
- It now injects `IMongoDatabase` to get access to the `Admins` collection.
- Upon a login attempt, it queries the `Admins` collection for a user with a matching username.
- If a user is found, it compares the submitted password with the password stored in the database document.
- The `AdminCredentials` section was subsequently removed from `appsettings.json`.

---
## Update 2: File System-Based CV Management

The handling of the portfolio resume/CV was refactored to use the server's file system instead of storing the file directly in MongoDB. This approach is more efficient for serving files and simplifies file management.

### 6.1. Upload Logic (`AdminController.cs`)
The `[HttpPost] Index` action, which handles the CV upload, was updated:
- The `IWebHostEnvironment` service is now injected into the controller to resolve physical file paths.
- Instead of writing to MongoDB, the action now calculates the path to the `wwwroot/Resume/` directory.
- The logic ensures this directory exists, deletes any pre-existing `.pdf` files within it, and saves the newly uploaded file in their place.
- All MongoDB-related code for `ResumeFile` was removed from this controller.

### 6.2. Download Logic (`HomeController.cs`)
The `DownloadCv` action in the `HomeController` was simplified:
- It no longer queries the database.
- It now uses `IWebHostEnvironment` to find the `wwwroot/Resume/` directory.
- It searches for the first `.pdf` file in the directory, reads its contents into a stream, and returns it to the user's browser as a file download.

### 6.3. Obsolete Code Removal
The temporary `UploadResumeToDb` action was removed from `HomeController.cs`, and the `ResumeFile.cs` model is no longer actively used by the controllers, as the file content is no longer stored in the database.
