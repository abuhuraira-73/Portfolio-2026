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

---
## Update 3: Dynamic 'About Me' Education Section

To make the "About" page content manageable without code changes, the Education section was made fully dynamic, allowing new entries to be added and removed directly from the admin panel.

### 7.1. Architecture Refactor: Singleton Database Service
During development, silent database failures were encountered. The root cause was traced to how the database connection was being managed by the dependency injection system (`AddScoped<IMongoDatabase>`). To fix this permanently and improve stability, the architecture was refactored to use a dedicated, long-lived service.

-   **`IDatabaseService` & `MongoDbService`**: An interface and a concrete class were created in the `Services/` directory. This service encapsulates all database logic (getting admins, getting educations, adding, deleting, etc.).
-   **Singleton Registration**: In `Program.cs`, the application was reconfigured to register `MongoDbService` as a singleton (`AddSingleton`). This ensures a single, stable instance of the service handles all database requests for the application's entire lifetime, resolving the connection issues.
-   **Controller Refactoring**: Both `AdminController` and `HomeController` were updated to inject `IDatabaseService` instead of `IMongoDatabase`, decoupling them from direct database access and centralizing the logic.

### 7.2. Data Models for Education
Two new models were created to support this feature:
-   **`Education.cs`**: The primary model representing an education entry in the `Educations` MongoDB collection. It includes properties for `Id`, `Year`, `Course`, `College`, `Description`, and `DisplayOrder`.
-   **`EducationInputModel.cs`**: A dedicated model (DTO/ViewModel) for form submissions. It contains all the fields from the `Education` model *except* the `Id`, which is generated by the database. This was a critical addition to solve `ModelState` validation errors.

### 7.3. Admin Panel UI (`Admin/Index.cshtml`)
The admin dashboard was enhanced with a new "About" tab to manage the Education section.
-   **Add Education Form**: A form was added with fields for `Year`, `Course`, `College`, `Description`, and `DisplayOrder`. The UI was designed using elements from the project's template (`job-application-form-dark.html`) for visual consistency.
-   **Existing Entries List**: Below the form, a new section was added to display all existing education entries from the database. The layout for each item was designed to mirror the public-facing `About.cshtml` page, ensuring a consistent look and feel between the admin and public views. Each item includes a "Delete" button.

### 7.4. Controller Logic (`AdminController.cs`)
Two new actions were added to handle the new functionality:
-   **`AddEducation`**: An `[HttpPost]` action that receives the `EducationInputModel`. It validates the input, creates a full `Education` object from the input model, and passes it to the database service to be saved.
-   **`DeleteEducation`**: An `[HttpPost]` action that takes the `Id` of an entry and calls the database service to remove it from the `Educations` collection.

### 7.5. Public View (`HomeController.cs` & `About.cshtml`)
To display the data, the public "About" page was updated:
-   The `About` action in `HomeController` now calls the database service to fetch a list of all education entries, sorted by `DisplayOrder`.
-   This list is passed to the `About.cshtml` view.
-   In `About.cshtml`, a `@foreach` loop was added after the two existing static entries. This loop renders each `Education` object from the database, using the same HTML structure and CSS as the static items to ensure a seamless look.

