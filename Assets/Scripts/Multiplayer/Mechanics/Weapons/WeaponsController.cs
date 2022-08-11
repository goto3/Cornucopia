using UnityEngine;

namespace MP.Platformer.Mechanics
{
    public class WeaponsController : MonoBehaviour
    {
        [Tooltip("Frames per second at which weapons are animated.")]
        public float frameRate = 12;
        [Tooltip("Instances of tokens which are animated. If empty, token instances are found and loaded at runtime.")]
        public KetchupConsumable[] instances;

        void Awake()
        {

        }

        void Update()
        {

        }

    }
}