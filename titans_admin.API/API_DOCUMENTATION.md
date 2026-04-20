<!-- API Documentation - Titans Admin API -->

# Titans Admin API Endpoints

This document outlines all the REST API endpoints available in the Titans Admin API project.

## Base URL
```
https://localhost:<port>/api
```

---

## 1. Health Check

### Get Health Status
- **Endpoint**: `GET /api/health`
- **Description**: Check API health status
- **Response**: 
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-01T12:00:00Z"
}
```

---

## 2. Admin Dashboard

### Get Dashboard Statistics
- **Endpoint**: `GET /api/adminapi/dashboard`
- **Description**: Retrieve dashboard statistics including total licenses, active programs, transactions, and violations
- **Response**: AdminDashboardViewModel

### Get Violations Count
- **Endpoint**: `GET /api/adminapi/violations-count`
- **Description**: Get the count of compliance violations
- **Response**:
```json
{
  "violationCount": 5
}
```

---

## 3. Users Management

### Get All Users
- **Endpoint**: `GET /api/users`
- **Description**: Retrieve all users
- **Response**: List of UserListItemViewModel

### Get User by ID
- **Endpoint**: `GET /api/users/{id}`
- **Description**: Retrieve a specific user
- **Parameters**: 
  - `id` (int): User ID
- **Response**: UserEditViewModel

### Create User
- **Endpoint**: `POST /api/users`
- **Description**: Create a new user
- **Request Body**: CreateUserViewModel
- **Response**: HTTP 201 Created with userId

### Update User
- **Endpoint**: `PUT /api/users/{id}`
- **Description**: Update user details
- **Parameters**: 
  - `id` (int): User ID
- **Request Body**: UserEditViewModel
- **Response**: HTTP 200 OK

### Delete User
- **Endpoint**: `DELETE /api/users/{id}`
- **Description**: Delete a user
- **Parameters**: 
  - `id` (int): User ID
- **Response**: HTTP 200 OK

### Get Users by Role
- **Endpoint**: `GET /api/users/by-role/{role}`
- **Description**: Get users filtered by role
- **Parameters**: 
  - `role` (string): UserRole enum value (Admin, Business, Officer, Manager, Compliance, Auditor)
- **Response**: List of UserListItemViewModel

### Get Users by Status
- **Endpoint**: `GET /api/users/by-status/{status}`
- **Description**: Get users filtered by status
- **Parameters**: 
  - `status` (string): UserStatus enum value (Active, Inactive, Suspended, PendingApproval)
- **Response**: List of UserListItemViewModel

### Update User Status
- **Endpoint**: `PATCH /api/users/{id}/status`
- **Description**: Update user's account status
- **Parameters**: 
  - `id` (int): User ID
- **Request Body**: 
```json
{
  "status": "Active",
  "modifiedByUserId": 1
}
```
- **Response**: HTTP 200 OK

---

## 4. Trade Licenses Management

### Get All Trade Licenses
- **Endpoint**: `GET /api/tradelicenses`
- **Description**: Retrieve all trade licenses
- **Response**: List of TradeLicenseListViewModel

### Get Trade License by ID
- **Endpoint**: `GET /api/tradelicenses/{id}`
- **Description**: Retrieve a specific trade license
- **Parameters**: 
  - `id` (int): Trade License ID
- **Response**: TradeLicenseEditViewModel

### Create Trade License
- **Endpoint**: `POST /api/tradelicenses`
- **Description**: Create a new trade license
- **Request Body**: TradeLicenseEditViewModel
- **Response**: HTTP 201 Created with licenseId

### Update Trade License
- **Endpoint**: `PUT /api/tradelicenses/{id}`
- **Description**: Update trade license details
- **Parameters**: 
  - `id` (int): Trade License ID
- **Request Body**: TradeLicenseEditViewModel
- **Response**: HTTP 200 OK

### Delete Trade License
- **Endpoint**: `DELETE /api/tradelicenses/{id}`
- **Description**: Delete a trade license
- **Parameters**: 
  - `id` (int): Trade License ID
- **Response**: HTTP 200 OK

### Get Trade Licenses by Status
- **Endpoint**: `GET /api/tradelicenses/by-status/{status}`
- **Description**: Get trade licenses filtered by status
- **Parameters**: 
  - `status` (string): License status (Active, Pending, Expired, etc.)
- **Response**: List of TradeLicenseListViewModel

---

## 5. Trade Programs Management

### Get All Trade Programs
- **Endpoint**: `GET /api/tradeprograms`
- **Description**: Retrieve all trade programs
- **Response**: List of TradeProgramListViewModel

### Get Active Trade Programs
- **Endpoint**: `GET /api/tradeprograms/active`
- **Description**: Retrieve only active trade programs
- **Response**: List of TradeProgramListViewModel

### Get Trade Program by ID
- **Endpoint**: `GET /api/tradeprograms/{id}`
- **Description**: Retrieve a specific trade program
- **Parameters**: 
  - `id` (int): Trade Program ID
- **Response**: TradeProgramEditViewModel

### Create Trade Program
- **Endpoint**: `POST /api/tradeprograms`
- **Description**: Create a new trade program
- **Request Body**: TradeProgramEditViewModel
- **Response**: HTTP 201 Created with programId

### Update Trade Program
- **Endpoint**: `PUT /api/tradeprograms/{id}`
- **Description**: Update trade program details
- **Parameters**: 
  - `id` (int): Trade Program ID
- **Request Body**: TradeProgramEditViewModel
- **Response**: HTTP 200 OK

### Delete Trade Program
- **Endpoint**: `DELETE /api/tradeprograms/{id}`
- **Description**: Delete a trade program
- **Parameters**: 
  - `id` (int): Trade Program ID
- **Response**: HTTP 200 OK

---

## 6. Compliance Records Management

### Get All Compliance Records
- **Endpoint**: `GET /api/compliancerecords`
- **Description**: Retrieve all compliance records
- **Response**: List of ComplianceRecordItemViewModel

### Get Compliance Record by ID
- **Endpoint**: `GET /api/compliancerecords/{id}`
- **Description**: Retrieve a specific compliance record
- **Parameters**: 
  - `id` (int): Compliance Record ID
- **Response**: ComplianceRecordEditViewModel

### Create Compliance Record
- **Endpoint**: `POST /api/compliancerecords`
- **Description**: Create a new compliance record
- **Request Body**: ComplianceRecordEditViewModel
- **Response**: HTTP 201 Created with recordId

### Update Compliance Record
- **Endpoint**: `PUT /api/compliancerecords/{id}`
- **Description**: Update compliance record details
- **Parameters**: 
  - `id` (int): Compliance Record ID
- **Request Body**: ComplianceRecordEditViewModel
- **Response**: HTTP 200 OK

### Delete Compliance Record
- **Endpoint**: `DELETE /api/compliancerecords/{id}`
- **Description**: Delete a compliance record
- **Parameters**: 
  - `id` (int): Compliance Record ID
- **Response**: HTTP 200 OK

### Get Compliance Tracking
- **Endpoint**: `GET /api/compliancerecords/tracking`
- **Description**: Get compliance tracking summary
- **Response**: ComplianceTrackingViewModel

---

## 7. Audit Logs

### Get Audit Logs (Paginated)
- **Endpoint**: `GET /api/auditlogs`
- **Description**: Retrieve paginated audit logs
- **Query Parameters**:
  - `pageNumber` (int, optional, default: 1): Page number
  - `pageSize` (int, optional, default: 50): Records per page (max: 100)
- **Response**: List of AuditLogViewModel

### Get Audit Logs by User
- **Endpoint**: `GET /api/auditlogs/user/{userId}`
- **Description**: Retrieve audit logs for a specific user
- **Parameters**: 
  - `userId` (int): User ID
- **Query Parameters**:
  - `pageNumber` (int, optional, default: 1): Page number
  - `pageSize` (int, optional, default: 50): Records per page (max: 100)
- **Response**: List of AuditLogViewModel

---

## Response Status Codes

| Code | Description |
|------|-------------|
| 200 | OK - Request successful |
| 201 | Created - Resource created successfully |
| 400 | Bad Request - Invalid parameters or validation failed |
| 404 | Not Found - Resource not found |
| 500 | Internal Server Error - Server error |

---

## Error Response Format

```json
{
  "message": "Error description here"
}
```

---

## Features

✅ Complete CRUD operations for Users, Trade Licenses, Trade Programs, and Compliance Records
✅ Filtering by status and role
✅ Pagination support for audit logs
✅ Dashboard statistics
✅ Compliance tracking
✅ Audit logging
✅ CORS enabled for cross-origin requests
✅ OpenAPI/Swagger documentation support
✅ Comprehensive error handling

---

## Usage Example

### Authentication (if required)
Add authentication headers as needed for your deployment.

### Example Request
```bash
curl -X GET "https://localhost:5001/api/users" \
  -H "Content-Type: application/json"
```

### Example Response
```json
[
  {
    "userId": 1,
    "username": "john.doe",
    "email": "john@example.com",
    "fullName": "John Doe",
    "role": "Admin",
    "status": "Active",
    "createdAt": "2024-01-01T00:00:00Z",
    "lastLoginAt": "2024-01-15T12:30:00Z"
  }
]
```

---

## Configuration

The API is configured in `Program.cs` with:
- Entity Framework Core with SQL Server
- CORS enabled for all origins
- OpenAPI/Swagger documentation
- Dependency injection for services and repositories
- HTTP/HTTPS redirection
