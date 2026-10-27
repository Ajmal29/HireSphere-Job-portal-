# HireSphere — Job Search Portal

HireSphere is a web application designed to bring job seekers and employers together through a centralized recruitment platform. It provides a structured foundation for candidate registration, company registration, job discovery, vacancy posting, and job applications.

The project follows the ASP.NET MVC architecture, organizing application responsibilities into models, views, and controllers.

## Project Objective

The goal of HireSphere is to simplify the interaction between candidates looking for opportunities and companies looking for applicants.

The application is organized around two main user groups:

- **Job seekers:** Candidates who register, maintain their information, explore vacancies, and apply for suitable opportunities.
- **Employers:** Companies that register and publish job opportunities.

## Application Modules

The project structure includes controllers and models associated with the following areas. Their complete behavior should be verified against the source code and database.

### 1. Public Website

The landing page introduces HireSphere and its purpose with a career-focused design.

Visible navigation includes:

- Home
- About
- Contact
- Signup
- Login

The homepage presents the message: “Find Your Dream Job. Discover Opportunities. Build Your Future.”

### 2. User Registration

The candidate registration area is represented by `UserRegController` and the `UserInsert` model.

This module is intended to handle the information required to register a job seeker.

### 3. Company Registration

The employer registration area is represented by `CompanyRegController` and the `CompanyInsert` model.

This module provides the foundation for companies to create an account and participate in the recruitment workflow.

### 4. Login

The project includes `LoginController`, `LoginDBController`, and `LoginClass`.

These components organize the login-related workflow. Authentication checks, session handling, and access restrictions require source-code verification.

### 5. User and Company Home Areas

The project contains separate `UserHomeController` and `CompanyHomeController` classes.

These provide separate entry points for candidate-facing and company-facing functionality after login.

### 6. Job Posting

`JobInsertController` and the `JobInsert` model represent the job-posting area.

This module is intended to handle vacancy information submitted through the employer workflow.

### 7. Job Viewing and Search

The project includes `JobViewController`, `DisplayAllController`, and a `Jobsearch` model.

These components indicate functionality related to displaying and searching job information. The available search fields and filtering rules should be documented after reviewing the implementation.

### 8. Job Applications

`AddApplicationController` and `Applicationcls` represent the application-submission area.

This module is intended to connect a candidate’s application with a job opportunity.

### 9. User Profile

`UserProfileDBController` and `ProfileCls` organize profile-related functionality.

The exact profile fields and supported editing operations depend on the implementation.

### 10. Password Changes

`ChangePwdDBController` and `ChangePwdCls` represent the password-change workflow.

Password validation, storage, and security behavior should be reviewed before making security claims about this feature.

## MVC Architecture

### Models

Models represent the data used by application features, including registration, login, jobs, applications, and profiles.

Examples visible in the project include:

- `UserInsert`
- `CompanyInsert`
- `LoginClass`
- `JobInsert`
- `Jobsearch`
- `Applicationcls`
- `ProfileCls`
- `ChangePwdCls`

### Views

Views provide the pages users interact with, including the public website and the forms or screens associated with application modules.

### Controllers

Controllers handle incoming requests and coordinate the relevant application operations and responses.

The project separates responsibilities across controllers for registration, login, job posting, job viewing, applications, profiles, and account changes.

## Routing

The visible route configuration uses the conventional MVC route:

`{controller}/{action}/{id}`

The default route opens the `Index` action of `HomeController`, with an optional `id` parameter.

## Development Environment

The screenshots show:

- C#
- Classic ASP.NET MVC project structure
- Microsoft Visual Studio
- IIS Express for local execution

The exact .NET Framework version, database configuration, and package dependencies should be taken from the project files.

## Intended User Journey

### Job Seeker

Register → Log in → Open the user area → Explore jobs → Submit an application.

### Employer

Register a company → Log in → Open the company area → Publish job opportunities.

These journeys describe the intended module flow; complete end-to-end behavior should be verified by running the application.

## Possible Future Enhancements

The following are improvement ideas, not claims about existing functionality:

- Saved jobs and candidate shortlists
- Application status tracking
- Email notifications
- Pagination and advanced search filters
- Resume upload with file validation
- Employer application review tools
- Automated tests for key workflows
- Stronger authentication and authorization controls

## Author

Ajmal N K
