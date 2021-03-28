using Godot;

public class CameraController : Camera2D
{

  [Export]
  public float _scrollSpeed = 500f;
  [Export]
  public float _scrollWheelRatio = 1.5f;
  [Export]
  public float _trackpadScrollRatio = 1.02f;

  public override void _Process(float delta)
  {
    Vector2 cameraMovement = Vector2.Zero;
    cameraMovement.x += Input.IsActionPressed("camera_right") ? 1 : 0;
    cameraMovement.x += Input.IsActionPressed("camera_left") ? -1 : 0;
    cameraMovement.y += Input.IsActionPressed("camera_up") ? -1 : 0;
    cameraMovement.y += Input.IsActionPressed("camera_down") ? 1 : 0;
    Translate(cameraMovement * _scrollSpeed * delta);

    if (Input.IsActionJustReleased("camera_zoom_in"))
      Zoom /= _scrollWheelRatio;
    if (Input.IsActionJustReleased("camera_zoom_out"))
      Zoom *= _scrollWheelRatio;
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventPanGesture scroll)
    {
      GD.Print(scroll.Delta.y);
      if (scroll.Delta.y < 0)
        Zoom /= _trackpadScrollRatio;
      else if (scroll.Delta.y > 0)
        Zoom *= _trackpadScrollRatio;
    }
  }
}
