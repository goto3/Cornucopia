using MP.Platformer.Core;
using MP.Platformer.Mechanics;

namespace MP.Platformer.Gameplay
{
    /// <summary>
    /// This event is fired when user input should be enabled.
    /// </summary>
    public class EnablePlayerInput : Simulation.Event<EnablePlayerInput>
    {
        public PlayerController playerController;

        public override void Execute()
        {
            playerController.controlEnabled = true;
        }
    }
}