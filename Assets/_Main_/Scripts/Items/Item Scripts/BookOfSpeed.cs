using DG.Tweening;
using UnityEngine;

public class BookOfSpeed : Item
{
    private readonly float maxSpeedAllowed = 10;
    private readonly float speed           = 1;

    public override void Purchase(PlayerController player, int quantity)
    {
        player.AddMovementSpeed(speed * quantity);

        player.EffectsAnimator.SetTrigger("level_up_1");
        player.SpriteRenderer.transform.DOComplete();
        player.SpriteRenderer.transform.DOPunchScale(Vector2.one * 0.25f, 1f).SetDelay(0.15f).SetEase(Ease.OutBounce);
        player.Blink(Color.yellow);
    }

    public override bool Validate(PlayerController player, int quantity)
    {
        if (player.MovementSpeed + (speed * quantity) > maxSpeedAllowed)
        {
            UIManager.LogToScreen("Exceeds the max possible speed");
            return false;
        }

        return true;
    }

}
