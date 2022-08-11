using Photon.Pun;
using UnityEngine;

public class ItemSpawnController : MonoBehaviour
{

    private ItemSpawnPoint[] itemSpawns;

    private void Start()
    {
        itemSpawns = UnityEngine.Object.FindObjectsOfType<ItemSpawnPoint>();

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (ItemSpawnPoint spawnPoint in itemSpawns)
            {
                var prefabName = spawnPoint.item.name;
                var position = spawnPoint.transform.position;
                PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
            }
        }
    }
}
