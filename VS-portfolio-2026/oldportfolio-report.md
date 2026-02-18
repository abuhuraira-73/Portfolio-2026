# Backend Report for My-CV-3

This report details the backend structure and implementation of the My-CV-3 portfolio website.

## 1. Project Overview

The backend is an ASP.NET Core 8.0 web application, built using C#. It follows the Model-View-Controller (MVC) architectural pattern. The project was created using the standard `dotnet new mvc` template and then modified. The main project file, `My-CV-3.csproj`, defines the project's dependencies and settings.

## 2. Technology Stack

*   **Framework:** ASP.NET Core 8.0
*   **Language:** C#
*   **Database:** MongoDB (using the `MongoDB.Driver` package)

## 3. How the Backend is Built

The backend is built by wiring together several components, as defined in the project's C# files.

### 3.1. Application Startup (`Program.cs`)

The application's entry point is the `Program.cs` file. This is where the web application is configured and launched. Here's a breakdown of how it's built:

1.  A `WebApplication.CreateBuilder(args)` is used to create a new web application builder.
2.  `builder.Services.AddControllersWithViews()` registers the necessary services for the MVC pattern, enabling the use of controllers and views.
3.  `var app = builder.Build()` creates the application instance.
4.  The following middleware is configured to handle HTTP requests:
    *   `app.UseExceptionHandler("/Home/Error")` and `app.UseHsts()` are used for error handling and security in a production environment.
    *   `app.UseHttpsRedirection()` forces all HTTP requests to be redirected to HTTPS.
    *   `app.UseStaticFiles()` enables the serving of static files (like CSS, JavaScript, and images) from the `wwwroot` directory. A custom `FileExtensionContentTypeProvider` is added to support `.webp` image files.
    *   `app.UseRouting()` enables the routing middleware.
    *   `app.UseAuthorization()` enables the authorization middleware.
    *   `app.MapControllerRoute(...)` configures the default routing pattern for MVC controllers. The pattern is `{controller=Home}/{action=Index}/{id?}`, which means that if no controller or action is specified in the URL, it will default to the `Index` action of the `HomeController`.
5.  `app.Run()` starts the web application and begins listening for HTTP requests.

### 3.2. Handling Requests (`Controllers/HomeController.cs`)

The `HomeController.cs` file contains the logic for handling requests to the home page and other pages. Here's how it's built:

1.  The `HomeController` class is a C# class that inherits from `Microsoft.AspNetCore.Mvc.Controller`.
2.  The constructor of the `HomeController` uses dependency injection to get an `ILogger` for logging and an `IConfiguration` to access the application's settings.
3.  Inside the constructor, the MongoDB connection is established:
    *   `configuration.GetSection("MongoDbSettings")` retrieves the MongoDB settings from `appsettings.json`.
    *   `new MongoClient(mongoSettings["ConnectionString"])` creates a new MongoDB client with the connection string.
    *   `client.GetDatabase(mongoSettings["DatabaseName"])` gets the database instance.
    *   `database.GetCollection<Contact>(mongoSettings["ContactCollectionName"])` gets the collection for storing contact form submissions.
4.  The controller has several "action methods" that correspond to different URLs:
    *   `public IActionResult Index()`: This method is called when a user navigates to the root URL (`/`). It returns the main view for the home page using `return View()`.
    *   `public IActionResult Privacy()`: This method returns the view for the privacy page.
    *   `public IActionResult portfolio()`: This method returns the view for the portfolio page.
    *   `public async Task<IActionResult> SubmitContact([FromForm] Contact contact)`: This method handles the HTTP POST request from the contact form. It is marked with `[HttpPost]` and `[ValidateAntiForgeryToken]` for security.
        *   It takes a `Contact` object as a parameter, which is automatically populated with the form data by ASP.NET Core's model binding.
        *   It checks `if (!ModelState.IsValid)` to ensure the submitted data is valid.
        *   If the data is valid, it sets the `SubmittedAt` property and calls `_contactCollection.InsertOneAsync(contact)` to save the contact information to the MongoDB database.
        *   It returns a JSON response to the client indicating success or failure.

### 3.3. Data Model (`Models/Contact.cs`)

The `Contact.cs` file defines the data structure for the contact form submissions. Here's how it's built:

1.  It's a simple C# class with properties for each field in the contact form.
2.  It uses attributes from the `MongoDB.Bson.Serialization.Attributes` namespace to control how the data is stored in MongoDB:
    *   `[BsonId]` and `[BsonRepresentation(BsonType.ObjectId)]` are used to mark the `Id` property as the primary key and to store it as an `ObjectId` in the database.
    *   `[BsonElement("Name")]` specifies the name of the field in the MongoDB document.
3.  The `SubmittedAt` property is initialized with `DateTime.UtcNow` to automatically set the submission time.

## 4. How it All Works Together

1.  A user navigates to the website in their browser.
2.  The browser sends an HTTP request to the ASP.NET Core web server (Kestrel).
3.  The request goes through the middleware configured in `Program.cs`.
4.  The routing middleware determines which controller and action method should handle the request. For the home page, it's the `Index` method of the `HomeController`.
5.  The `Index` method in `HomeController.cs` is executed, and it returns the `Index.cshtml` view.
6.  The Razor view engine renders the HTML for the page, which is sent back to the browser.
7.  The user fills out the contact form and clicks "submit".
8.  The browser sends an HTTP POST request to the `/Home/SubmitContact` URL, with the form data in the request body.
9.  The `SubmitContact` method in `HomeController.cs` is executed.
10. The method validates the form data, creates a `Contact` object, and saves it to the MongoDB database.
11. A JSON response is sent back to the browser, which is then handled by the JavaScript on the page to show a success or error message.

## 5. SEO (Search Engine Optimization)

The website has a good foundation for SEO, with several best practices implemented in the `Views/Shared/_Layout.cshtml` file.

### 5.1. Meta Tags

The website uses a variety of meta tags to provide information to search engines and other web crawlers:

*   **`<title>`:** The title of the page is set to "Portfolio - Abu Huraira".
*   **`<meta name="description">`:** A detailed description of the portfolio is provided.
*   **`<meta name="keywords">`:** A list of relevant keywords is included.
*   **`<meta name="author">`:** The author of the website is specified.

### 5.2. Open Graph Protocol

The website implements the Open Graph protocol to improve the appearance of shared links on social media platforms like Facebook. The following Open Graph meta tags are used:

*   `og:title`
*   `og:description`
*   `og:image:height`
*   `og:image:width`

### 5.3. Favicons and Icons

A comprehensive set of favicons and icons is provided to ensure the website looks good on different devices and platforms:

*   A standard `favicon.ico`.
*   An `apple-touch-icon` for iOS devices.
*   A `site.webmanifest` file for Android devices.

### 5.4. Responsive Design

The website is designed to be responsive and work well on mobile devices. The following meta tag is used to control the viewport:

*   `<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1">`

### 5.5. Semantic HTML and Accessibility

The views use semantic HTML5 elements like `<section>`, `<h1>`, `<h2>`, etc. This helps search engines understand the structure of the page and improves accessibility. Additionally, all images have `alt` attributes, which is important for both accessibility and SEO.

### 5.6. Performance Optimization

The website uses `.webp` for images, which is a modern image format that provides superior lossless and lossy compression for images on the web. Using `.webp` can help reduce file sizes and improve page load speed, which is a key ranking factor for search engines.
