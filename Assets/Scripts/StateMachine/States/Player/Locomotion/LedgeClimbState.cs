using UnityEngine;
using Climbing;

namespace Player.Locomotion
{
    /// <summary>
    /// State untuk climbing ke atas ledge.
    /// </summary>
    public class LedgeClimbState : PlayerBaseState
    {
        private LedgeDetectionHit ledgeHit;
        private float climbDuration = 0.7f; // Durasi animasi climb
        private float climbTimer;
        private bool hasClimbed;

        public LedgeClimbState(PlayerStateMachine stateMachine, LedgeDetectionHit ledgeHit) : base(stateMachine)
        {
            this.ledgeHit = ledgeHit;
        }

        public override void Enter()
        {
            climbTimer = climbDuration;
            hasClimbed = false;
            PlayAnimation("LedgeClimb", 0.05f);
            PlayerStateMachine.Controller.enabled = false;
        }

        public override void UpdateLogic()
        {
            climbTimer -= Time.deltaTime;
            if (!hasClimbed && climbTimer <= climbDuration * 0.5f)
            {
                // Pindahkan pemain ke atas ledge (sedikit di atas grab point)
                Vector3 climbUpPos = ledgeHit.grabPoint.position + ledgeHit.grabPoint.up * 1.0f;
                PlayerStateMachine.transform.position = climbUpPos;
                hasClimbed = true;
            }
            if (climbTimer <= 0f)
            {
                PlayerStateMachine.Controller.enabled = true;
                PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
            }
        }

        public override void Exit()
        {
            PlayerStateMachine.Controller.enabled = true;
        }
    }
} 