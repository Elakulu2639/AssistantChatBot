# Intelligent Chatbot Assistance for ERP

A modern chatbot application designed to provide intelligent assistance for ERP (Enterprise Resource Planning) systems. The chatbot uses a trained model to understand and respond to user queries about various ERP modules.

## Features

- **Intelligent Response System**: Uses a trained model to provide context-aware responses
- **ERP Module Support**: Covers key ERP areas:
  - Inventory Management
  - HR Operations
  - Financial Management
  - Sales Operations
  - Purchase Management
- **Confidence Scoring**: Provides confidence levels for responses
- **Modern UI**: Built with React and styled with modern CSS
- **RESTful API**: Built with ASP.NET Core

## Technology Stack

### Backend
- ASP.NET Core 7.0
- C# 11
- Swagger/OpenAPI

### Frontend
- React
- Vite
- Modern CSS

## Getting Started

### Prerequisites
- .NET 7.0 SDK
- Node.js 16+
- npm or yarn

### Backend Setup
1. Navigate to the server directory:
   ```bash
   cd ChatBot.Server
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access Swagger UI at: `http://localhost:5052/swagger`

### Frontend Setup
1. Navigate to the client directory:
   ```bash
   cd chatbot.client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Run the development server:
   ```bash
   npm run dev
   ```
4. Access the application at: `http://localhost:5173`

## API Documentation

The API documentation is available through Swagger UI when running the backend server. Key endpoints include:

- `POST /api/chat`: Send messages to the chatbot
  - Request body: `{ "message": "your question here" }`
  - Response: `{ "response": "answer", "category": "module", "confidence": 0.95 }`

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 