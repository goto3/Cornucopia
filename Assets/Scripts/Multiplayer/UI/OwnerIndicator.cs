using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MP.Platformer.UI
{
    public class OwnerIndicator : MonoBehaviour
    {

        public GameObject Owner;

        void Start()
        {
            if (!Owner.GetComponent<PhotonView>().IsMine)
            {
                gameObject.SetActive(false);
            }
        }
    }
}

