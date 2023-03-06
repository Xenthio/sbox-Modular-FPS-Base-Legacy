using Sandbox;

namespace MyGame;
public partial class Gun : Weapon
{
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
		}
	}
	public virtual void ShootBullet( float damage = 0, float spread = 0, float force = 0 )
	{
		Game.SetRandomSeed( Time.Tick );
		var forward = Owner.AimRay.Forward;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
		var tr = Trace.Ray( Owner.AimRay.Position, forward * 65565 ).UseHitboxes().Ignore( Owner ).Run();
		if ( tr.Hit )
		{
			tr.Surface.DoBulletImpact( tr );
			if ( tr.Entity.IsValid() )
			{
				tr.Entity.TakeDamage( DamageInfo.FromBullet( tr.HitPosition, force, damage ) );
			}
		}
	}
}
