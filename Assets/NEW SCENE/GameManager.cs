using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class DangerZoneManager : MonoBehaviour
{
    [Header("Navigation")]
    public ThemeManager themeManager;   // drag the ThemeManager GameObject here

    [Header("Cards")]
    public Button[] cardButtons;
    public Sprite[] faceDownSprites;
    public Sprite safeSprite;
    public Sprite bombSprite;

    [Header("Level System")]
    public LevelData[] levels;
    public Image levelIndicatorImage;

    [Header("In-game Panels")]
    public GameObject successPanel;
    public GameObject failPanel;
    public GameObject congratsPanel;

    [Header("Timer UI")]
    public Text countdownText;

    [Header("Success Panel UI")]
    public Text finishedTimeText;
    public Text levelText;

    [Header("Fail Panel UI")]
    public Text loseFinishedTimeText;
    public Text loseLevelText;

    // ── Private state ─────────────────────────────────────────────────────────
    private int currentLevel = 0;
    private bool cardChosen = false;
    private int clickedIndex = -1;
    private float timeRemaining = 0f;
    private float timeUsed = 0f;
    private List<int> bombIndexes = new List<int>();

    // ── IMPORTANT ─────────────────────────────────────────────────────────────
    // There is NO Start() and NO OnEnable() here.
    // LoadLevel() is the only entry point, called by ThemeManager
    // after it has confirmed this GameObject is active.
    // This eliminates the "Coroutine on inactive GameObject" error entirely.
    // ─────────────────────────────────────────────────────────────────────────

    public void LoadLevel(int levelIndex)
    {
        // Stop any running coroutines from a previous session
        StopAllCoroutines();

        // Immediately hide result panels — no animation, clean slate
        successPanel.SetActive(false);
        failPanel.SetActive(false);

        // Reset state
        cardChosen = false;
        clickedIndex = -1;
        bombIndexes.Clear();

        // Clamp and store
        currentLevel = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
        LevelData lvl = levels[currentLevel];

        // Apply level data
        levelIndicatorImage.sprite = lvl.levelIndicator;
        timeRemaining = lvl.levelTime;
        timeUsed = 0f;
        UpdateTimerUI(timeRemaining);

        // Place bombs randomly
        while (bombIndexes.Count < lvl.bombCount)
        {
            int r = Random.Range(0, cardButtons.Length);
            if (!bombIndexes.Contains(r))
                bombIndexes.Add(r);
        }

        // Reset every card
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int index = i; // capture for lambda
            cardButtons[i].interactable = true;
            cardButtons[i].image.sprite = faceDownSprites[i];
            cardButtons[i].transform.localRotation = Quaternion.identity;
            cardButtons[i].onClick.RemoveAllListeners();
            cardButtons[i].onClick.AddListener(() => OnCardClicked(index));
        }

        // Start the countdown
        StartCoroutine(LevelTimer());
    }

    // ── Timer ─────────────────────────────────────────────────────────────────

    IEnumerator LevelTimer()
    {
        while (timeRemaining > 0f && !cardChosen)
        {
            timeRemaining -= Time.deltaTime;
            timeUsed += Time.deltaTime;
            UpdateTimerUI(timeRemaining);
            yield return null;
        }

        // Time ran out and player never picked a card
        if (!cardChosen)
        {
            ShowLoseInfo();
            ShowPanel(failPanel);
        }
    }

    void UpdateTimerUI(float time)
    {
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        countdownText.text = m.ToString("00") + ":" + s.ToString("00");
    }



    // Haptic feedback helper
    private void Vibrate()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (MultiToggleButton.VibrationEnabled)
        {
            Handheld.Vibrate();
        }
