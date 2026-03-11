# Performance Optimization Plan: Caching

This file outlines the plan to improve site performance by implementing browser caching and other optimizations.

---

### Step 1: Implement Browser Caching for Static Assets

**Objective:** Configure the web application to send `Cache-Control` headers for static files (like CSS, JavaScript, fonts, and images served from your server). This instructs the user's browser to save a local copy of these files, preventing them from being re-downloaded on every page load and dramatically speeding up repeat visits.

**Step 1.1: Modify `Program.cs` to Configure Static File Caching**
-   **What:** We will modify the `Program.cs` file to add caching headers to all static files served from the `wwwroot` directory.
-   **How:**
    1.  Target file: `VS-portfolio-2026/Program.cs`.
    2.  Locate the existing line of code: `app.UseStaticFiles();`.
    3.  We will replace this simple line with a configuration block that specifies caching options. The new code will look like this:
        ```csharp
        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                // Cache static files for one year
                const int durationInSeconds = 60 * 60 * 24 * 365;
                ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={durationInSeconds}");
            }
        });
        ```
    This change tells the server to add a header to every static file response, telling the browser to cache it for one year.

---

### Step 2: Defer Loading of Non-Critical CSS (Updated)

**Objective:** Prevent non-critical CSS files from blocking the initial rendering while ensuring the main layout remains stable.

**Step 2.1: Implement Hybrid Loading**
- **Critical CSS (Blocking)**: `bootstrap.min.css` and `main.css` have been reverted to standard blocking links. This is necessary to prevent **FOUC (Flash of Unstyled Content)**, ensuring the site doesn't appear as unstructured text for 0.5s on load.
- **Non-Critical CSS (Deferred)**: Files like `font-awesome-pro.css`, `swiper-bundle.css`, `magnific-popup.css`, and `nice-select.css` continue to use the `preload` pattern to speed up the initial page draw.

---

### Step 3: Optimize Web Font Loading

**Objective:** Fix the "Font display" warning to ensure text is visible while your custom web fonts are loading.

**Step 3.1: Add `display=swap` to Google Fonts URL**
-   **What:** We need to tell Google Fonts that we want to use the `swap` strategy for font display.
-   **How:** I will find the `<link>` tag in `_Layout.cshtml` that imports your Google Fonts and append `&display=swap` to the URL in its `href` attribute. This is the standard way to resolve this issue with Google Fonts.
