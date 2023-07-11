using Sandbox;

namespace MyGame;
public partial class Gun : Weapon
{
	internal float viewpunchmod = 0;
	bool punched = false;
	public virtual void ShootEffects()
	{
		Game.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimParameter( "fire", true );

	}

	public virtual void DoViewPunch( float punch )
	{
		if ( Game.IsClient )
		{
			(Owner as Player).ViewAngles += new Angles( -punch, 0, 0 );
			viewpunchmod = 0.5f;
			punched = false;
		}
	}
	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );
		ViewPunchEffectFrame();
	}
	public virtual void ViewPunchEffectFrame()
	{
		if ( Owner == null || Owner.LifeState != LifeState.Alive ) return;
		if ( viewpunchmod <= -0.1f )
		{
			punched = true;

		}
		if ( punched )
		{
			viewpunchmod = viewpunchmod.LerpTo( 0, Time.Delta * 28 );
		}
		else
		{
			viewpunchmod = viewpunchmod.LerpTo( -0.12f, Time.Delta * 48 );
		}
		(Owner as Player).ViewAngles += new Angles( -viewpunchmod, 0, 0 );
	}
	public virtual void ShootBullet( float damage = 0, float spread = 0, float force = 0, int count = 1 )
	{
		//ShootBullet( null, null, null, damage, spread, force, count, 0 );
		var bullet = new BulletInfo()
		{
			Owner = Owner,
			Weapon = this,
			Position = Owner.AimRay.Position,
			Direction = Owner.AimRay.Forward,
			Damage = damage,
			Spread = spread,
			Force = force,
			Count = count, 
			TracerPosition = (EffectEntity.GetAttachment( "muzzle" ) ?? Transform).Position
		};
		Bullet.ShootBullet( bullet );
	} 
}
