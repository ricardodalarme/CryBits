namespace CryBits.Client.Components.Core;

/// <summary>
/// Tag component. The entity carrying this component is followed by the camera.
/// Only one entity should have this tag at a time; if none does, the camera
/// falls back to the local player automatically.
/// </summary>
internal struct CameraTargetTag;
