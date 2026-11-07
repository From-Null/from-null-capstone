# Controls

Controls are implemented using [`UnityEngine.InputSystem.EnhancedTouch`](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.20/api/UnityEngine.InputSystem.EnhancedTouch.Touch.html).
There is an older Unity touch package that we don't use.

**References**:
- [Implement Touch with the new Input System](https://github.com/gamedev-resources/implementing-touch-with-the-new-input-system)

## Rotation

Rotation is implemented by tracking the sliding of a finger or mouse across the screen.

- We have to make sure to rotate around the camera axes, not the model's, so that rotation matches the direction the finger moved.
- Use quaternions to avoid issues with getting stuck on an axis (gimbal lock).

**References**:
- [Rotation and orientation in Unity](https://docs.unity3d.com/2022.3/Documentation/Manual/QuaternionAndEulerRotationsInUnity.html)
- [Simple solution to rotate an object with touch/mouse drag without using OnMouseDrag()](https://discussions.unity.com/t/simple-solution-to-rotate-an-object-with-touch-mouse-drag-without-using-onmousedrag/758554)
- [Transform.Rotate](https://docs.unity3d.com/ScriptReference/Transform.Rotate.html)
