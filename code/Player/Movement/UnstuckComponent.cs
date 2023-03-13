

using Sandbox;
namespace MyGame;
public class UnstuckComponent : EntityComponent
{
	public bool Debug = false;
	public bool IsActive; // replicate

	internal int StuckTries = 0;
	[Event.Tick.Server]
	public virtual bool TestAndFix()
	{
		var result = TraceBBox( Entity.Position, Entity.Position );

		// Not stuck, we cool
		if ( !result.StartedSolid )
		{
			StuckTries = 0;
			return false;
		}
		if ( Entity is Player player )
		{
			if ( player.MovementController.HasTag( "noclip" ) ) return false;
			//if ( player.DoUnstick != true )
			//{
			//	return false;
			//}
		}
		if ( result.StartedSolid )
		{
			if ( Debug )
			{
				DebugOverlay.Text( $"[stuck in {result.Entity}]", Entity.Position, Color.Red );
				DebugOverlay.Box( result.Entity, Color.Red );
			}
		}

		//
		// Client can't jiggle its way out, needs to wait for
		// server correction to come
		//
		if ( Game.IsClient )
			return true;

		int AttemptsPerTick = 20;

		for ( int i = 0; i < AttemptsPerTick; i++ )
		{
			var pos = Entity.Position + Vector3.Random.Normal * (((float)StuckTries) / 2.0f);

			// First try the up direction for moving platforms
			if ( i == 0 )
			{
				pos = Entity.Position + Vector3.Up * 5;
			}

			result = TraceBBox( pos, pos );

			if ( !result.StartedSolid )
			{
				if ( Debug )
				{
					DebugOverlay.Text( $"unstuck after {StuckTries} tries ({StuckTries * AttemptsPerTick} tests)", Entity.Position, Color.Green, 5.0f );
					DebugOverlay.Line( pos, Entity.Position, Color.Green, 5.0f, false );
				}

				Entity.Position = pos;
				return false;
			}
			else
			{
				if ( Debug )
				{
					DebugOverlay.Line( pos, Entity.Position, Color.Yellow, 0.5f, false );
				}
			}
		}

		StuckTries++;

		return true;
	}
	// Duck body height 32
	// Eye Height 64
	// Duck Eye Height 28


	/// <summary>
	/// Any bbox traces we do will be offset by this amount.
	/// todo: this needs to be predicted
	/// </summary>
	public Vector3 TraceOffset;
	/// <summary>
	/// Traces the current bbox and returns the result.
	/// liftFeet will move the start position up by this amount, while keeping the top of the bbox at the same
	/// position. This is good when tracing down because you won't be tracing through the ceiling above.
	/// </summary>
	public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f )
	{
		return TraceBBox( start, end, (Entity as Player).CollisionBounds.Mins, (Entity as Player).CollisionBounds.Maxs, liftFeet );
	}

	/// <summary>
	/// Traces the bbox and returns the trace result.
	/// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same 
	/// position. This is good when tracing down because you won't be tracing through the ceiling above.
	/// </summary>
	public virtual TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f )
	{
		if ( liftFeet > 0 )
		{
			start += Vector3.Up * liftFeet;
			maxs = maxs.WithZ( maxs.z - liftFeet );
		}

		var tr = Trace.Ray( start + TraceOffset, end + TraceOffset )
					.Size( mins, maxs )
					.WithAnyTags( "solid", "playerclip", "passbullets", "player" )
					.Ignore( Entity )
					.Run();

		tr.EndPosition -= TraceOffset;
		return tr;
	}
}
