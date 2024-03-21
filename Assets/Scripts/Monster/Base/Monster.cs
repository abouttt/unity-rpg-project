using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : MonoBehaviour
{
    public event Action HPChanged;

    [field: SerializeField]
    public MonsterData Data { get; private set; }

    [field: Header("플레이어 탐지")]
    [field: SerializeField]
    public float DetectionRadius { get; private set; }

    [field: SerializeField, Range(0, 360)]
    public float DetectionAngle { get; private set; }

    [field: SerializeField]
    public Transform Eyes { get; private set; }

    [field: SerializeField]
    public LayerMask TargetMask { get; private set; }

    [field: SerializeField]
    public LayerMask ObstacleMask { get; private set; }

    [field: Header("플레이어 추적")]
    [field: SerializeField]
    public float TrackingDistance { get; private set; }

    [field: Header("전투")]
    [field: SerializeField]
    public float AttackDistance { get; private set; }

    [field: SerializeField]
    public float RotationSmoothTime { get; private set; }

    [field: SerializeField]
    public float AttackDelayTime { get; private set; }

    [field: SerializeField]
    public Transform AttackOffset { get; private set; }

    [field: SerializeField]
    public float AttackRadius { get; private set; }

    public bool IsLockOnTarget
    {
        get => _isLockOnTarget;
        set
        {
            _isLockOnTarget = value;
            if (_isLockOnTarget)
            {
                ShowHPBar();
            }
        }
    }

    public IReadOnlyList<Collider> LockOnTargetColliders => _lockOnTargetColliders;
    public int CurrentHP { get; set; }
    public int CurrentDamage { get; private set; }
    public Vector3 OriginalPosition { get; private set; }
    public Collider Collider { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent NavMeshAgent { get; private set; }

    protected readonly Collider[] PlayerCollider = new Collider[1];

    private readonly List<Collider> _lockOnTargetColliders = new();
    private readonly Dictionary<MonsterState, int> _stateAnimID = new();

    private UI_MonsterHPBar _hpBar;
    private bool _isLockOnTarget;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy");

        Collider = GetComponent<Collider>();
        Animator = GetComponent<Animator>();
        NavMeshAgent = GetComponent<NavMeshAgent>();

        int lockOnTargetLayer = LayerMask.NameToLayer("LockOnTarget");
        foreach (Transform child in transform)
        {
            if (child.gameObject.layer == lockOnTargetLayer)
            {
                _lockOnTargetColliders.Add(child.GetComponent<Collider>());
            }
        }

        _stateAnimID.Add(MonsterState.Idle, Animator.StringToHash("Idle"));
        _stateAnimID.Add(MonsterState.Tracking, Animator.StringToHash("Tracking"));
        _stateAnimID.Add(MonsterState.Restore, Animator.StringToHash("Restore"));
        _stateAnimID.Add(MonsterState.Attack, Animator.StringToHash("Attack"));
        _stateAnimID.Add(MonsterState.Stunned, Animator.StringToHash("Stunned"));
        _stateAnimID.Add(MonsterState.Damaged, -1);
        _stateAnimID.Add(MonsterState.Dead, -1);
    }

    private void Start()
    {
        Managers.Resource.Instantiate("MinimapIcon.prefab", transform)
            .GetComponent<MinimapIcon>().Setup("MonsterMinimapIcon.sprite", Data.MonsterName);
    }

    private void OnEnable()
    {
        CurrentHP = Data.MaxHP;
        OriginalPosition = transform.position;
        Collider.enabled = true;
        foreach (var collider in _lockOnTargetColliders)
        {
            collider.enabled = true;
        }
    }

    public void Transition(MonsterState state)
    {
        if (_stateAnimID[state] == -1)
        {
            Animator.Play(state.ToString(), -1, 0f);
        }
        else
        {
            Animator.SetTrigger(_stateAnimID[state]);
        }
    }

    public bool TakeDamage(int damage)
    {
        if (CurrentHP <= 0)
        {
            return false;
        }

        ShowHPBar();
        CurrentDamage = Mathf.Clamp(damage - Data.Defense, 0, damage);
        CurrentHP -= CurrentDamage;
        HPChanged?.Invoke();
        CurrentDamage = 0;

        if (CurrentHP <= 0)
        {
            Transition(MonsterState.Dead);
        }
        else
        {
            Transition(MonsterState.Damaged);
        }

        return true;
    }

    public void Stunned()
    {
        if (CurrentHP <= 0)
        {
            return;
        }

        Transition(MonsterState.Stunned);
    }

    public bool PlayerDetect()
    {
        var monsterCenterPos = Collider.bounds.center;
        int cnt = Physics.OverlapSphereNonAlloc(monsterCenterPos, DetectionRadius, PlayerCollider, TargetMask);
        if (cnt != 0)
        {
            var playerCenterPos = PlayerCollider[0].bounds.center;
            var centerDir = (playerCenterPos - monsterCenterPos).normalized;
            if (Vector3.Angle(transform.forward, centerDir) < DetectionAngle * 0.5f)
            {
                var monsterEyesPos = Eyes.position;
                var eyesDir = (playerCenterPos - monsterEyesPos).normalized;
                var EyesToPlayerCenterDist = Vector3.Distance(monsterEyesPos, playerCenterPos);
                if (!Physics.Raycast(monsterEyesPos, eyesDir, EyesToPlayerCenterDist, ObstacleMask))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsThePlayerInAttackRange()
    {
        return Vector3.Distance(Player.Transform.position, transform.position) <= AttackDistance;
    }

    public bool IsArrivedCurrentDestination()
    {
        return !NavMeshAgent.pathPending && NavMeshAgent.remainingDistance <= 0f;
    }

    public void ResetAllTriggers()
    {
        foreach (var param in Animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                Animator.ResetTrigger(param.name);
            }
        }
    }

    public void NaveMeshAgentUpdateToggle(bool toggle)
    {
        NavMeshAgent.isStopped = !toggle;
        NavMeshAgent.updatePosition = toggle;
        NavMeshAgent.updateRotation = toggle;
        if (!toggle)
        {
            NavMeshAgent.velocity = Vector3.zero;
        }
    }

    private void ShowHPBar()
    {
        if (_hpBar == null)
        {
            _hpBar = Managers.Resource.Instantiate("UI_MonsterHPBar.prefab",
                Managers.UI.Get<UI_AutoCanvas>().transform, true).GetComponent<UI_MonsterHPBar>();
            _hpBar.SetTarget(this);
        }
        else
        {
            HPChanged?.Invoke();
        }
    }
}
