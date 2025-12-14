# Frontend Services Architecture

This directory contains the abstraction layer for the face recognition frontend application. The architecture is designed to make it easy for developers to modify and extend functionality.

## Architecture Overview

The frontend uses a service-oriented architecture with clear interfaces and implementations. This allows for:

- **Easy swapping** of implementations (e.g., different face detection algorithms)
- **Testability** through dependency injection
- **Separation of concerns** between UI, business logic, and external services
- **Maintainability** through clear interfaces and documentation

## Core Interfaces

### `ICameraService`
Abstraction for camera operations. Handles:
- Starting/stopping the camera
- Capturing frames
- Camera status events

**Implementation**: `CameraService` (uses Emgu.CV VideoCapture)

### `IFaceDetector`
Abstraction for face detection algorithms. Handles:
- Detecting faces in video frames
- Loading and initializing detection models
- Status updates

**Implementation**: `HaarCascadeFaceDetector` (uses Haar Cascade classifier)

**To add a new face detector** (e.g., DNN-based):
1. Create a new class implementing `IFaceDetector`
2. Implement the required methods
3. Use `ServiceFactory.CreateFaceDetector()` or inject directly

### `IImageProcessor`
Abstraction for image processing operations. Handles:
- Converting between Mat and Bitmap formats
- Image cropping with padding
- JPEG encoding

**Implementation**: `ImageProcessor`

### `IAttendanceService`
Abstraction for attendance API operations. Handles:
- Recording attendance (time-in/time-out)
- API communication

**Implementation**: `AttendanceService`

### `IUserDataService`
Abstraction for user data API operations. Handles:
- Fetching user data by ID
- Registering new users

**Implementation**: `UserDataService`

### `IFaceDetectionEventHandler`
Abstraction for handling face detection events. Handles:
- Single face detected
- Multiple faces detected
- No faces detected

**Implementation**: `FaceDetectionEventHandler`

## High-Level Manager

### `FaceScanningManager`
Coordinates all services to provide a complete face scanning workflow:
- Manages camera capture
- Performs face detection
- Handles face processing events
- Processes attendance requests
- Fetches user data after time-in

**Key Features**:
- Request throttling (prevents server overload)
- Automatic user data fetching after time-in
- Event-driven architecture
- Clean separation of concerns

## Service Factory

### `ServiceFactory`
Static factory class for creating service instances. Provides:
- Default configurations
- Easy service creation
- Centralized configuration

**Usage Example**:
```csharp
var httpClient = new HttpClient();
var cameraService = ServiceFactory.CreateCameraService();
var faceDetector = ServiceFactory.CreateFaceDetector();
var attendanceService = ServiceFactory.CreateAttendanceService(httpClient);
```

## How to Modify Functionality

### Changing Face Detection Algorithm

1. Create a new class implementing `IFaceDetector`:
```csharp
public class DnnFaceDetector : IFaceDetector
{
    // Implement interface methods
}
```

2. Update `ServiceFactory.CreateFaceDetector()` or inject directly:
```csharp
var faceDetector = new DnnFaceDetector();
```

### Changing Camera Source

1. Create a new class implementing `ICameraService`:
```csharp
public class NetworkCameraService : ICameraService
{
    // Implement interface methods
}
```

2. Use the new implementation in your form or factory.

### Modifying Face Processing Logic

The `FaceScanningManager` handles face processing. To modify:
- Override event handlers in `IFaceDetectionEventHandler`
- Modify `FaceScanningManager.ProcessSingleFace()` method
- Adjust throttling in `FaceScanningManager.CanSendRequest()`

### Changing API Endpoints

Update the `ServiceFactory` constants or pass custom URLs:
```csharp
var attendanceService = ServiceFactory.CreateAttendanceService(
    httpClient, 
    "https://api.example.com/attendance"
);
```

## Event Flow

1. **Camera Capture**: `ICameraService` captures frame → raises `FrameCaptured` event
2. **Face Detection**: `FaceScanningManager` receives frame → calls `IFaceDetector.DetectFaces()`
3. **Event Handling**: Based on face count, `IFaceDetectionEventHandler` methods are called
4. **Processing**: If single face detected, `FaceScanningManager` processes attendance
5. **API Call**: `IAttendanceService` sends request to backend
6. **User Data**: After time-in, `IUserDataService` fetches user information
7. **UI Update**: Events are raised to update UI components

## Benefits of This Architecture

1. **Easy Testing**: All services can be mocked for unit testing
2. **Flexibility**: Swap implementations without changing UI code
3. **Maintainability**: Clear interfaces make code easier to understand
4. **Extensibility**: Add new features by implementing interfaces
5. **Separation of Concerns**: UI, business logic, and services are separated

## Example: Adding a New Feature

To add face recognition confidence scoring:

1. Extend `IFaceDetector`:
```csharp
public interface IFaceDetector
{
    // ... existing methods ...
    float GetConfidence(Rectangle face, Mat frame);
}
```

2. Update implementations to provide confidence scores
3. Use confidence in `FaceScanningManager` to filter low-confidence detections

This architecture makes it straightforward to extend functionality while maintaining clean, testable code.

