using Godot;
using System;
using System.Threading;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 5.5f;

	public Vector3 lookLocation = Vector3.Zero;

    [Export]
	SpringArm3D SpringArm;
	[Export]
	float acceleration = 20.0f;

	[Export]
	AnimationPlayer anim;

	CylinderShape3D collider;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Ready()
    {
		collider = (CylinderShape3D)GetNode<CollisionShape3D>("CollisionShape3D").Shape;
    }

    // Next set up the vertical camera rotation.
    public override void _PhysicsProcess(double delta)
	{
		var spaceRid = GetWorld3D().DirectSpaceState;
		var query = new PhysicsRayQueryParameters3D();
		query.From = Position;
		query.To = Position + (Vector3.Down * 3);
		query.CollisionMask = 1;

        var result = spaceRid.IntersectRay(query);

		Vector3 floorLocation = Vector3.Zero;

        Vector3 velocity = Velocity;
		Camera3D cameraRef = GetViewport().GetCamera3D();
		// GD.Print(IsOnFloor());

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y -= gravity * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			if (result.Count > 0)
			{
				floorLocation = (Vector3)result["position"];
			}
			else
			{
				floorLocation = Position + Vector3.Down * ((collider.Height/2.0f) + 0.1f);
			}
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
        //Vector3 direction = (Transform.Basis * cameraRef.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        //Vector3 direction = (Transform.Basis * SpringArm.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        Vector3 direction = (new Vector3(inputDir.X, 0, inputDir.Y).Rotated(Vector3.Up, SpringArm.Rotation.Y)).Normalized();

		if (inputDir.Length() > 0)
		{
			RotationDegrees = new Vector3(RotationDegrees.X, SpringArm.RotationDegrees.Y - (MathF.Atan2(inputDir.Y, inputDir.X) * (180 / Mathf.Pi) + 90), RotationDegrees.Z);
		}
		
        if (direction != Vector3.Zero)
		{
            //velocity.x = direction.X * Speed;
            //velocity.Z = direction.Z * Speed;

            velocity.X = (float)Mathf.MoveToward(velocity.X, direction.X * Speed, delta * acceleration);
			velocity.Z = (float)Mathf.MoveToward(velocity.Z, direction.Z * Speed, delta * acceleration);
        }
		else
		{
			if (!IsOnFloor())
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, (float)delta * acceleration * 0.5f);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, (float)delta * acceleration * 0.5f);
				//velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}
			else
			{
                velocity.X = Mathf.MoveToward(Velocity.X, 0, (float)delta * acceleration * 2.0f);
                velocity.Z = Mathf.MoveToward(Velocity.Z, 0, (float)delta * acceleration * 2.0f);
            }
        }

        if (Position.Y < floorLocation.Y - 0.1f && !IsOnFloor())
        {
			lookLocation = Position;
        }
        else
        {
            lookLocation = new Vector3(Position.X, floorLocation.Y + 0.1f, Position.Z);
        }
        //SpringArm.Position = lookLocation;
        SpringArm.Position = new Vector3(Position.X, floorLocation.Y + 0.1f, Position.Z);

        Velocity = velocity;

		//Beginnings of animation code, will need to be more complex, maybe output a struct with stats or run an outside function
		if (Velocity.Length() > 0)
		{
			anim.Play("Run");
		}
		else
		{
			anim.Play("Idle");
		}
		MoveAndSlide();
	}
}
