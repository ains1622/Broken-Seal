using System.Collections.Generic;
using UnityEngine;

public enum PlayerZone
{
    Northwest,
    Northeast,
    Southwest,
    Southeast
}


public class PlayerAreaIndicator : MonoBehaviour
{
    [Header("Enemy Count by Diagonal Zone")]
    public int northwestCount;
    public int northeastCount;
    public int southwestCount;
    public int southeastCount;
    [Header("Visual")]
    public float radius = 10f;
    public Color areaColor = new Color(1, 0, 0, 0.25f);

    private List<Transform> activeEnemies = new();
    private Dictionary<PlayerZone, int> enemyCounts = new();

    private void Start()
    {
        InitZones();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            UpdateEnemyZoneCounts();
        }
    }

    private void InitZones()
    {
        enemyCounts.Clear();
        foreach (PlayerZone zone in System.Enum.GetValues(typeof(PlayerZone)))
        {
            enemyCounts[zone] = 0;
        }
    }

    private void UpdateEnemyZoneCounts()
    {
        InitZones();

        foreach (Transform enemy in activeEnemies)
        {
            if (enemy == null) continue;

            PlayerZone zone = GetZone(enemy.position);
            enemyCounts[zone]++;
        }

        // Asignar valores a variables públicas
        northwestCount = GetEnemyCountInZone(PlayerZone.Northwest);
        northeastCount = GetEnemyCountInZone(PlayerZone.Northeast);
        southwestCount = GetEnemyCountInZone(PlayerZone.Southwest);
        southeastCount = GetEnemyCountInZone(PlayerZone.Southeast);
    }

    public void RegisterEnemy(Transform enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(Transform enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public PlayerZone GetZone(Vector2 position)
    {
        Vector2 delta = position - (Vector2)transform.position;

        if (delta.x < 0 && delta.y > 0)
            return PlayerZone.Northwest;
        else if (delta.x > 0 && delta.y > 0)
            return PlayerZone.Northeast;
        else if (delta.x < 0 && delta.y < 0)
            return PlayerZone.Southwest;
        else
            return PlayerZone.Southeast;
    }
    public int GetEnemyCountInZone(PlayerZone zone)
    {
        return enemyCounts.TryGetValue(zone, out int count) ? count : 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = areaColor;
        Vector3 center = transform.position;

        // Círculo general
        Gizmos.DrawWireSphere(center, radius);

        // Líneas diagonales
        Vector3 ne = center + (Vector3)(new Vector2(1, 1).normalized * radius);
        Vector3 nw = center + (Vector3)(new Vector2(-1, 1).normalized * radius);
        Vector3 se = center + (Vector3)(new Vector2(1, -1).normalized * radius);
        Vector3 sw = center + (Vector3)(new Vector2(-1, -1).normalized * radius);

        Gizmos.DrawLine(center, ne);
        Gizmos.DrawLine(center, nw);
        Gizmos.DrawLine(center, se);
        Gizmos.DrawLine(center, sw);
    }

    private void OnGUI()
{
    if (!Application.isPlaying) return;

    GUI.Label(new Rect(10, 10, 200, 20), $"Northwest: {northwestCount}");
    GUI.Label(new Rect(10, 30, 200, 20), $"Northeast: {northeastCount}");
    GUI.Label(new Rect(10, 50, 200, 20), $"Southwest: {southwestCount}");
    GUI.Label(new Rect(10, 70, 200, 20), $"Southeast: {southeastCount}");
}
}