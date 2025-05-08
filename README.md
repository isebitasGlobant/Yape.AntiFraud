# Yape.AntiFraud  

## Overview  
Yape.AntiFraud is a .NET 8-based solution designed to detect and prevent fraudulent activities. The project is structured to ensure scalability, maintainability, and ease of testing.  

## Architecture  
The solution follows a classic hexagonal architecture, with the following key components:  
- **Domain Layer**: Contains the core business logic and domain entities.  
- **Application Layer**: Handles application-specific logic, such as use cases and service orchestration.  
- **Adapters**: Manages external dependencies, such as database access, APIs, and logging, and provides interfaces for interaction with the system.  
![image](https://github.com/user-attachments/assets/24810a56-c64a-4ba0-962a-2b4f0685e671)



## Solution Structure  
The solution is organized into the following projects:  
- `Yape.AntiFraud.Domain`: Contains domain models and business rules.  
- `Yape.AntiFraud.Application`: Implements use cases and application services.  
- `Yape.AntiFraud.API`: Exposes RESTful APIs for external communication.  

## How to Run the Solution  

### Prerequisites  
- Install Docker.  
- Install Visual Studio with .NET 8 SDK.  

### Steps to Run  
1. **Start Docker**:  
 - Navigate to the root directory of the solution.  
 - Run the following command 'docker-compose up' to build and start the Docker containers:
 - ![image](https://github.com/user-attachments/assets/0438d28e-b2f4-4d66-8a0e-8fc60edd85c5)


2. **Run the Solution**:  
  - Open the solution in Visual Studio.  
  - Set the `Yape.AntiFraud.API` project as the startup project.  
  - Press `F5` or click on the "Start" button to run the solution.  

3. **View Logs**:  
  - Logs will be printed directly to the console in Visual Studio's Output pane.
  - ![image](https://github.com/user-attachments/assets/89bfa03a-357f-4b59-84c3-182645f18479)
  - ![image](https://github.com/user-attachments/assets/b335873e-2f9d-490e-a9f4-3f2411cd2ee6)
  - ![image](https://github.com/user-attachments/assets/772bd321-6940-4296-baf5-fff361cbb35c)

## Notes  
- Ensure Docker is running before starting the solution.  
- The API endpoints can be tested using Swagger or tools like Postman or cURL.
