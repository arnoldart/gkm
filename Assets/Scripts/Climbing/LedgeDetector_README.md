# LedgeDetector System Documentation

## Overview

The LedgeDetector is a comprehensive component for detecting climbable ledges in Unity. It integrates seamlessly with the existing Ledge component system and provides accurate detection using multiple casting techniques.

## Features

- **Multi-directional Detection**: Uses capsule casting in multiple directions for comprehensive ledge detection
- **Ledge Component Integration**: Automatically finds and uses grab points from Ledge components
- **Validation System**: Validates ledges based on normal alignment, height, and other criteria
- **Debug Visualization**: Extensive debug drawing and gizmos for development
- **Flexible Configuration**: Highly configurable parameters for different use cases

## Components

### LedgeDetector.cs
The main detection component that performs ledge detection and validation.

### LedgeDetectionHit.cs (Data Structure)
Contains information about a detected ledge including:
- `horizontalHit`: RaycastHit for the vertical surface
- `verticalHit`: RaycastHit for the top surface
- `ledgeComponent`: Reference to the Ledge component (if present)
- `grabPoint`: Closest grab point from the Ledge component
- `distance`: Distance from detection center
- `angleAlignment`: How well aligned the ledge is with the detector's forward direction

### LedgeDetectorExample.cs
Example script demonstrating how to use the LedgeDetector component.

## Setup Instructions

### 1. Basic Setup
1. Add the `LedgeDetector` component to your character or detection object
2. Configure the layer mask to include objects that should be detected as ledges
3. Adjust detection parameters based on your character size and needs

### 2. Parameter Configuration

#### Detection Parameters
- **Ledge Mask**: Layer mask for objects that can be climbed
- **Detection Radius**: Initial sphere overlap radius for quick detection
- **Capsule Radius**: Radius of the detection capsule
- **Capsule Height**: Height of the detection capsule
- **Capsule Cast Iterations**: Number of directions to cast (more = more accurate but slower)
- **Capsule Cast Distance**: How far to cast in each direction

#### Sphere Cast Parameters
- **Sphere Cast Radius**: Radius for top surface detection
- **Sphere Cast Max Height**: Maximum height above detection center to start top detection
- **Sphere Cast Distance**: How far down to cast for top surface

#### Validation
- **Min Normal Dot**: Minimum dot product for valid top surface (0.5 = 60° from vertical)
- **Min Forward Alignment**: Minimum alignment with detector's forward direction
- **Ignore Tags**: Array of tags to ignore during detection

### 3. Integration with Existing Systems

The LedgeDetector is designed to work with the existing Ledge component system:

```csharp
// Example integration
public class ClimbController : MonoBehaviour
{
    [SerializeField] private LedgeDetector ledgeDetector;
    
    private void Update()
    {
        if (ledgeDetector.DetectLedges())
        {
            var bestLedge = ledgeDetector.GetBestLedge();
            
            if (bestLedge.ledgeComponent != null)
            {
                // Use the Ledge component's grab points
                var grabPoint = bestLedge.grabPoint;
                // Position character at grab point
            }
            else
            {
                // Use raw detection data
                var climbPosition = bestLedge.verticalHit.point;
                var climbNormal = bestLedge.horizontalHit.normal;
            }
        }
    }
}
```

## API Reference

### Public Methods

#### `bool DetectLedges()`
Performs the main detection routine. Returns true if any valid ledges were found.

#### `LedgeDetectionHit GetBestLedge()`
Returns the best detected ledge based on alignment and distance.

#### `List<LedgeDetectionHit> GetAllDetectedLedges()`
Returns all detected ledges.

#### `bool HasDetectedLedges()`
Returns true if any ledges are currently detected.

#### `LedgeDetectionHit GetClosestLedge(Vector3 position)`
Returns the ledge closest to the specified position.

### Configuration Properties

All detection parameters are exposed as serialized fields and can be configured in the inspector or via code.

## Debug Features

### Visual Debug Drawing
- Enable `Enable Debug Drawing` to see detection visualization in the scene view
- Capsule casts are drawn in cyan
- Detected ledges are marked with colored spheres
- Grab points are shown in blue with forward direction rays

### Gizmos
- Yellow wireframe sphere shows the initial detection radius
- Cyan wireframe capsule shows the detection volume
- Green spheres mark the best detected ledge
- Red spheres mark other detected ledges
- Blue spheres and rays show grab points and their directions

### Example Script
Use the `LedgeDetectorExample` component to:
- Test detection with keyboard input (E key by default)
- Enable auto-detection with configurable intervals
- Display debug information in the game view
- See detection results in the console

## Performance Considerations

- **Detection Frequency**: Avoid calling `DetectLedges()` every frame. Use intervals or trigger-based detection.
- **Capsule Cast Iterations**: Higher values provide better coverage but impact performance. 8-12 iterations are usually sufficient.
- **Layer Mask Optimization**: Use specific layer masks to reduce the number of objects checked.
- **Detection Radius**: Keep the detection radius as small as practical for your use case.

## Troubleshooting

### No Ledges Detected
1. Check that the layer mask includes the target objects
2. Verify that the detection radius is large enough
3. Ensure ignored tags don't include your ledge objects
4. Check that ledge surfaces have appropriate normals

### Inconsistent Detection
1. Increase capsule cast iterations for better coverage
2. Adjust capsule cast distance if ledges are too far
3. Check validation parameters (normal dots, alignment)
4. Enable debug drawing to visualize detection

### Performance Issues
1. Reduce detection frequency
2. Lower capsule cast iterations
3. Optimize layer masks
4. Reduce detection radius

## Integration Examples

### With Existing Climbing System
```csharp
// In your climbing ability class
if (ledgeDetector.DetectLedges())
{
    var ledge = ledgeDetector.GetBestLedge();
    
    // Set climb target
    SetClimbTarget(ledge.verticalHit.point, ledge.horizontalHit.normal);
    
    // If using Ledge components
    if (ledge.grabPoint != null)
    {
        SetGrabPoint(ledge.grabPoint);
    }
}
```

### With State Machine
```csharp
// In your state machine transition logic
public bool CanClimb()
{
    return ledgeDetector.DetectLedges() && 
           ledgeDetector.GetBestLedge().distance < maxClimbDistance;
}
```

## Advanced Usage

### Custom Validation
You can extend the LedgeDetector by overriding the validation methods or adding custom filters:

```csharp
public class CustomLedgeDetector : LedgeDetector
{
    protected override bool ValidateLedge(RaycastHit horizontalHit, RaycastHit verticalHit)
    {
        // Add custom validation logic
        if (!base.ValidateLedge(horizontalHit, verticalHit))
            return false;
        
        // Example: Check for specific components
        return verticalHit.collider.GetComponent<ClimbableTag>() != null;
    }
}
```

### Multiple Detection Centers
For complex characters, you can use multiple detection centers:

```csharp
public class MultiPointLedgeDetector : MonoBehaviour
{
    [SerializeField] private LedgeDetector[] detectors;
    
    public LedgeDetectionHit GetBestLedgeFromAll()
    {
        var allLedges = new List<LedgeDetectionHit>();
        
        foreach (var detector in detectors)
        {
            if (detector.DetectLedges())
            {
                allLedges.AddRange(detector.GetAllDetectedLedges());
            }
        }
        
        // Find best ledge from all detectors
        return allLedges.OrderByDescending(l => l.angleAlignment).FirstOrDefault();
    }
}
```

This documentation should help you integrate and use the LedgeDetector system effectively in your climbing mechanics.
