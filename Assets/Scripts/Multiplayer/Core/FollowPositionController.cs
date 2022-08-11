using System.Collections.Generic;
using UnityEngine;

namespace MP.Platformer.Core
{
    public class FollowPositionController : MonoBehaviour
    {

        public List<Transform> players = new List<Transform>();
        public Vector3 offset;
        public float smoothTime = .1f;
        public float minZoom = 40f;
        public float maxZoom = 10f;
        public float zoomLimiter = 50f;

        private Vector3 defaultPos;

        private Vector3 velocity;
        private Camera cam;

        void Start()
        {
            defaultPos = transform.position;
            cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        }

        void LateUpdate()
        {
            if (players.Count == 0)
            {
                transform.position = defaultPos;
            }
            else
            {
                Vector3 centerPoint = GetCenterPoint() + offset;
                transform.position = Vector3.SmoothDamp(transform.position, centerPoint, ref velocity, smoothTime);
            }            
        }

        float GetGreatestDistance()
        {
            var bounds = new Bounds(players[0].position, Vector3.zero);
            foreach (Transform player in players)
            {
                bounds.Encapsulate(player.position);
            }

            return bounds.size.x;
        }

        Vector3 GetCenterPoint()
        {
            if (players.Count == 1) return players[0].position;

            var bounds = new Bounds(players[0].position, Vector3.zero);     
            foreach(Transform player in players)
            {
                bounds.Encapsulate(player.position);
            }

            return bounds.center;
        }
    }
}

