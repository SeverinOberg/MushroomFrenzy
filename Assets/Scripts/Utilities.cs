using UnityEngine;

public enum Factions { Player, Enemy, Neutral }

public static class Utilities 
{
    public static Color DragColor = new Color(0, 1, 1, 0.5f);

    public static float GetMinMaxDamageRoll(float minDamage, float maxDamage)
    {
        return Random.Range(minDamage, maxDamage);
    }

    public static Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public static RaycastHit2D GetRaycastOnMousePoint()
    {
        return Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero, 0.0f);
    }

    public static RaycastHit2D[] GetRaycastAllOnMousePoint()
    {
        return Physics2D.RaycastAll(GetMouseWorldPosition(), Vector2.zero, 0.0f);
    }

    public static RaycastHit2D GetRaycastOnMousePoint(string layerMask)
    {
        return Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero, 0.0f, LayerMask.GetMask(layerMask));
    }

    public static void ResetTimer(ref float timer)
    {
        timer = 0;
    }

    // Force reduce velocity to keep Player from gliding
    public static void ForceReduceVelocity(ref Rigidbody2D rb)
    {
        if (rb.velocity.normalized != Vector2.zero)
        {
            rb.velocity = rb.velocity * 0.95f;
        }
    }

    public static float GetDistanceBetween(Vector2 selfPosition, Vector2 Position)
    {
        return Vector2.Distance(selfPosition, Position);
    }

    public static void DestroyAfterDelay(GameObject gameObject, float seconds)
    {
        GameObject.Destroy(gameObject, seconds);
    }
}
