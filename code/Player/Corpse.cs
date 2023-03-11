using Sandbox;

namespace MyGame;

public partial class Corpse : ModelEntity
{
	public DamageInfo KillDamage { get; set; }
	[Net] public IClient Attacker { get; set; }
	[Net] public IClient Weapon { get; set; }
	[Net] public IClient OwnerClient { get; set; }
}
