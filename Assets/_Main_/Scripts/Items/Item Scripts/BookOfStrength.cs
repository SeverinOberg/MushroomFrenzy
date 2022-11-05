using DG.Tweening;
using UnityEngine;

public class BookOfStrength : Item
{
    private readonly float maxAllowedDamage = 4;
    private readonly float damage           = 1;

    public override void Purchase(PlayerController player, int quantity)
    {
        player.AttackDamage += damage * quantity;

        player.EffectsAnimator.SetTrigger("level_up_1");
        player.SpriteRenderer.transform.DOComplete();
        player.SpriteRenderer.transform.DOPunchScale(Vector2.one * 0.25f, 1f).SetDelay(0.15f).SetEase(Ease.OutBounce);
        player.Blink(Color.red);
    }

    public override bool Validate(PlayerController player, int quantity)
    {
        if (player.AttackDamage + (damage * quantity) > maxAllowedDamage)
        {
            UIManager.LogToScreen("Exceeds the max possible strength");
            return false;
        }

        return true;
    }
}
