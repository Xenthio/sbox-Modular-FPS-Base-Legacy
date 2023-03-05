using Sandbox;

namespace MyGame;
public partial class Weapon : Carriable
{
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
	public override string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";
	public virtual float PrimaryAttackDelay => 0.0f;
	public virtual float SecondaryAttackDelay => 0.0f;
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
		if ( Owner is not Player ) return;
		if ( CanReloadPrimary() && Input.Pressed( InputButton.Reload ) )
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
		(Owner as Player).Ammo.TakeAmmo( PrimaryAmmoType, ammo );
		PrimaryAmmo += ammo;
	}
	public TimeSince TimeSinceSecondaryReload;
	public virtual void ReloadSecondary()
	{
		var ammo = (Owner as Player).Ammo.AmmoCount( SecondaryAmmoType ).Clamp( 0, MaxSecondaryAmmo - SecondaryAmmo );
		(Owner as Player).Ammo.TakeAmmo( SecondaryAmmoType, ammo );
		SecondaryAmmo += ammo;
	}
	public virtual bool CanReloadPrimary()
	{
		return PrimaryAmmo != MaxPrimaryAmmo && (Owner as Player).Ammo.AmmoCount( PrimaryAmmoType ) > 0;
	}
	public virtual bool CanReloadSecondary()
	{
		return false;
		//return Input.Pressed( InputButton.sometyhingidk ) && PrimaryAmmo != MaxPrimaryAmmo;
	}
	public TimeSince TimeSincePrimaryAttack;
	public TimeSince TimeSinceSecondaryAttack;
	public virtual void PrimaryAttack()
	{

	}
	public virtual void SecondaryAttack()
	{

	}
	public virtual bool CanPrimaryAttack()
	{
		return Input.Pressed( InputButton.PrimaryAttack ) && TimeSincePrimaryAttack >= PrimaryAttackDelay;
	}
	public virtual bool CanSecondaryAttack()
	{
		return Input.Pressed( InputButton.SecondaryAttack ) && TimeSinceSecondaryAttack >= SecondaryAttackDelay;
	}
}
