using DG.Tweening;
using UnityEngine;

public class BookOfHealth : Item
{
    private readonly float health           = 25;
    private readonly float maxAllowedHealth = 150;

    public override void Purchase(PlayerController player, int quantity)
    {
        player.AddMaxHealth(health * quantity);

        player.EffectsAnimator.SetTrigger("level_up_1");
        player.SpriteRenderer.transform.DOComplete();
        player.SpriteRenderer.transform.DOPunchScale(Vector2.one * 0.25f, 1f).SetDelay(0.15f).SetEase(Ease.OutBounce);
        player.Blink(Color.green);
    }

    public override bool Validate(PlayerController player, int quantity)
    {
        if (player.MaxHealth + (health * quantity) > maxAllowedHealth)
        {
            UIManager.LogToScreen("Exceeds the max possible health");
            return false;
        }

        return true;
    }
}
