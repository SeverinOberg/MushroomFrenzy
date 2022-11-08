using UnityEngine;

public class Base : Building 
{
    private float extendedInteractRange = 5;
    private bool isUIActive;

    private void OnEnable()
    {
        SelectionController.OnSelectTarget += OnSelectTargetCallback;
        owner.InputController.OnCancel     += OnCancelCallback;
    }

    private void OnDisable()
    {
        SelectionController.OnSelectTarget -= OnSelectTargetCallback;
        owner.InputController.OnCancel     -= OnCancelCallback;
    }

    private void Update()
    {
        ValidateIsWithinInteractionRange();
    }

    private void ValidateIsWithinInteractionRange()
    {
        if (isUIActive && Vector2.Distance(owner.transform.position, transform.position) > owner.InteractRange + extendedInteractRange)
        {
            SetUI(false);
        }
    }

    private void OnSelectTargetCallback(Building building)
    {
        if (building.gameObject == gameObject && owner.IsWithinMouseInteractRange())
        {
            SetUI(true);
        }
    }

    private void OnCancelCallback()
    {
        SetUI(false);
    }

    private void SetUI(bool value)
    {
        isUIActive = value;
        owner.UIManager.SetArmoryUI(value);
    }

    public override void Die(float destroyDelaySeconds = 3)
    {
        base.Die(destroyDelaySeconds);
        GameManager.Instance.LoseGame();
    }
}
