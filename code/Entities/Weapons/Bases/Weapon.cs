using Sandbox;

namespace MyGame;
public class Weapon : Carriable
{
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
	public override string WorldModelPath => "weapons/rust_pistol/rust_pistol.vmdl";
	public virtual float PrimaryAttackDelay => 0.0f;
	public virtual float SecondaryAttackDelay => 0.0f;
	public override void Spawn()
	{
		base.Spawn();
	}
	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );
		if ( CanPrimaryAttack() )
		{
			TimeSincePrimaryAttack = 0;
			using ( LagCompensation() )
			{
				PrimaryAttack();
			}
		}
		if ( CanSecondaryAttack() )
		{
			TimeSinceSecondaryAttack = 0;
			using ( LagCompensation() )
			{
				SecondaryAttack();
			}
		}
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
