<h1>SmartWarehouseManagement</h1>
<b>SmartWarehouseManagement</b> is a warehouse management web app with an ASP.NET backend, 
a React frontend, and PostgreSQL as the database. The app allows authenticated users to add items to the warehouse, 
create and view orders, and create shipments for said orders.
<h2>Why?</h2>
The app was made as a small side project to try developing with a React frontend, with no intention of being more than a project to show recruiters.
<h2>Technologies Used</h2>

### Backend
- **ASP.NET Core (Web API)**
- **ASP.NET Identity** for authentication
- **PostgreSQL** as the database
- **Entity Framework Core** for ORM
- **NUnit** for unit testing

### Frontend
- **React** with functional components
- **TypeScript** for static typing
- **React Router DOM** for navigation

### Database
- **PostgreSQL** (local setup)

<h2>Setup Instructions</h2>

1. **Clone the repository:**

   First, clone the repository to your local machine:
   ```bash
   git clone https://github.com/thoni21/SmartWarehouseManagement.git
   ```

2. **Navigate to the backend folder:**

   Go to the `SmartWarehouseManagement.Server` directory:
   ```bash
   cd SmartWarehouseManagement/SmartWarehouseManagement.Server
   ```

3. **Update the database connection string in `appsettings.json`:**

   Open the `appsettings.json` file and update the connection string to match your local PostgreSQL database setup:
   ```json
   {
       "ConnectionStrings": {
           "DefaultConnection": "Host=localhost;Port=5432;Username=your_username;Password=your_password;Database=your_database"
       }
   }
   ```

4. **Apply migrations to the database:**

   Run the following command to apply any pending migrations to your database:
   ```bash
   dotnet ef database update
   ```

5. **Run the project:**

   Finally, start the project by using the following command:
   ```bash
   dotnet run --launch-profile https
   ```

