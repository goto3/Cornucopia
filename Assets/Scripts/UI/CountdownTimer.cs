using ExitGames.Client.Photon;
using MP.Platformer.Mechanics;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace MP.Platformer.UI
{
    public class CountdownTimer : MonoBehaviourPunCallbacks
    {
        public delegate void CountdownTimerHasExpired();
        public const string CountdownStartTime = "StartTime";

        [Header("Countdown time in seconds")]
        public float Countdown = 5.0f;

        [Header("Reference to a Text component for visualizing the countdown")]
        public Text Text;

        private bool isTimerRunning;
        private int startTime;

        public void Start()
        {
            var textNickNamePlayer1 = GameObject.Find("textNickNamePlayer1").GetComponent<Text>();
            var textNickNamePlayer2 = GameObject.Find("textNickNamePlayer2").GetComponent<Text>();
            var textNickNamePlayer3 = GameObject.Find("textNickNamePlayer3").GetComponent<Text>();
            var textNickNamePlayer4 = GameObject.Find("textNickNamePlayer4").GetComponent<Text>();

            var avatarImagePlayer1 = GameObject.Find("avatarImagePlayer1").GetComponent<Image>();
            var avatarImagePlayer2 = GameObject.Find("avatarImagePlayer2").GetComponent<Image>();
            var avatarImagePlayer3 = GameObject.Find("avatarImagePlayer3").GetComponent<Image>();
            var avatarImagePlayer4 = GameObject.Find("avatarImagePlayer4").GetComponent<Image>();

            var livesDisplayImagePlayer1 = GameObject.Find("livesDisplayImagePlayer1").GetComponent<Image>();
            var livesDisplayImagePlayer2 = GameObject.Find("livesDisplayImagePlayer2").GetComponent<Image>();
            var livesDisplayImagePlayer3 = GameObject.Find("livesDisplayImagePlayer3").GetComponent<Image>();
            var livesDisplayImagePlayer4 = GameObject.Find("livesDisplayImagePlayer4").GetComponent<Image>();
            

            var textplayer1Wins = GameObject.Find("textplayer1Wins").GetComponent<Text>();
            var textplayer2Wins = GameObject.Find("textplayer2Wins").GetComponent<Text>();
            var textplayer3Wins = GameObject.Find("textplayer3Wins").GetComponent<Text>();
            var textplayer4Wins = GameObject.Find("textplayer4Wins").GetComponent<Text>();

            textplayer1Wins.enabled = false;
            textplayer2Wins.enabled = false;
            textplayer3Wins.enabled = false;
            textplayer4Wins.enabled = false;

            livesDisplayImagePlayer1.enabled = false;
            avatarImagePlayer1.enabled = false;
            textNickNamePlayer1.enabled = false;

            livesDisplayImagePlayer2.enabled = false;
            avatarImagePlayer2.enabled = false;
            textNickNamePlayer2.enabled = false;

            livesDisplayImagePlayer3.enabled = false;
            avatarImagePlayer3.enabled = false;
            textNickNamePlayer3.enabled = false;

            livesDisplayImagePlayer4.enabled = false;
            avatarImagePlayer4.enabled = false;
            textNickNamePlayer4.enabled = false;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            Initialize();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public void Update()
        {
            if (!this.isTimerRunning) return;

            float countdown = TimeRemaining();
            int timeLeft = Mathf.RoundToInt(countdown);

            if (timeLeft == 0)
                this.Text.text = "PELEA!";
            else
                this.Text.text = string.Format("{0}", timeLeft.ToString());

            if (countdown > 0.0f) return;

            OnTimerEnds();
        }

        private void OnTimerRuns()
        {
            this.isTimerRunning = true;
            this.enabled = true;
        }

        private void OnTimerEnds()
        {
            this.isTimerRunning = false;
            this.enabled = false;

            this.Text.text = string.Empty;

            Hashtable props = new Hashtable
            {
                {CountdownStartTime, null}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            GetComponent<GameManager>().StartGame();
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(CountdownStartTime)) Initialize();
        }

        private void Initialize()
        {
            int propStartTime;
            if (TryGetStartTime(out propStartTime))
            {
                this.startTime = propStartTime;
                this.isTimerRunning = TimeRemaining() > 0;

                if (this.isTimerRunning)
                    OnTimerRuns();
                else
                    OnTimerEnds();
            }
        }

        private float TimeRemaining()
        {
            int timer = PhotonNetwork.ServerTimestamp - this.startTime;
            return this.Countdown - timer / 1000f;
        }

        public static bool TryGetStartTime(out int startTimestamp)
        {
            startTimestamp = PhotonNetwork.ServerTimestamp;

            object startTimeFromProps;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTime, out startTimeFromProps))
            {
                startTimestamp = (int)startTimeFromProps;
                return true;
            }

            return false;
        }

        public void SetStartTime()
        {
            int startTime = 0;
            bool wasSet = TryGetStartTime(out startTime);

            Hashtable props = new Hashtable
            {
                {CountdownStartTime, (int)PhotonNetwork.ServerTimestamp}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        [PunRPC]
        private void Enable()
        {
            this.enabled = true;
        }

    }
}


