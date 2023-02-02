using Sandbox;

namespace MyGame;

public class FirstPersonCamera : SimulatedComponent
{
	public override void FrameSimulate( IClient cl )
	{

		var pl = Entity as Player;
		// Update rotation every frame, to keep things smooth
		Entity.Rotation = pl.ViewAngles.ToRotation();

		Camera.Position = Entity.Position + new Vector3( 0, 0, 64 );
		Camera.Rotation = Entity.Rotation;

		// Set field of view to whatever the user chose in options
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );

		// Set the first person viewer to this, so it won't render our model
		Camera.FirstPersonViewer = Entity;
	}
	public override void BuildInput()
	{
		var pl = Entity as Player;
		var look = Input.AnalogLook;
		if ( pl.ViewAngles.pitch > 90f || pl.ViewAngles.pitch < -90f )
		{
			look = look.WithYaw( look.yaw * -1f );
		}
		var viewAngles = pl.ViewAngles;
		viewAngles += look;
		viewAngles.pitch = viewAngles.pitch.Clamp( -89f, 89f );
		pl.ViewAngles = viewAngles.Normal;
	}
}
