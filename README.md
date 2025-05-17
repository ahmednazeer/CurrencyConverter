Setup Instructions:
   - restore all nuget packages
   - clear project then build
   - download and install .net8.0

Features

- Real-time currency conversion using multiple exchange rate providers
- Historical exchange rate data with pagination
- JWT authentication and role-based access control
- API throttling and rate limiting
- Distributed tracing with OpenTelemetry
- Resilient architecture with retry policies and circuit breakers
- Environment-specific deployment configurations

Assumptions:
   - Assume that our app saves data in our local DB to help us generate reports for operations on our system
   - Assume that authentication endpoints exsis
Future Enhancements:
   - add authethetication logic
   - add factory method
   - provide docker file
