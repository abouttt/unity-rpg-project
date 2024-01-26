using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    [field: SerializeField]
    public int MonsterID { get; private set; }
    [field: SerializeField]
    public int MaxHP { get; private set; }
    [field: SerializeField]
    public int Damage { get; private set; }
    [field: SerializeField]
    public int Defense { get; private set; }
    [field: SerializeField]
    public int DropMinXP { get; private set; }
    [field: SerializeField]
    public int DropMaxXP { get; private set; }
    [field: SerializeField]
    public int DropMinGold { get; private set; }
    [field: SerializeField]
    public int DropMaxGold { get; private set; }

    public int GetXP() => Random.Range(DropMinXP, DropMaxXP + 1);

    public int GetGold() => Random.Range(DropMinGold, DropMaxGold + 1);
}
