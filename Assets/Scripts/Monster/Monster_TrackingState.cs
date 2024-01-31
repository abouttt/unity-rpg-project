using UnityEngine;

public class Monster_TrackingState : StateMachineBehaviour
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
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Vector3.Distance(Player.GameObject.transform.position, _monster.transform.position) > _monster.TrackingDistance)
        {
            _monster.Transition(BasicMonsterState.Restore);
        }
        else if (_monster.IsThePlayerInAttackRange())
        {

        }
        else
        {
            _monster.NavMeshAgent.SetDestination(Player.GameObject.transform.position);
        }
    }
}
