using System;
using System.Collections;
using UnityEngine;

public class Smelter : Building
{

    [SerializeField] private Pickup ironBarPickup;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnForce = 1f;

    private bool  isUIActive;
    private int   ironBarsProcessing;
    private float secondsPerProcess = 5;
    private bool currentTarget;
    
    protected override void Awake()
    {
        base.Awake();
        type = UnitTypes.Smelter;
        ironBarPickup.spawnForce = spawnForce;
    }

    private void OnEnable()
    {
        if (transform.localScale.x > 0)
        {
            ironBarPickup.spawnDirection = Direction.Right;
        }
        else
        {
            ironBarPickup.spawnDirection = Direction.Left;
        }

        SelectionController.OnSelectTarget      += OnSelectTargetCallback;
        owner.inputController.OnCancel          += OnCancelCallback;
        owner.uiManager.OnSmelterUILoadBtnClick += OnLoadBtnCallback;
    }

    private void OnDisable()
    {
        SetUIActive(false);
        SelectionController.OnSelectTarget      -= OnSelectTargetCallback;
        owner.inputController.OnCancel          -= OnCancelCallback;
        owner.uiManager.OnSmelterUILoadBtnClick -= OnLoadBtnCallback;
    }

    private void Update()
    {
        if (isUIActive && IsOwnerOutsideInteractRange())
        {
            SetUIActive(false);
        }

        if (ironBarsProcessing > 0)
        {
            // process and spawn iron bars every time one is done
        }
    }

    private void OnLoadBtnCallback()
    {
        if (!currentTarget) return; 

        if (owner.resourceManager.IronOre >= 2 && currentTarget)
        {
            Process();
        }
        else
        {
            // @TODO: Make logToScreen non-static and logToScreen here
            // owner.uiManager.LogToScreen("Not enough resources");
            Debug.Log("Not enough resources");
        }
    }

    private Coroutine doProcess;

    private void Process()
    {
        // Each Iron Bar costs 2 Iron Ore to make
        int ironBarsToMake = (int)(owner.resourceManager.IronOre * 0.5f);
        ironBarsProcessing += ironBarsToMake;
        owner.uiManager.SetSmelterUIResourceAmount(ironBarsProcessing);
        owner.resourceManager.IronOre -= ironBarsToMake * 2;

        doProcess ??= StartCoroutine(DoProcess());
    }

    private IEnumerator DoProcess()
    {
        while (ironBarsProcessing > 0)
        {
            yield return new WaitForSeconds(secondsPerProcess);
            Instantiate(ironBarPickup.gameObject, spawnPoint.position, Quaternion.identity);
            ironBarsProcessing--;
            owner.uiManager.SetSmelterUIResourceAmount(ironBarsProcessing);
        }
        doProcess = null;
    }

    private void OnSelectTargetCallback(Unit target)
    {
        if (this == target && owner.IsWithinInteractRange(false))
        {
            owner.uiManager.SetSmelterUIResourceAmount(ironBarsProcessing);
            SetUIActive(true);
            currentTarget = true;
        }
    }

    private void SetUIActive(bool value)
    {
        if (owner)
        {
            owner.uiManager.SetSmelterUIActive(value);
            isUIActive = value;
        }
    }

    private void OnCancelCallback()
    {
        SetUIActive(false);
        currentTarget = false;
    }

    private bool IsOwnerOutsideInteractRange()
    {
        return Utilities.GetDistanceBetween(transform.position, owner.transform.position) > owner.InteractRange;
    }

}
