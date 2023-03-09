using Godot;
namespace Characters.Player;

public partial class Player : CharacterBody2D
{
	private Controller controller;

	public override void _Ready()
	{
		controller = GetNode<Controller>("Controller");
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = controller.CustomVelocity;
		MoveAndSlide();
	}
}
