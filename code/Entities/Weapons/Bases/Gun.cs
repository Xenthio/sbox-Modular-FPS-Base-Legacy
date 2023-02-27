using Sandbox;

namespace MyGame;
public class Gun : Weapon
{
	public virtual float PrimaryReloadDelay => 0.0f;
	public virtual float SecondaryReloadDelay => 0.0f;
	public virtual int MaxPrimaryAmmo => 0;
	public virtual int MaxSecondaryAmmo => 0;
	public int PrimaryAmmo { get; set; } = 0;
	public int SecondaryAmmo { get; set; } = 0;
	bool IsPrimaryReloading => TimeSincePrimaryReload < PrimaryReloadDelay;
	bool IsSecondaryReloading => TimeSinceSecondaryReload < SecondaryReloadDelay;
	public override void Spawn()
	{
		base.Spawn();
	}
	public override void Simulate( IClient cl )
	{
		if ( CanReloadPrimary() )
		{
			TimeSincePrimaryReload = 0;
			ReloadPrimary();
		}
		if ( !IsPrimaryReloading )
		{
			if ( CanPrimaryAttack() )
			{
				TimeSincePrimaryAttack = 0;
				if ( PrimaryAmmo > 0 )
				{
					using ( LagCompensation() )
					{
						PrimaryAttack();
					}
				}
				else
				{
					TimeSincePrimaryReload = 0;
					ReloadPrimary();
				}
			}
		}
		if ( !IsSecondaryReloading )
		{
			if ( CanSecondaryAttack() )
			{
				TimeSinceSecondaryAttack = 0;
				if ( SecondaryAmmo > 0 )
				{
					using ( LagCompensation() )
					{
						SecondaryAttack();
					}
				}
				else
				{
					TimeSinceSecondaryReload = 0;
					ReloadSecondary();
				}
			}
		}
	}
	public TimeSince TimeSincePrimaryReload;
	public virtual void ReloadPrimary()
	{
		PrimaryAmmo = MaxPrimaryAmmo;
	}
	public TimeSince TimeSinceSecondaryReload;
	public virtual void ReloadSecondary()
	{
		PrimaryAmmo = MaxSecondaryAmmo;
	}
	public virtual bool CanReloadPrimary()
	{
		return Input.Pressed( InputButton.Reload ) && PrimaryAmmo != MaxPrimaryAmmo;
	}
	public virtual bool CanReloadSecondary()
	{
		return false;
		//return Input.Pressed( InputButton.sometyhingidk ) && PrimaryAmmo != MaxPrimaryAmmo;
	}
	public virtual void ShootBullet( float damage = 0, float spread = 0, float force = 0 )
	{
		Game.SetRandomSeed( Time.Tick );
		var spreadvec = Vector3.Random * spread;
		var tr = Trace.Ray( Owner.AimRay.Position, (spreadvec + Owner.AimRay.Forward) * 65565 ).UseHitboxes().Ignore( Owner ).Run();
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
