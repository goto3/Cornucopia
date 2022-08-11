using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace MP.Platformer.Core
{
    public class ObjectPools : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;
        public int spawned = 0;

        public static ObjectPools Instance;

        //SINGLETON PARA INSTANCIAR UNA VEZ
        private void Awake()
        {
            Instance = this;
        }

        //CREO LA CANTIDAD DE OBJETOS CON EL TAG CORRESPONDIENTE Y LAS INSERTO EN LA COLA
        void Start()
        {

        }

        public void RestartPool()
        {
            if (!PhotonNetwork.IsMasterClient) return;

            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = PhotonNetwork.Instantiate(pool.prefab.name, new Vector2(-10,20), Quaternion.identity);
                    obj.GetComponent<PhotonView>().RPC("Enable", RpcTarget.All, false); 
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        //EJECUTO EL METODO PARA INSTANCIAR Y ENCOLAR NUEVAMENTE
        public GameObject SpawnFromPool(string tag, Vector2 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doestn't exist");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.GetComponent<PhotonView>().RPC("Enable", RpcTarget.All, true);

            //MEDIANTE ESTA INTERFAZ LE ASIGNO LAS FISICAS AL OBJETO INSTANCIADO
            IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

            if (pooledObj != null)
            {
                pooledObj.OnObjectSpawn();
            }

            poolDictionary[tag].Enqueue(objectToSpawn);
            return objectToSpawn;
        }

    }
}

