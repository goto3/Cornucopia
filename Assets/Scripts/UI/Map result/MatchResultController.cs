using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace MP.Platformer.UI
{
    public class MatchResultController : MonoBehaviour
    {
        private Text winnerName;
        
        void Awake()
        {
            winnerName = GetComponentInChildren<UnityEngine.UI.Text>();
        }
        
        void Start()
        {
            winnerName = GetComponentInChildren<UnityEngine.UI.Text>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetWinnerName(string winner)
        {
            Debug.Log("EL GANADOR EN MATCH RESULT CONTROLLER ES:");
            Debug.Log(winner);
            winnerName.text = winner;
        }

        public void GoBackToMenuClicked()
        {
            PhotonNetwork.Disconnect();
            PhotonNetwork.LoadLevel("Startup");
        }
    }
}
