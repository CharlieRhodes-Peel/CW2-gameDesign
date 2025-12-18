using UnityEngine;

public class BossAnimationBridge : MonoBehaviour
{
    [SerializeField] private LogBossStates bossState;

    public void AttackEnded()
    {
        bossState.OnAttackAnimationFinished();
    }
}
