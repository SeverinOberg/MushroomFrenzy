using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Smelter : Building
{

    [SerializeField] private Pickup ironBarPickup;

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float     spawnForce = 1f;

    [SerializeField] private GameObject progressBarFolder;
    [SerializeField] private Image      progressBarImage;

    [SerializeField] private float secondsPerProcess = 5;

    private bool  isUIActive;
    private bool  currentTarget;
    private int   ironBarsProcessing;
    private float processTime;
    private float totalProcessTime;

    private Coroutine doProcess;
    
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

    private void Process()
    {
        // Each Iron Bar costs 2 Iron Ore to make
        int ironBarsToSmelt = (int)(owner.resourceManager.IronOre * 0.5f);

        ironBarsProcessing += ironBarsToSmelt;
        processTime        += ironBarsToSmelt    * secondsPerProcess;
        totalProcessTime    = ironBarsProcessing * secondsPerProcess;
        
        doProcess ??= StartCoroutine(DoProcess());

        owner.uiManager.SetSmelterUIResourceAmount(ironBarsProcessing);
        owner.resourceManager.IronOre -= ironBarsToSmelt * 2;
    }

    private IEnumerator DoProcess()
    {
        progressBarFolder.SetActive(true);

        float secondsPerProcessTime = secondsPerProcess;
        owner.uiManager.SetSmelterUIProgressBarFillAmount(1, 4);
        while (ironBarsProcessing > 0)
        {
            processTime           -= Time.deltaTime;
            secondsPerProcessTime -= Time.deltaTime;

            progressBarImage.fillAmount = processTime / totalProcessTime;
            
            if (secondsPerProcessTime < 0)
            {
                secondsPerProcessTime = secondsPerProcess;
                ironBarsProcessing--;
                Instantiate(ironBarPickup.gameObject, spawnPoint.position, Quaternion.identity);

                owner.uiManager.SetSmelterUIResourceAmount(ironBarsProcessing);

                if (ironBarsProcessing > 0)
                {
                    owner.uiManager.SetSmelterUIProgressBarFillAmount(1, 4);
                }   
            }

            yield return null;
        }
        
        progressBarFolder.SetActive(false);
        doProcess = null;
    }

    private void SetUIActive(bool value)
    {
        if (owner)
        {
            owner.uiManager.SetSmelterUIActive(value);
            isUIActive = value;
        }
    }

    private bool IsOwnerOutsideInteractRange()
    {
        return Utilities.GetDistanceBetween(transform.position, owner.transform.position) > owner.InteractRange;
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

    private void OnCancelCallback()
    {
        SetUIActive(false);
        currentTarget = false;
    }



}
