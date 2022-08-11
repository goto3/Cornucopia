using MP.Platformer.Core;
using MP.Platformer.Mechanics;

namespace MP.Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player has died.
    /// </summary>
    /// <typeparam name="PlayerDeath"></typeparam>
    public class PlayerDeath : Simulation.Event<PlayerDeath>
    {
        public PlayerController playerController;

        public override void Execute()
        {
            playerController.controlEnabled = false;

            if (playerController.audioSource && playerController.ouchAudio)
                playerController.audioSource.PlayOneShot(playerController.ouchAudio);

            playerController.animator.SetTrigger("hurt");
            playerController.animator.SetBool("dead", true);

            if (playerController.health.CanRevive)
            {
                var revive = Simulation.Schedule<PlayerSpawn>(2);
                revive.playerController = playerController;
            }

        }
    }
}