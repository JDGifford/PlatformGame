using Godot;
using System;

public partial class PlayerCamera : Camera3D
{
	[Export]
	public Player target;
    [Export]
    public SpringArm3D arm;
	[Export]
	public float distance = 8;

	Vector2 cameraInput = Vector2.Zero;
	Vector3 angle = Vector3.Forward;

	float angleMin = -80, angleMax = 10;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 inputAxis = Input.GetVector("camLeft", "camRight", "camUp", "camDown");

		arm.RotationDegrees = new Vector3(Mathf.Clamp(arm.RotationDegrees.X + inputAxis.Y, angleMin, angleMax), arm.RotationDegrees.Y + inputAxis.X, arm.RotationDegrees.Z);

        LookAt(target.lookLocation);
	}
}
