using Godot;
using System;
namespace Characters.Player;

public partial class Controller : Node2D
{
	[Export] public float MoveSpeed { get; private set; } = 100f;
	[Export] public Vector2 InputDirection { get; private set; } = new(0, 1);
	public Vector2 CustomVelocity { get; private set; } = Vector2.Zero;
	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback stateMachine;
	private AnimationPlayer animationPlayer;
	private Sprite2D movementSprites;
	private Sprite2D actionSprites;

	public override void _Ready()
	{
		animationTree = GetNode<AnimationTree>("AnimationTree");
		stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		movementSprites = GetNode<Sprite2D>("MovementSprites");
		actionSprites = GetNode<Sprite2D>("ActionSprites");

		UpdateAnimationParameters(InputDirection);
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleInput();
		UpdateAnimationParameters(InputDirection);
		PickNewState();
	}

	public void HandleInput()
	{
		if (Input.IsActionPressed("dig"))
		{
			animationPlayer.Play("dig_down");
			actionSprites.Visible = true;
			movementSprites.Visible = false;
			CustomVelocity = Vector2.Zero;
			return;
		}

		actionSprites.Visible = false;
		movementSprites.Visible = true;
		InputDirection = GetInputDirection();
		CustomVelocity = InputDirection * MoveSpeed;
	}

	private static Vector2 GetInputDirection() => new Vector2(
			Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
			Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
		).Normalized();

	private void UpdateAnimationParameters(Vector2 moveInput)
	{
		if (moveInput == Vector2.Zero) return;

		animationTree.Set("parameters/Idle/blend_position", moveInput);
		animationTree.Set("parameters/Walk/blend_position", moveInput);
	}

	private void PickNewState() => stateMachine.Travel(CustomVelocity == Vector2.Zero ? "Idle" : "Walk");
}
