using MP.Platformer.Mechanics;
using Photon.Pun;
using UnityEngine;

public abstract class WeaponConsumable : MonoBehaviourPun
{
    public GameObject objectToEquip;

    public AudioClip collectAudio;

    public int ammoPickupAmount = 5;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player.photonView.IsMine)
            {                
                DisableItem(player);
                PickupItem(player);
            }
        }
    }

    protected abstract void DisableItem(PlayerController player);

    protected abstract void PickupItem(PlayerController player);

    public abstract GameObject Equip(PlayerController player);

    public abstract void Unequip(PlayerController player);

}
