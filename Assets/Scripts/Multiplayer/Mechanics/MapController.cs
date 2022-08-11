using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace MP.Platformer.Core
{
    public class MapController : MonoBehaviourPunCallbacks
    {
        private const string PLAYER_DATA_LIVES = "LIVES";

        public List<GameObject> PlayerSpawnPoints;

        [Header("Item spawn")]
        public GameObject ItemToSpawn;
        public GameObject ItemSpawnLocation;
        [Tooltip("in seconds")]
        public float itemSpawnFrequency;
        public int maxNumConsumable;

        private float timeFromLastSpawn = 0;
        private ObjectPools _objectPooler;
        private int currentNumConsumables;

        void Start()
        {
            _objectPooler = GameObject.Find("ObjectPooler").GetComponent<ObjectPools>();
        }

        public override void OnEnable()
        {
            Hashtable restartLives = new Hashtable() { { PLAYER_DATA_LIVES, 3 } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(restartLives);

            if (!PhotonNetwork.IsMasterClient) return;
            ObjectPools.Instance.RestartPool();
            currentNumConsumables = 0;
        }

        void Update()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            timeFromLastSpawn += Time.deltaTime;
            if (timeFromLastSpawn > itemSpawnFrequency && currentNumConsumables < maxNumConsumable)
            {
                timeFromLastSpawn = 0;
                currentNumConsumables++;
                var item = _objectPooler.SpawnFromPool("Box", ItemSpawnLocation.transform.position, Quaternion.identity);
            }            
        }

        [PunRPC]
        void OnItemPickupOrDestroy()
        {
            currentNumConsumables--;
        }

    }
}

