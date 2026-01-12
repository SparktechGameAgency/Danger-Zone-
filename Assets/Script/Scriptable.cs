using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "DangerZone/LevelData")]
public class LevelData : ScriptableObject
{
    public int bombCount;            // how many bombs in this level
    public Sprite levelIndicator;    // UI image showing the level number or icon
}
