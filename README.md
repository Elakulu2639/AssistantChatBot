# Intelligent Assistant Chatbot for ERP

A modern chatbot application designed to provide intelligent assistance for ERP (Enterprise Resource Planning) systems. The chatbot uses a combination of pattern matching, fuzzy search, and external API integration to understand and respond to user queries about various ERP modules.

## Features

- **Intelligent Response System**: 
  - Pattern-based intent recognition
  - Fuzzy matching for response selection
  - External API integration for dynamic responses
  - Confidence scoring for response accuracy
  - Natural language processing capabilities

- **ERP Module Support**: Comprehensive coverage of key ERP areas:
  - Inventory Management
    - Stock tracking and management
    - Inventory valuation
    - Stock movement tracking
  - HR Operations
    - Employee management
    - Leave management
    - Payroll information
  - Financial Management
    - Accounting operations
    - Financial reporting
    - Budget management
  - Sales Operations
    - Order processing
    - Customer management
    - Sales analytics
  - Purchase Management
    - Vendor management
    - Purchase order processing
    - Procurement tracking

- **User Interface**:
  - Modern, responsive design using React
  - Real-time chat interface with message history
  - Message status indicators (sent, received, processed)
  - Support for both light and dark themes
  - Mobile-friendly layout

- **API Features**:
  - RESTful API architecture
  - Swagger/OpenAPI documentation
  - Secure endpoint authentication
  - Rate limiting and request validation
  - Comprehensive error handling

- **Development Features**:
  - Hot reloading for both frontend and backend
  - Integrated development environment support
  - Comprehensive logging system
  - Debug mode support
  - Cross-platform compatibility

- **Database Integration**:
  - SQL Server 2022 support
  - Entity Framework Core integration
  - Data persistence and caching
  - Transaction management
  - Data validation and integrity checks

## Technology Stack

### Backend
- ASP.NET Core 9
- C#
- Swagger/OpenAPI
- Machine Learning Model for Response Generation
- Entity Framework Core (for future database integration)
- SQL Server 2022

### Frontend
- React
- Vite
- Modern CSS with Flexbox and Grid
- React Router for navigation
- Axios for API communication

## Getting Started

### Prerequisites
- .NET 7.0 SDK
- Node.js 16+
- npm or yarn
- Visual Studio 2022 or Visual Studio Code (recommended)
- SQL Server 2022

### Running in Visual Studio
1. Open the solution file `ChatBot.sln` in Visual Studio 2022
2. Set `ChatBot.Server` as the startup project
3. Click the "Start" button or press F5 to run both backend and frontend
4. The application will automatically:
   - Start the backend server at `http://localhost:7105`
   - Launch the frontend at `http://localhost:54439`
   - Open Swagger UI at `http://localhost:7105/swagger`

### Manual Setup

#### Backend Setup
1. Navigate to the server directory:
   ```bash
   cd ChatBot.Server
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Build the project:
   ```bash
   dotnet build
   ```
4. Run the application:
   ```bash
   dotnet run
   ```
5. Access Swagger UI at: `http://localhost:7105/swagger`

#### Frontend Setup
1. Navigate to the client directory:
   ```bash
   cd chatbot.client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Create a `.env` file in the client directory with:
   ```
   VITE_API_URL=http://localhost:7105
   ```
4. Run the development server:
   ```bash
   npm run dev
   ```
5. Access the application at: `http://localhost:54439`

## Project Structure

```
ChatBot/
├── ChatBot.Server/           # Backend API
│   ├── Controllers/         # API endpoints
│   ├── Models/             # Data models
│   ├── Services/           # Business logic
│   └── Program.cs          # Application entry point
└── chatbot.client/         # Frontend React application
    ├── src/
    │   ├── components/     # React components
    │   ├── services/      # API services
    │   └── App.tsx        # Main application component
    └── public/            # Static assets
```

## API Documentation

The API documentation is available through Swagger UI when running the backend server. Key endpoints include:

- `POST /api/chat`: Send messages to the chatbot
  - Request body: `{ "message": "your question here" }`
  - Response: `{ "response": "answer", "category": "module", "confidence": 0.95 }`

## Development

### Running Tests
```bash
# Backend tests
cd ChatBot.Server
dotnet test

# Frontend tests
cd chatbot.client
npm test
```

### Code Style
- Backend: Follow C# coding conventions
- Frontend: Follow React best practices and use TypeScript

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request 