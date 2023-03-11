using Sandbox;

namespace MyGame;

public partial class Player
{
	// Empty overrides for events.
	public override void StartTouch( Entity other )
	{
		base.Touch( other );
		Event.Run( "Player.StartTouch", other );
	}
	public override void Touch( Entity other )
	{
		base.Touch( other );
		Event.Run( "Player.Touch", other );
	}
	public override void EndTouch( Entity other )
	{
		base.Touch( other );
		Event.Run( "Player.EndTouch", other );
	}
}
