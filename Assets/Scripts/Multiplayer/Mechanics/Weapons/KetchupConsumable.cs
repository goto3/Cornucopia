using MP.Platformer.Gameplay;
using Photon.Pun;
using UnityEngine;
using Assets.Scripts.Multiplayer.Mechanics.Weapons;

using static MP.Platformer.Core.Simulation;

namespace MP.Platformer.Mechanics
{
    [RequireComponent(typeof(Collider2D))]
    public class KetchupConsumable : WeaponConsumable
    {
        [Tooltip("If true, animation will start at a random position in the sequence.")]
        public bool randomAnimationStartTime = false;

        [Tooltip("List of frames that make up the animation.")]
        public Sprite[] idleAnimation, collectedAnimation;

        void Awake() { }

        protected override void DisableItem(PlayerController player)
        {
            AudioSource.PlayClipAtPoint(collectAudio, transform.position);

            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("DisableItem", RpcTarget.All);
        }

        protected override void PickupItem(PlayerController player)
        {
            player.GetComponent<Animator>().SetLayerWeight(1, 1);
            var ev = Schedule<OnPlayerConsumeItem>();
            ev.item = this;
            ev.player = player;
        }

        public override GameObject Equip(PlayerController player)
        {
            var data = new object[] { player.GetComponent<PhotonView>().ViewID };
            var playerPosition = player.transform.position;
            var weaponOffset = objectToEquip.GetComponent<WeaponMechanics>().InstancePosition();
            playerPosition += weaponOffset;
            return PhotonNetwork.Instantiate(objectToEquip.name, playerPosition, Quaternion.identity, 0, data);
        }

        public override void Unequip(PlayerController player)
        {
            PhotonNetwork.Destroy(player.inventory.EquipedWeapon.gameObject);
        }

        [PunRPC]
        void DisableItem()
        {
            gameObject.SetActive(false);
        }
    }
}