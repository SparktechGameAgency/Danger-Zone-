using UnityEngine;
using System.Collections;

public class PanelAnimator : MonoBehaviour
{
    public float duration = 0.3f;

    public void Show(GameObject panel)
    {
        panel.SetActive(true);
        StartCoroutine(Scale(panel.transform, Vector3.zero, Vector3.one));
    }

    public void Hide(GameObject panel)
    {
        StartCoroutine(HideAndDeactivate(panel));
    }

    IEnumerator HideAndDeactivate(GameObject panel)
    {
        yield return Scale(panel.transform, panel.transform.localScale, Vector3.zero);
        panel.SetActive(false);
    }

    IEnumerator Scale(Transform target, Vector3 from, Vector3 to)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            target.localScale = Vector3.Lerp(from, to, progress);
            yield return null;
        }

        target.localScale = to;
    }
}
