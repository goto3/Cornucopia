using MP.Platformer.Core;
using MP.Platformer.Mechanics;

namespace MP.Platformer.Gameplay
{
    /// <summary>
    /// Fired when the player is spawned after dying.
    /// </summary>
    public class PlayerSpawn : Simulation.Event<PlayerSpawn>
    {
        public PlayerController playerController;

        public override void Execute()
        {
            playerController.collider2d.enabled = true;
            playerController.controlEnabled = false;

            if (playerController.audioSource && playerController.respawnAudio)
                playerController.audioSource.PlayOneShot(playerController.respawnAudio);

            playerController.health.RefillHealth();

            var spawnPoint = GameManager.Instance.GetRandomSpawnPoint();

            playerController.Teleport(spawnPoint.transform.position);
            playerController.jumpState = PlayerController.JumpState.Grounded;
            playerController.animator.SetBool("dead", false);

            var playerInput = Simulation.Schedule<EnablePlayerInput>(1f);
            playerInput.playerController = playerController;
        }
    }
}