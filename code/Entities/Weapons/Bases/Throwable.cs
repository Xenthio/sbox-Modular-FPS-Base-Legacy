using Sandbox;

namespace MyGame;
public partial class Throwable : Carriable
{
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
				Delete();
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
