using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("References")]
    public CanvasManager canvasManager;
    public GameObject loadingPanel;
    public GameObject menuPanel;
    public Slider loadingSlider;

    [Header("Settings")]
    public float loadDuration = 3f;

    private void Start()
    {
        StartCoroutine(PlayLoading());
    }

    private IEnumerator PlayLoading()
    {
        // 1️⃣ Turn everything off
        canvasManager.HideAll();

        // 2️⃣ Show loading panel only
        canvasManager.ShowOnly(loadingPanel);

        loadingSlider.value = 0f;
        float elapsed = 0f;

        // 3️⃣ Fake loading progress
        while (elapsed < loadDuration)
        {
            elapsed += Time.deltaTime;
            loadingSlider.value = Mathf.Clamp01(elapsed / loadDuration);
            yield return null;
        }

        loadingSlider.value = 1f;

        yield return new WaitForSeconds(0.3f);

        // 4️⃣ Show menu
        canvasManager.ShowOnly(menuPanel);
        loadingPanel.SetActive(false);
    }
}