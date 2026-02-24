using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [Header("All panels in the scene")]
    public GameObject[] allPanels;

    /// <summary>Activate one panel, deactivate everything else.</summary>
    public void ShowOnly(GameObject target)
    {
        foreach (var panel in allPanels)
        {
            if (panel != null)
                panel.SetActive(panel == target);
        }
    }

    /// <summary>Turn every panel off.</summary>
    public void HideAll()
    {
        foreach (var panel in allPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }


}