### 7.6. Challenges & Resolutions
Implementing this feature revealed a significant and persistent bug that required architectural changes to solve.
-   **Initial Problem**: Adding a new education entry from the admin panel would refresh the page, but the data would not be saved, and no new entry would appear. No errors were immediately obvious in the browser or logs.
-   **Investigation**: By adding detailed logging to the `AddEducation` action, we discovered the root cause: `ModelState.IsValid` was consistently returning `false`, preventing the database save operation from ever being called.
-   **Incorrect Diagnosis**: The initial assumption was that other properties on the shared `AdminDashboardViewModel` (like the `NewCvFile`) were causing the validation to fail. This was incorrect.
-   **Correct Diagnosis**: After further logging of the `ModelState` errors themselves, the specific error was identified: **`Error: Key=NewEducation.Id, Error=The Id field is required.`** The `Education` model's `Id` property, being a non-nullable string, was being treated as a required field by the model binder. Since the "Add" form doesn't (and shouldn't) provide an `Id`, the validation failed every time.
-   **Final Solution**: The issue was resolved by implementing the correct software design pattern for this scenario. A dedicated `EducationInputModel` was created without the `Id` property. The `AddEducation` controller action was changed to accept this new input model. This correctly separates the data required *from the form* from the data model required *by the database*, finally resolving the `ModelState` validation error and allowing the data to be saved successfully.

---
## Update 4: Dynamic 'About Me' Experience Section

Following the successful implementation for Education, the same dynamic functionality was applied to the "Experience" section of the "About" page.

### 8.1. Data Models for Experience
-   **`Experience.cs`**: A new model was created to represent a work experience entry in the `Experiences` collection, containing `Id`, `Year`, `Role`, `Company`, `Description`, and `DisplayOrder`.
-   **`ExperienceInputModel.cs`**: A corresponding DTO was created for form submissions, containing all properties except for the database-generated `Id`.

### 8.2. Admin Panel and Service Layer Updates
-   **Admin UI**: The "About" tab in `Admin/Index.cshtml` was extended to include a new form for adding experiences and a separate list for displaying and deleting existing ones, mirroring the education UI.
-   **Database Service**: The `IDatabaseService` and `MongoDbService` were updated to include methods for `GetExperiences`, `AddExperience`, and `DeleteExperience`, and to manage the new `Experiences` collection.
-   **Admin Controller**: The `AdminController` was updated to fetch the list of experiences for the dashboard and to include `AddExperience` and `DeleteExperience` actions to handle the new form submissions.

### 8.3. Public View (`About.cshtml`)
-   **`AboutPageViewModel`**: To pass multiple lists of data to the view, a new `AboutPageViewModel.cs` was created to act as a container for both the `List<Education>` and `List<Experience>`.
-   **Controller Update**: The `HomeController`'s `About` action was modified to fetch both lists from the database service and pass the new `AboutPageViewModel` to the view.
-   **View Update**: The `About.cshtml` view was changed to use the new view model. A `@foreach` loop was added after the three static, hardcoded experience entries to dynamically render any additional experiences from the database, ensuring a consistent look.

---
## Update 5: Dynamic Blog Section

The "Blog" page was converted from a static page to a fully dynamic one, allowing blog posts (in the form of LinkedIn embeds) to be managed from the admin panel.

### 9.1. Data Models for Blog
-   **`BlogPost.cs`**: A model was created to represent a blog post, containing `Id`, `LinkedInEmbedUrl`, `PostDate`, and a `DisplayOrder` property to allow for manual sorting.
-   **`BlogPostInputModel.cs`**: A simple DTO was created for the "Add" form, containing only the `LinkedInEmbedUrl` and `DisplayOrder`. The `PostDate` is set automatically in the controller.

### 9.2. Admin Panel and Service Layer Updates
-   **Admin UI**: The "Blog" tab in `Admin/Index.cshtml` was implemented with a form to add a new LinkedIn embed URL and a `DisplayOrder` number. A list below the form shows all existing posts with their URL, date, order, and a "Delete" button.
-   **Database Service**: The database service layer was extended to manage a new `BlogPosts` collection and included methods for `GetBlogPosts`, `AddBlogPost`, and `DeleteBlogPost`. The `GetBlogPosts` method sorts the results by `DisplayOrder`.
-   **Admin Controller**: `AdminController` was updated to fetch the list of posts and to include `AddBlogPost` and `DeleteBlogPost` actions.

### 9.3. Public View (`Blog.cshtml`)
-   **Controller Update**: The `HomeController`'s `Blog` action was updated to fetch the sorted list of blog posts from the database service.
-   **View Update**: The `Blog.cshtml` file was completely refactored. The static `<iframe>` elements were removed and replaced with a `@model` directive and a `@foreach` loop that dynamically generates the entire grid of embedded posts from the data passed by the controller, maintaining the original responsive layout and styling.
