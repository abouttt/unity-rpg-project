using UnityEngine;

public class Monster_DamagedState : StateMachineBehaviour
{
    private Monster _monster;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.NaveMeshAgentUpdateToggle(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 1f)
        {
            if (_monster.IsThePlayerInAttackRange())
            {
                _monster.Transition(BasicMonsterState.Attack);
            }
            else
            {
                _monster.Transition(BasicMonsterState.Tracking);
            }
        }
    }
}
