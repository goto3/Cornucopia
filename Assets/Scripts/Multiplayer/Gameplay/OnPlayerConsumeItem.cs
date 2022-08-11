using MP.Platformer.Core;
using MP.Platformer.Mechanics;

namespace MP.Platformer.Gameplay
{
    public class OnPlayerConsumeItem : Simulation.Event<OnPlayerConsumeItem>
    {
        public PlayerController player;
        public WeaponConsumable item;

        public override void Execute()
        {
            player.inventory.PickupItem(item);
        }
    }
}