using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MP.Platformer.Core
{
    public class AddToCamera : MonoBehaviour
    {        

        void Start()
        {
            var followPosition = GameObject.FindGameObjectWithTag("FollowPosition");
            followPosition.GetComponent<FollowPositionController>().players.Add(gameObject.transform);
        }

    }
}
