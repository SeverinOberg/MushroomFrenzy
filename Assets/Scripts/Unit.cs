using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour 
{

    public bool isDead { get; private set; }

    private string title = "Unit";
    protected string Title
    {
        get { return title; }
        set
        {
            if (value == "")
            {
                Debug.LogError("Title value can not be empty");
                return;
            }
            else
            {
                title = value;
            }
        }
    }

    private float health = 1;
    protected float Health { get => health; set => health = value; }

    private int faction = 0;
    public int Faction { 
        get { return faction; } 
        set
        {
            if (value < 0)
            {
                Debug.LogError("Faction can not be less than 0");
                return;
            }
            else
            {
                faction = value;
            }
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (!isDead)
        {
            health -= damageAmount;

            if (health < 1)
            {
                Die();
            }
            //Debug.Log(title + " took " + damageAmount + " damage. Has " + health + " left.");
        }
    }

    public virtual void Die()
    {
        Debug.Log(title + " has died.");
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

}
