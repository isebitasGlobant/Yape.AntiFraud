# Yape.AntiFraud# Yape.AntiFraud  

## Overview  
Yape.AntiFraud is a .NET 8-based solution designed to detect and prevent fraudulent activities. The project is structured to ensure scalability, maintainability, and ease of testing.  

## Architecture  
The solution follows a modular architecture, with the following key components:  
- **Domain Layer**: Contains the core business logic and domain entities.  
- **Application Layer**: Handles application-specific logic, such as use cases and service orchestration.  
- **Infrastructure Layer**: Manages external dependencies, such as database access, APIs, and logging.  
- **Presentation Layer**: Exposes APIs or user interfaces for interaction with the system.  

## Solution Structure  
The solution is organized into the following projects:  
- `Yape.AntiFraud.Domain`: Contains domain models and business rules.  
- `Yape.AntiFraud.Application`: Implements use cases and application services.  
- `Yape.AntiFraud.Infrastructure`: Handles data persistence, external integrations, and configuration.  
- `Yape.AntiFraud.API`: Exposes RESTful APIs for external communication.  

## How to Run the Solution  

### Prerequisites  
- Install Docker.  
- Install Visual Studio with .NET 8 SDK.  

### Steps to Run  
1. **Start Docker**:  
  - Navigate to the root directory of the solution.  
  - Run the following command to build and start the Docker containers:
2. **Run the Solution**:  
   - Open the solution in Visual Studio.  
   - Set the `Yape.AntiFraud.API` project as the startup project.  
   - Press `F5` or click on the "Start" button to run the solution.  

3. **View Logs**:  
   - Logs will be printed directly to the console in Visual Studio's Output pane.  

## Testing  
- The solution includes unit tests and integration tests.  
- To run the tests, open the Test Explorer in Visual Studio and execute all tests.  

## Notes  
- Ensure Docker is running before starting the solution.  
- The API endpoints can be tested using tools like Postman or cURL.
