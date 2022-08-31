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

    private int health = 1;
    protected int Health { get => health; set => health = value; }

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

    public void TakeDamage(int damageAmount)
    {
        if (!isDead)
        {
            health -= damageAmount;

            if (health < 1)
            {
                Die();
            }

            Debug.Log("Took " + damageAmount + " damage. Has " + health + " left.");
        }
    }

    public virtual void Die()
    {
        Debug.Log(Title + " has died.");
        isDead = true;
    }

}
