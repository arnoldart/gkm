using UnityEngine;

/// <summary>
/// State untuk serangan melee combo (meele1, meele2, meele3).
/// </summary>
public class MeeleState : PlayerBaseState
{
    private int comboStep = 0;
    private float comboTimer = 0f;
    private readonly float comboResetTime = 1.0f; // waktu reset combo dalam detik
    private readonly string[] comboAnimations = { "meele1", "meele2", "meele3" };

    public MeeleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        comboStep = 0;
        PlayComboAnimation();
        comboTimer = 0f;
    }

    public override void UpdateLogic()
    {
        comboTimer += Time.deltaTime;
        if (comboTimer > comboResetTime)
        {
            PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
            return;
        }

        // Cek input serangan dari InputHandler (InputSystem)
        if (PlayerStateMachine.InputHandler != null && PlayerStateMachine.InputHandler.IsAttackPressed())
        {
            if (comboStep < comboAnimations.Length - 1)
            {
                comboStep++;
                PlayComboAnimation();
                comboTimer = 0f;
            }
            PlayerStateMachine.InputHandler.ResetAttackPressed();
        }
    }

    private void PlayComboAnimation()
    {
        PlayAnimation(comboAnimations[comboStep], 0.1f);
    }

    public override void Exit()
    {
        // Reset combo jika keluar dari state
        comboStep = 0;
        comboTimer = 0f;
    }
}
