using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuringMushroom : Building
{
    [SerializeField] private Transform lurePoint;
    [SerializeField] private float     scanRadius   = 20;
    [SerializeField] private float     scanCooldown = 5;

    private LayerMask scanLayerMask;
    private float timeSinceLastScan;

    protected override void Start()
    {
        base.Start();

        scanLayerMask = LayerMask.GetMask("Spirit Essence");
    }

    private void Update()
    {
        timeSinceLastScan += Time.deltaTime;
        if (timeSinceLastScan > scanCooldown)
        {
            timeSinceLastScan = 0;
            Lure();
        }
    }

    private void Lure()
    {
        Collider2D[] result = Physics2D.OverlapCircleAll(transform.position, scanRadius, scanLayerMask);
        if (result.Length > 0)
        {
            for (int i = 0; i < result.Length; i++)
            {
                if( result[i].TryGetComponent(out Lureable lureable))
                {
                    lureable.Initialize(lurePoint);
                }
            }
        }
    }

}
