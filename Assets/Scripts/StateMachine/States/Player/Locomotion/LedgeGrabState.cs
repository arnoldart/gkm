using UnityEngine;
using Climbing;
using Player.Locomotion;

namespace Player.Locomotion
{
    /// <summary>
    /// State untuk ketika pemain menggantung di ledge.
    /// </summary>
    public class LedgeGrabState : PlayerBaseState
    {
        private LedgeDetectionHit ledgeHit;
        private float hangTime = 0.2f; // Waktu delay sebelum bisa input
        private float hangTimer;
        private bool canInput;

        public LedgeGrabState(PlayerStateMachine stateMachine, LedgeDetectionHit ledgeHit) : base(stateMachine)
        {
            this.ledgeHit = ledgeHit;
        }

        public override void Enter()
        {
            hangTimer = hangTime;
            canInput = false;
            // Set posisi pemain ke grab point
            if (ledgeHit.grabPoint != null)
            {
                PlayerStateMachine.Controller.enabled = false; // Matikan controller sementara
                PlayerStateMachine.transform.position = ledgeHit.grabPoint.position;
                PlayerStateMachine.transform.rotation = Quaternion.LookRotation(-ledgeHit.horizontalHit.normal, Vector3.up);
            }
            PlayAnimation("LedgeHang", 0.1f);
            PlayerStateMachine.VerticalVelocity = 0f;
        }

        public override void UpdateLogic()
        {
            hangTimer -= Time.deltaTime;
            if (hangTimer <= 0f) canInput = true;

            // Tunggu input setelah delay
            if (!canInput) return;

            // Input naik (misal: W atau Space)
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            {
                // Transisi ke climb state
                PlayerStateMachine.ChangeState(new LedgeClimbState(PlayerStateMachine, ledgeHit));
                return;
            }
            // Input turun/lepas (misal: S atau Ctrl)
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.LeftControl))
            {
                PlayerStateMachine.Controller.enabled = true;
                PlayerStateMachine.ChangeState(new FallingState(PlayerStateMachine));
                return;
            }
            // Input kiri (A/Left)
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                TryMoveOnLedge(-1);
                return;
            }
            // Input kanan (D/Right)
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                TryMoveOnLedge(1);
                return;
            }
        }

        private void TryMoveOnLedge(int direction)
        {
            if (ledgeHit.ledgeComponent == null || ledgeHit.grabPoint == null) return;
            var nextGrab = ledgeHit.ledgeComponent.GetAdjacentGrabPoint(ledgeHit.grabPoint, direction);
            if (nextGrab != null)
            {
                // Pindah di ledge yang sama
                ledgeHit.grabPoint = nextGrab;
                PlayerStateMachine.transform.position = nextGrab.position;
                PlayerStateMachine.transform.rotation = Quaternion.LookRotation(-ledgeHit.horizontalHit.normal, Vector3.up);
                PlayAnimation("LedgeHang", 0.05f);
            }
            else
            {
                // Coba transfer ke ledge lain
                TryTransferToOtherLedge(direction);
            }
        }

        private void TryTransferToOtherLedge(int direction)
        {
            // Arah transfer: gunakan right/left dari grab point
            Vector3 transferDir = (direction == 1) ? ledgeHit.grabPoint.right : -ledgeHit.grabPoint.right;
            Vector3 origin = ledgeHit.grabPoint.position + transferDir * 0.2f;
            float searchRadius = 1f;
            LayerMask ledgeMask = PlayerStateMachine.LedgeDetector.GetLedgeMask();

            Collider[] hits = Physics.OverlapSphere(origin, searchRadius, ledgeMask);
            foreach (var col in hits)
            {
                var newLedge = col.GetComponent<Climbing.Ledge>();
                if (newLedge != null && newLedge != ledgeHit.ledgeComponent)
                {
                    // Cari grab point terdekat di ledge baru
                    var newGrab = newLedge.GetClosestPoint(origin);
                    if (newGrab != null)
                    {
                        ledgeHit.ledgeComponent = newLedge;
                        ledgeHit.grabPoint = newGrab;
                        PlayerStateMachine.transform.position = newGrab.position;
                        PlayerStateMachine.transform.rotation = Quaternion.LookRotation(-col.transform.forward, Vector3.up);
                        PlayAnimation("LedgeHang", 0.05f);
                        break;
                    }
                }
            }
        }

        public override void Exit()
        {
            PlayerStateMachine.Controller.enabled = true;
        }
    }
} 