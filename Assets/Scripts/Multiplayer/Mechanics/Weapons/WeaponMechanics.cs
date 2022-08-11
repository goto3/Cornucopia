using MP.Platformer.Mechanics;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.Multiplayer.Mechanics.Weapons
{
    public abstract class WeaponMechanics : MonoBehaviourPun, IPunInstantiateMagicCallback
    {
        public bool TwoHanded = false;
        public int ammo;

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            // Attatch to parent (Player)
            object[] instantiationData = info.photonView.InstantiationData;
            var photonViewId = (int)instantiationData[0];

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player.GetComponent<PhotonView>().ViewID == photonViewId)
                {
                    this.transform.SetParent(player.transform);
                    return;
                }
            }
        }

        public abstract void Attack(PlayerController player);

        public abstract Vector3 InstancePosition();

        public abstract void ChargeAttack(PlayerController player, bool isCharging, bool isShooting);
    }
}
