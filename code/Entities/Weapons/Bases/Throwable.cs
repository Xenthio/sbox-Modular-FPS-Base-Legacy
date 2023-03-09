using Sandbox;

namespace MyGame;
public partial class Throwable : Carriable
{
	[ConVar.Replicated] public static bool sv_infinite_grenade { get; set; } = false;
	public TimeSince TimeSinceClicked;
	public override void Simulate( IClient cl )
	{
		if ( Owner is not Player ) return;
		if ( PullPin() )
		{
			TimeSinceClicked = 0;
		}
		if ( WillThrow() )
		{
			using ( LagCompensation() )
			{
				Throw();
			}
			if ( Game.IsServer )
			{
				if ( Owner is Player ply )
				{
					ply.Inventory?.Items.Remove( this );
				}
				if ( !sv_infinite_grenade ) Delete();
			}
		}
	}
	public virtual bool PullPin()
	{
		return Input.Pressed( InputButton.PrimaryAttack );
	}
	public virtual bool WillThrow()
	{
		return Input.Released( InputButton.PrimaryAttack );
	}
	public virtual void Throw()
	{

	}
}
