using Sandbox;

namespace MyGame;

public partial class Corpse : ModelEntity
{
	[Net] public DamageInfo KillDamage { get; set; }
	[Net] public IClient OwnerClient { get; set; }
}
