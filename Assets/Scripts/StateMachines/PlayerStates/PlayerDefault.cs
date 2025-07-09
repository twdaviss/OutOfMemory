using System.Collections;

namespace RobotGame.States
{
    public class PlayerDefault : PlayerState
    {
        readonly PlayerController player;
        public PlayerDefault(PlayerController player) { this.player = player; name = "PlayerDefault"; }
        
        public override IEnumerator Start()
        {
            yield break;
        }

        public override IEnumerator Update()
        {
            yield break;
        }
        
        public override IEnumerator End()
        {
            yield break;
        }
        public override IEnumerator FixedUpdate()
        {
            player.InputHandler();
            yield break;
        }
    }
}