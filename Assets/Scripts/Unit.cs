using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
#region Variables/Properties
    // Do not set any of the data in this Scriptable Object, only get.
    public UnitSO unitData;

    public bool  isDead        { get; private set; }
    public float health        { get; private set; }
    public float movementSpeed { get; private set; }

    private SpriteRenderer spriteRenderer;
    private Color          defaultColor;

    public System.Action OnHealthChanged;
    protected System.Action<float> OnMovementSpeedChanged;
#endregion


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    protected virtual void Start()
    {
        health = unitData.health;
        movementSpeed = unitData.movementSpeed;
    }



    #region Health
    public void TakeDamage(float damageAmount)
    {
        Debug.Log("Take Damage: " + damageAmount);
        if (!isDead)
        {
            health -= damageAmount;
            OnHealthChanged?.Invoke();

            if (health < 1)
            {
                Die();
            }
        }
    }

    public void BlinkRed()
    {
        spriteRenderer.color = Color.red;
        Invoke("SetDefaultColor", 0.2f);
    }

    private void SetDefaultColor()
    {
        spriteRenderer.color = defaultColor;
    }
    #endregion

#region MovementSpeed

    public void SetMovementSpeed(float value)
    {

        if (value < 0)
        {
            movementSpeed = 0;
            OnMovementSpeedChanged?.Invoke(movementSpeed);
            return;
        }

        movementSpeed = value;
        OnMovementSpeedChanged?.Invoke(value);
    }

#endregion

#region Die
    public virtual void Die()
    {
        isDead = true;

        if (gameObject.CompareTag("Player") == false)
        {
            StartCoroutine(DeathDelay());
        }

        if (gameObject.CompareTag("Player") || gameObject.CompareTag("Farm"))
        {
            GameManager.Instance.LoseGame();
        }
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
#endregion

}
