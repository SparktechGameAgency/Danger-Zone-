using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [Header("Panels managed by this canvas")]
    public GameObject[] panels;

    // Hide all panels
    public void HideAll()
    {
        foreach (var panel in panels)
        {
            if (panel != null)
                panel.SetActive(false);
        }
    }

    // Show only one panel (hide the rest)
    public void ShowOnly(GameObject target)
    {
        HideAll();

        if (target != null)
            target.SetActive(true);
    }

    // Show specific panel (without hiding others)
    public void Show(GameObject target)
    {
        if (target != null)
            target.SetActive(true);
    }

    // Hide specific panel
    public void Hide(GameObject target)
    {
        if (target != null)
            target.SetActive(false);
    }

    // Toggle specific panel
    public void Toggle(GameObject target)
    {
        if (target != null)
            target.SetActive(!target.activeSelf);
    }
}
