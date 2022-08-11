using System;
using MP.Platformer.Mechanics;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace MP.Platformer.UI
{
    public class MapResultController : MonoBehaviourPunCallbacks
    {
        [Header("Time this screen is shown")]
        public float timeout;

        private Text winnerName;
        private float timePassed;


        void Awake()
        {
            winnerName = GetComponentInChildren<UnityEngine.UI.Text>();
        }

        void Start()
        {
            winnerName = GetComponentInChildren<UnityEngine.UI.Text>();
        }

        public override void OnEnable()
        {
            timePassed = 0;
        }

        void Update()
        {
            if (gameObject.activeInHierarchy)
                timePassed += Time.deltaTime;
            if (timePassed >= timeout)
            {
                CloseScreen();
            }
        }

        private void CloseScreen()
        {
            gameObject.SetActive(false);
            GameManager.Instance.LoadNextMap();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetWinnerName(string winner)
        {
            Debug.Log("EL GANADOR EN MAP RESULT CONTROLLER ES:");
            Debug.Log(winner);
            winnerName.text = winner;
        }
    }
}

