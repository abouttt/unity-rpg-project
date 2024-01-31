using UnityEngine;

public class Monster_IdleState : StateMachineBehaviour
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

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_monster.PlayerDetect())
        {
            _monster.Transition(BasicMonsterState.Tracking);
        }
    }
}
