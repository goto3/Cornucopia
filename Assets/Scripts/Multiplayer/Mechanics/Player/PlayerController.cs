using MP.Platformer.Core;
using MP.Platformer.Gameplay;
using MP.Platformer.Model;
using MP.Platformer.UI;
using Photon.Pun;
using System;
using UnityEngine;
using static MP.Platformer.Core.Simulation;
using Random = System.Random;

namespace MP.Platformer.Mechanics
{
    public class PlayerController : KinematicObject, IPunInstantiateMagicCallback
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        public float maxSpeed = 7;
        public float jumpTakeOffSpeed = 7;
        public float groundJumpTakeOffSpeed = 6;
        public float airbornejumpTakeOffSpeed = 4;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        public Collider2D collider2d;
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        private int jumps;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        public PlayerUIManager playerUIManager;

        public Bounds Bounds => collider2d.bounds;

        public PlayerInventory inventory;

        private Vector2 mousePos;
        private Camera cam;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            inventory = GetComponent<PlayerInventory>();
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            playerUIManager = GetComponent<PlayerUIManager>();
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            SetupUI(info.Sender.NickName);
        }

        protected override void Update()
        {
            if (photonView.IsMine)
            {
                if (controlEnabled)
                {
                    move.x = Input.GetAxis("Horizontal");
                    if (Input.GetButtonDown("Jump") && jumps > 0)
                    {
                        jumpState = JumpState.PrepareToJump;
                    }                        
                    else if (Input.GetButtonUp("Jump"))
                    {
                        stopJump = true;
                        Schedule<PlayerStopJump>().player = this;
                    }

                    if (transform.position.y < -3)
                    {
                        photonView.RPC("TakeDamage", RpcTarget.All, health.currentHP);
                    }
                }
                else
                {
                    move.x = 0;
                }

                if (Input.GetMouseButtonDown(0) && health.GetHealth() > 0)
                {
                    inventory.ChargeAttack(true, false);
                }
                if (Input.GetMouseButtonUp(0) && health.GetHealth() > 0)
                {
                    inventory.Attack(this);
                }
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            switch (jumpState)
            {
                case JumpState.Grounded:
                    if (IsGrounded)
                        jumps = 2;
                    else
                    {
                        jumps = 1;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jumps--;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (photonView.IsMine)
            {
                if (jumpState == JumpState.PrepareToJump || jumpState == JumpState.Jumping)
                {
                    if (IsGrounded)
                        velocity.y = groundJumpTakeOffSpeed * model.jumpModifier;
                    else
                        velocity.y = airbornejumpTakeOffSpeed * model.jumpModifier;
                }
                else if (stopJump)
                {
                    stopJump = false;
                    if (velocity.y > 0)
                    {
                        velocity.y = velocity.y * model.jumpDeceleration;
                    }
                }

                targetVelocity = move * maxSpeed;
            }
        }

        protected override void ComputeAnimations()
        {
            if (photonView.IsMine)
            {
                if (inventory.EquipedWeapon != null)
                {
                    mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                    var aimingLeft = mousePos.x < transform.position.x;
                    animator.SetBool("facingLeft", aimingLeft);

                    if (inventory.EquipedWeapon.TwoHanded)
                    {
                        animator.SetLayerWeight(3, 1);
                        if (((Input.GetKey("right") || Input.GetKey("d")) && aimingLeft) || ((Input.GetKey("left") || Input.GetKey("a")) && !aimingLeft))
                            animator.SetLayerWeight(4, 1);
                        else
                            animator.SetLayerWeight(4, 0);
                    }
                    else
                    {
                        animator.SetLayerWeight(1, 1);
                        animator.SetLayerWeight(3, 0);
                        animator.SetLayerWeight(4, 0);
                        if (((Input.GetKey("right") || Input.GetKey("d")) && aimingLeft) || ((Input.GetKey("left") || Input.GetKey("a")) && !aimingLeft))
                            animator.SetLayerWeight(2, 1);
                        else
                            animator.SetLayerWeight(2, 0);
                    }
                }
                else
                {
                    var facingLeft = animator.GetBool("facingLeft");
                    if (move.x < -0.01f)
                        facingLeft = true;
                    else if (move.x > 0.01f)
                        facingLeft = false;
                    animator.SetBool("facingLeft", facingLeft);

                    animator.SetLayerWeight(1, 0);
                    animator.SetLayerWeight(2, 0);
                    animator.SetLayerWeight(3, 0);
                    animator.SetLayerWeight(4, 0);
                }

                animator.SetInteger("jumps", jumps);
                animator.SetBool("grounded", IsGrounded);
                animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);
                animator.SetFloat("velocityY", velocity.y);
            }
            spriteRenderer.flipX = animator.GetBool("facingLeft");
        }

        private void SetupUI(string nickname)
        {
            Random rnd = new Random();
            string finalNickname = String.IsNullOrEmpty(PhotonNetwork.NickName)
                ? rnd.Next(500, 99999).ToString()
                : PhotonNetwork.NickName;
            if (String.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                PhotonNetwork.NickName = finalNickname;
            }
            string chosenCharacter = (string)PhotonNetwork.LocalPlayer.CustomProperties["characterSpriteName"];

            playerUIManager.RunAddPlayer(finalNickname, chosenCharacter);
        }

        public void OnDestroy()
        {
            GameObject.FindGameObjectWithTag("FollowPosition").GetComponent<FollowPositionController>().players.Remove(gameObject.transform);
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }
    }
}