#endif
    }

    // ── Card Interaction ──────────────────────────────────────────────────────

    void OnCardClicked(int index)
    {
        if (cardChosen) return;

        cardChosen = true;
        clickedIndex = index;

        // Lock all cards immediately
        foreach (var btn in cardButtons)
        {
            btn.interactable = false;
            ColorBlock cb = btn.colors;
            cb.disabledColor = Color.white; // keep full opacity when disabled
            btn.colors = cb;
        }

        StartCoroutine(FlipCard(index));
        StartCoroutine(RevealOthersAfterDelay());
    }

    IEnumerator FlipCard(int index)
    {
        RectTransform card = cardButtons[index].transform as RectTransform;
        float duration = 0.25f;
        float elapsed = 0f;

        // Rotate 0 → 90
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            card.localRotation = Quaternion.Euler(0, Mathf.Lerp(0, 90, elapsed / duration), 0);
            yield return null;
        }
        card.localRotation = Quaternion.Euler(0, 90, 0);

        // Swap sprite at the halfway point
        cardButtons[index].image.sprite = bombIndexes.Contains(index) ? bombSprite : safeSprite;

        elapsed = 0f;

        // Rotate 90 → 180
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            card.localRotation = Quaternion.Euler(0, Mathf.Lerp(90, 180, elapsed / duration), 0);
            yield return null;
        }
        card.localRotation = Quaternion.Euler(0, 180, 0);


        //audioManager.Instance.PlayClickSFX();
    }

    IEnumerator RevealOthersAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);

        // Flip all other cards
        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (i != clickedIndex)
                StartCoroutine(FlipCard(i));
        }

        yield return new WaitForSeconds(1f);

        // Evaluate result
        // Evaluate result
        // Evaluate result
        if (bombIndexes.Contains(clickedIndex))
        {
            // Player clicked a bomb → lose
            audioManager.Instance.PlayLoseSFX(); // play bomb/lose sound
            Vibrate(); // vibrate on losing
            ShowLoseInfo();
            ShowPanel(failPanel);
        }
        else
        {
            // Player clicked safe zone → win
            audioManager.Instance.PlayWinSFX(); // play win sound
            Vibrate(); // optional: you can vibrate also for correct pick
            if (currentLevel >= levels.Length - 1)
                ShowCongratulations();
            else
            {
                ShowSuccessInfo();
                ShowPanel(successPanel);
            }
        }
    }

    // ── Result Info ───────────────────────────────────────────────────────────

    void ShowSuccessInfo()
    {
        int m = Mathf.FloorToInt(timeUsed / 60f);
        int s = Mathf.FloorToInt(timeUsed % 60f);
        finishedTimeText.text = "Finished: " + m.ToString("00") + ":" + s.ToString("00");
        levelText.text = "Level: " + (currentLevel + 1);
    }

    void ShowLoseInfo()
    {
        int m = Mathf.FloorToInt(timeUsed / 60f);
        int s = Mathf.FloorToInt(timeUsed % 60f);
        loseFinishedTimeText.text = "Finished: " + m.ToString("00") + ":" + s.ToString("00");
        loseLevelText.text = "Level: " + (currentLevel + 1);
    }

    // ── Panel Helpers ─────────────────────────────────────────────────────────

    void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;
        rt.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutElastic);
    }

    void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    // ── Button Callbacks (wire these in Inspector) ────────────────────────────

    // Retry button
    public void RetryLevel()
    {
        audioManager.Instance.PlayClickSFX();
        LoadLevel(currentLevel);
    }

    // Next Level button
    public void NextLevel()
    {
        audioManager.Instance.PlayClickSFX(); 
        if (currentLevel >= levels.Length - 1)
        {
            ShowCongratulations();
            return;
        }
        currentLevel++;
        LoadLevel(currentLevel);
    }



    // Home button (inside game or congrats panel)
    public void GoHome()
    {
        StopAllCoroutines();
        congratsPanel.SetActive(false);
        successPanel.SetActive(false);
        failPanel.SetActive(false);

        // Tell ThemeManager to go back to menu and deactivate this panel
        if (themeManager != null)
            themeManager.GoToMenu();
    }

    // Restart from beginning (congrats panel)
    public void RestartFromBeginning()
    {
        congratsPanel.SetActive(false);
        currentLevel = 0;
        LoadLevel(currentLevel);
    }

    // ── Congratulations ───────────────────────────────────────────────────────

    void ShowCongratulations()
    {
        successPanel.SetActive(false);
        failPanel.SetActive(false);

        congratsPanel.SetActive(true);
        RectTransform rt = congratsPanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localScale = Vector3.zero;
            rt.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic);
        }
    }


}