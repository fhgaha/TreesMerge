using Godot;
namespace Characters.Player;

public partial class Player : CharacterBody2D
{
	[Export] public float MoveSpeed { get; private set; } = 100f;
	[Export] public Vector2 StartingDirection { get; private set; } = new(0, 1);

	private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback stateMachine;

	public override void _Ready()
	{
		animationTree = GetNode<AnimationTree>("AnimationTree");
		stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");

		UpdateAnimationParameters(StartingDirection);
	}

	public override void _PhysicsProcess(double delta)
	{
		var inputDirection = new Vector2(
			Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
			Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
		).Normalized();

		UpdateAnimationParameters(inputDirection);
		Velocity = inputDirection * MoveSpeed;
		MoveAndSlide();
		PickNewState();
	}

	private void UpdateAnimationParameters(Vector2 moveInput)
	{
		if (moveInput == Vector2.Zero) return;

		animationTree.Set("parameters/Idle/blend_position", moveInput);
		animationTree.Set("parameters/Walk/blend_position", moveInput);
	}

	private void PickNewState() => stateMachine.Travel(Velocity == Vector2.Zero ? "Idle" : "Walk");
}
