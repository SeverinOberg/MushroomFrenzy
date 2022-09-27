using UnityEngine;

public enum Factions { Player, Enemy, Neutral }

public static class Utilities 
{
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

    public static RaycastHit2D GetRaycastOnMousePoint(string layerMask)
    {
        return Physics2D.Raycast(GetMouseWorldPosition(), Vector2.zero, 0.0f, LayerMask.GetMask(layerMask));
    }
}
