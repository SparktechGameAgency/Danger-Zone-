using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "DangerZone/LevelData")]
public class LevelData : ScriptableObject
{
    public int bombCount;
    public Sprite levelIndicator;

    [Header("Timer")]
    public float levelTime = 120f; // seconds (02:00)
}
