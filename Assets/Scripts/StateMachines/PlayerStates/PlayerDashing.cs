using System.Collections;
using UnityEngine;
namespace RobotGame.States
{
    public class PlayerDashing : PlayerState
    {
        readonly PlayerController player;
        readonly float dashSpeed;
        readonly float dashTime;
        private float dashTimer = 0;

        private float currentDashSpeed;

        public PlayerDashing(PlayerController player, float dashSpeed, float dashTime) { this.player = player; this.dashSpeed = dashSpeed; this.dashTime = dashTime; this.name = "PlayerDashing"; }
        
        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            if(dashTimer >= dashTime)
            {
                player.TransitionState(new PlayerDefault(player));
                yield break;
            }
            float x = dashTimer / dashTime;
            currentDashSpeed = (1 - Mathf.Pow(1 - x, 4)) * dashSpeed;
            player.GetComponent<Rigidbody2D>().MovePosition(player.transform.position += (Vector3)player.moveDirection * currentDashSpeed * Time.deltaTime);
            dashTimer += Time.deltaTime;
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