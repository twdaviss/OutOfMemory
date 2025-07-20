using System.Collections;
using UnityEngine;
namespace RobotGame.States
{
    public class PlayerDashing : PlayerState
    {
        readonly PlayerController player;
        readonly float dashSpeed;
        readonly float dashTime;
        private float dashTimer;

        public PlayerDashing(PlayerController player, float dashSpeed, float dashTime) { this.player = player; this.dashSpeed = dashSpeed; this.dashTime = dashTime; this.name = "PlayerDashing"; }
        
        public override IEnumerator Start()
        {
            dashTimer = dashTime;
            yield break;
        }

        public override IEnumerator Update()
        {
            if(dashTimer <= 0)
            {
                player.TransitionState(new PlayerDefault(player));
                yield break;
            }
            player.GetComponent<Rigidbody2D>().MovePosition(player.transform.position += (Vector3)player.moveDirection * dashSpeed * Time.deltaTime);
            dashTimer -= Time.deltaTime;
            yield break;
        }

        public override IEnumerator FixedUpdate()
        {
            yield break;
        }

        public override IEnumerator End()
        {
            yield break;
        }
    }
}