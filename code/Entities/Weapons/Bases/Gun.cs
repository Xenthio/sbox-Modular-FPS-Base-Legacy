using Sandbox;

namespace MyGame;
public partial class Gun : Weapon
{
	public virtual float PrimaryReloadDelay => 0.0f;
	public virtual float SecondaryReloadDelay => 0.0f;
	public virtual int MaxPrimaryAmmo => 0;
	public virtual int MaxSecondaryAmmo => 0;
	public virtual AmmoType PrimaryAmmoType => AmmoType.None;
	public virtual AmmoType SecondaryAmmoType => AmmoType.None;
	[Net] public int PrimaryAmmo { get; set; } = 0;
	[Net] public int SecondaryAmmo { get; set; } = 0;
	bool IsPrimaryReloading => TimeSincePrimaryReload < PrimaryReloadDelay;
	bool IsSecondaryReloading => TimeSinceSecondaryReload < SecondaryReloadDelay;
	public override void Spawn()
	{
		base.Spawn();
		PrimaryAmmo = MaxPrimaryAmmo;
	}
	public override void Simulate( IClient cl )
	{
		if ( CanReloadPrimary() )
		{
			TimeSincePrimaryReload = 0;
			ReloadPrimary();
		}
		if ( CanPrimaryAttack() && !IsPrimaryReloading )
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
				if ( CanReloadPrimary() )
				{
					TimeSincePrimaryReload = 0;
					ReloadPrimary();
				}
			}
		}
		if ( CanSecondaryAttack() && !IsSecondaryReloading )
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
				if ( CanReloadSecondary() )
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
		var ammo = (Owner as Player).Ammo.AmmoCount( PrimaryAmmoType ).Clamp( 0, MaxPrimaryAmmo - PrimaryAmmo );
		PrimaryAmmo += ammo;
	}
	public TimeSince TimeSinceSecondaryReload;
	public virtual void ReloadSecondary()
	{
		var ammo = (Owner as Player).Ammo.AmmoCount( SecondaryAmmoType ).Clamp( 0, MaxSecondaryAmmo - SecondaryAmmo );
		SecondaryAmmo += ammo;
	}
	public virtual bool CanReloadPrimary()
	{
		return Input.Pressed( InputButton.Reload ) && PrimaryAmmo != MaxPrimaryAmmo && (Owner as Player).Ammo.AmmoCount( PrimaryAmmoType ) > 0;
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
