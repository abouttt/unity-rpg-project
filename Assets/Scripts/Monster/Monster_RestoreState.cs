using System.Threading;
using UnityEngine;

public class Monster_RestoreState : StateMachineBehaviour
{
    private Monster _monster;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster == null)
        {
            _monster = animator.GetComponent<Monster>();
        }

        _monster.ResetAllTriggers();
        _monster.NaveMeshAgentUpdateToggle(true);
        _monster.NavMeshAgent.SetDestination(_monster.OriginalPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.PlayerDetect())
        {
            _monster.Transition(BasicMonsterState.Tracking);
        }
        else
        {
            if (_monster.IsArrivedCurrentDestination())
            {
                _monster.Transition(BasicMonsterState.Idle);
            }
        }
    }
}
