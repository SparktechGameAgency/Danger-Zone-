using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("Panels")]
    public CanvasManager canvasManager;   // Reference to your CanvasManager
    public GameObject loadingPanel;
    public Slider loadingSlider;
    public GameObject menuPanel;         // Main menu panel

    [Header("Settings")]
    public float loadDuration = 3f;      // Simulated load time in seconds

    private void Start()
    {
        // Start the loading process
        canvasManager.HideAll();
        loadingPanel.SetActive(true);
        StartCoroutine(PlayLoading());
    }

    private IEnumerator PlayLoading()
    {
        loadingSlider.value = 0f;
        float elapsed = 0f;

        while (elapsed < loadDuration)
        {
            elapsed += Time.deltaTime;
            loadingSlider.value = Mathf.Clamp01(elapsed / loadDuration);
            yield return null;
        }

        // Ensure slider is full
        loadingSlider.value = 1f;

        // Small delay for smoothness (optional)
        yield return new WaitForSeconds(0.2f);

        // Hide loading panel
        canvasManager.Hide(loadingPanel);

        // Show main menu panel
        canvasManager.ShowOnly(menuPanel);
    }
}
