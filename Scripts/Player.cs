using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 130.0f;

	public override void _PhysicsProcess(double delta)
	{
		float directionX = Input.GetAxis("move_left", "move_right");
		float directionY = Input.GetAxis("move_up", "move_down");
		
		if (directionX != 0f)
		{
			Velocity = new Vector2(directionX * Speed, Velocity.Y);
		}

		else if (directionY != 0f)
		{
			Velocity = new Vector2( Velocity.X, directionY * Speed);
		}

		else
		{
			Velocity = Vector2.Zero;
		}
		MoveAndSlide();
	}

}
