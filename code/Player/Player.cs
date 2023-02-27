using Sandbox;

using System.ComponentModel;
namespace MyGame;

partial class Player : AnimatedEntity
{
	/// <summary>
	/// Called when the entity is first created 
	/// </summary>
	public override void Spawn()
	{
		base.Spawn();

		//
		// Use a watermelon model
		//
		//SetModel( "models/sbox_props/watermelon/watermelon.vmdl" );
		SetModel( "models/citizen/citizen.vmdl" );
		Components.Add( new WalkController() );
		Components.Add( new FirstPersonCamera() );
		Components.Add( new InventoryComponent() );
		Components.Add( new CitizenAnimationComponent() );
		CreateHull();
		Tags.Add( "player" );
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableTouch = true;
	}

	// An example BuildInput method within a player's Pawn class.
	[ClientInput] public Vector3 InputDirection { get; set; }
	[ClientInput] public Angles ViewAngles { get; set; }

	public MovementComponent MovementController => Components.Get<MovementComponent>();
	public CameraComponent CameraController => Components.Get<CameraComponent>();
	public AnimationComponent AnimationController => Components.Get<AnimationComponent>();
	public InventoryComponent Inventory => Components.Get<InventoryComponent>();


	/// <summary>
	/// Position a player should be looking from in world space.
	/// </summary>
	[Browsable( false )]
	public Vector3 EyePosition
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	/// <summary>
	/// Position a player should be looking from in local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Vector3 EyeLocalPosition { get; set; }

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity.
	/// </summary>
	[Browsable( false )]
	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
	/// </summary>
	[Net, Predicted, Browsable( false )]
	public Rotation EyeLocalRotation { get; set; }

	public BBox Hull
	{
		get => new
		(
			new Vector3( -16, -16, 0 ),
			new Vector3( 16, 16, 72 )
		);
	}

	public override Ray AimRay => new Ray( EyePosition, EyeRotation.Forward );
	/// <summary>
	/// Create a physics hull for this player. The hull stops physics objects and players passing through
	/// the player. It's basically a big solid box. It also what hits triggers and stuff.
	/// The player doesn't use this hull for its movement size.
	/// </summary>
	public virtual void CreateHull()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, Hull.Mins, Hull.Maxs );

		//var capsule = new Capsule( new Vector3( 0, 0, 16 ), new Vector3( 0, 0, 72 - 16 ), 32 );
		//var phys = SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, capsule );


		//	phys.GetBody(0).RemoveShadowController();

		// TODO - investigate this? if we don't set movetype then the lerp is too much. Can we control lerp amount?
		// if so we should expose that instead, that would be awesome.
		EnableHitboxes = true;
	}


	//---------------------------------------------// 

	/// <summary>
	/// Pawns get a chance to mess with the input. This is called on the client.
	/// </summary>
	public override void BuildInput()
	{
		base.BuildInput();
		foreach ( var i in Components.GetAll<SimulatedComponent>() )
		{
			if ( i.Enabled ) i.BuildInput();
		}
	}

	/// <summary>
	/// Called every tick, clientside and serverside.
	/// </summary>
	public override void Simulate( IClient cl )
	{

		EyeLocalPosition = Vector3.Up * (64f * Scale);
		base.Simulate( cl );
		if ( Game.IsClient )
		{
			if ( Input.Pressed( InputButton.SlotNext ) )
			{
				Inventory?.SwitchActiveSlot( 1, false );
			}
			if ( Input.Pressed( InputButton.SlotPrev ) )
			{
				Inventory?.SwitchActiveSlot( -1, false );
			}
		}
		foreach ( var i in Components.GetAll<SimulatedComponent>() )
		{
			if ( i.Enabled ) i.Simulate( cl );
		}
	}

	/// <summary>
	/// Called every frame on the client
	/// </summary>
	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		foreach ( var i in Components.GetAll<SimulatedComponent>() )
		{
			if ( i.Enabled ) i.FrameSimulate( cl );
		}
	}
}
