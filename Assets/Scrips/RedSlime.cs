using System.Collections;
using UnityEngine;

public class RedSlime : EnemyCTRL
{
    protected override void Awake()
    {
        base.Awake();
        MaxHealth = 20f; // 슬라임2는 체력 30
        moveSpeed = 1.5f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override int dropChans()
    {
        return Random.Range(30, 100);
    }
    protected override IEnumerator Dead()
    {
        
        return base.Dead();
    }
}
