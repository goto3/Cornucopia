using Assets.Scripts.Multiplayer.Mechanics.Weapons;
using MP.Platformer.Mechanics;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public WeaponMechanics EquipedWeapon { get; private set; }
    public Dictionary<string, WeaponConsumable> PickedUpItems { get; private set; } = new Dictionary<string, WeaponConsumable>();
    public Dictionary<string, WeaponMechanics> Inventory { get; private set; } = new Dictionary<string, WeaponMechanics>();

    private PlayerController PlayerController;

    void Start()
    {
        this.PlayerController = GetComponent<PlayerController>();
    }
    void Update() { }

    public void PickupItem(WeaponConsumable item)
    {
        if (!PickedUpItems.ContainsKey(item.name))
            AddItemToInventory(item);
        else
            PickUpAmmo(item);

        var mapPhotonView = GameManager.Instance.CurrentMap.GetComponent<PhotonView>();
        mapPhotonView.RPC("OnItemPickupOrDestroy", RpcTarget.MasterClient);
    }

    private void AddItemToInventory(WeaponConsumable item)
    {
        if (EquipedWeapon != null && EquipedWeapon.gameObject.tag != item.objectToEquip.tag)
            EquipedWeapon.photonView.RPC("SetEnable", RpcTarget.All, false);

        EquipedWeapon = item.Equip(PlayerController).GetComponent<WeaponMechanics>();
        PickedUpItems.Add(item.name, item);
        Inventory.Add(item.objectToEquip.tag, EquipedWeapon);
    }

    private void PickUpAmmo(WeaponConsumable item)
    {
        Inventory[item.objectToEquip.tag].ammo += item.ammoPickupAmount;
        if (EquipedWeapon.gameObject.tag != item.objectToEquip.tag)
        {
            EquipedWeapon.photonView.RPC("SetEnable", RpcTarget.All, false);
            Inventory[item.objectToEquip.tag].photonView.RPC("SetEnable", RpcTarget.All, true);
            EquipedWeapon = Inventory[item.objectToEquip.tag];
        }
    }

    public void Attack(PlayerController player)
    {
        if (EquipedWeapon != null) EquipedWeapon.Attack(player);
    }

    public void ChargeAttack(bool isCharging, bool isShooting)
    {
        if (EquipedWeapon != null) EquipedWeapon.ChargeAttack(PlayerController, isCharging, isShooting);
    }

    public void UnequipWeapons()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            foreach (WeaponConsumable cons in PickedUpItems.Values)
            {
                PhotonNetwork.Destroy(cons.gameObject);
            }
            foreach (WeaponMechanics weapon in Inventory.Values)
            {
                PhotonNetwork.Destroy(weapon.gameObject);
            }

            EquipedWeapon = null;
            PickedUpItems = new Dictionary<string, WeaponConsumable>();
            Inventory = new Dictionary<string, WeaponMechanics>();
        }

    }

}
