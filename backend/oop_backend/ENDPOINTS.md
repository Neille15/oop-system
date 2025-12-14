# oop_backend API Endpoints

This document lists the HTTP endpoints exposed by the `oop_backend` server.

Base path: `/api`

## AttendancesController (Route: `api/Attendances`)

- **GET** `/api/Attendances`
  - Description: Get all attendance records.
  - Response: `200` JSON array of `Attendance` objects.

- **GET** `/api/Attendances/{id}`
  - Description: Get a single attendance record by `attendanceID`.
  - Response: `200` `Attendance` or `404` if not found.

- **PUT** `/api/Attendances/{id}`
  - Description: Update an attendance record.
  - Body: JSON `Attendance` object.
  - Response: `204` on success, `400` or `404` on error.

- **POST** `/api/Attendances/recordAttendance`
  - Description: Record a time-in/time-out via face verification.
  - Content-Type: `multipart/form-data`
  - Form fields:
    - `Photo` (file) — required image file of face
    - `Type` (string) — required, either `time-in` or `time-out`
  - Behavior: Calls the configured face recognition service (`FaceRecognitionServiceUrl`) at `/verify` and, if verified, creates an attendance record.
  - Responses: `201` created on success, `400` for validation/verification failures, `503` if face service unavailable.

- **POST** `/api/Attendances`
  - Description: Create an attendance record (standard JSON create).
  - Body: JSON `Attendance`.
  - Response: `201` created.

- **DELETE** `/api/Attendances/{id}`
  - Description: Delete an attendance record.
  - Response: `204` on success, `404` if not found.

## UserDatasController (Route: `api/UserDatas`)

- **GET** `/api/UserDatas`
  - Description: Get all users.
  - Response: `200` JSON array of `UserData` objects.

- **GET** `/api/UserDatas/{id}`
  - Description: Get a user by `userID`.
  - Response: `200` `UserData` or `404` if not found.

- **PUT** `/api/UserDatas/{id}`
  - Description: Update user data.
  - Body: JSON `UserData` object.
  - Response: `204` on success, `400` or `404` on error.

- **POST** `/api/UserDatas`
  - Description: Register a new user and add face to face-recognition service.
  - Content-Type: `multipart/form-data`
  - Form fields:
    - `Photo` (file) — required image file of face
    - `FirstName` (string) — required
    - `LastName` (string) — required
    - `Email` (string) — required
    - `BirthDate` (string) — required (stored as string)
    - `StudentNumber` (string) — optional (falls back to `Email`)
  - Behavior: Creates user in DB, then calls face recognition endpoint `/addFace?id={userId}` on configured `FaceRecognitionServiceUrl`. If face registration fails, user creation is rolled back.
  - Responses: `201` created, `400` for validation, `503` if face service unavailable.

- **DELETE** `/api/UserDatas/{id}`
  - Description: Delete a user.
  - Response: `204` on success, `404` if not found.

## Face recognition service (called by backend)

- Backend configuration key: `FaceRecognitionServiceUrl` (default: `http://127.0.0.1:5000`).
- Called backend-facing endpoints:
  - **POST** `{FaceRecognitionServiceUrl}/verify` — expects multipart/form-data with `img` file, returns verification result(s).
  - **POST** `{FaceRecognitionServiceUrl}/addFace?id={userId}` — multipart/form-data with `img` file to register a face for the given `id`.

## Notes
- The server maps controllers with `app.MapControllers()` in `Program.cs`.
- Models referenced: `Attendance`, `UserData`. See `Models/` for schema details.

---
File generated: `backend/oop_backend/ENDPOINTS.md`
