using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;


public class DangerZoneManager : MonoBehaviour


{

    [Header("Final Completion Panel")]
    public GameObject congratsPanel;
    public CanvasManager canvasManager;
    public GameObject menuPanel;


    [Header("Cards")]
    public Button[] cardButtons;
    public Sprite[] faceDownSprites;
    public Sprite safeSprite;
    public Sprite bombSprite;

    [Header("Level System")]
    public LevelData[] levels;
    public Image levelIndicatorImage;

    [Header("Panels")]
    public GameObject successPanel;
    public GameObject failPanel;

    [Header("Timer UI")]
    public Text countdownText;

    [Header("Result UI (Success Panel)")]
    public Text finishedTimeText;
    public Text levelText;

    private int currentLevel = 0;
    private bool cardChosen = false;

    private List<int> bombIndexes = new List<int>();
    private int clickedIndex = -1;

    private float timeRemaining;
    private float timeUsed;

    audioManager audioManager;


    [Header("Result UI (Fail Panel)")]
    public Text loseFinishedTimeText;
    public Text loseLevelText;


    void Start()
    {
        LoadLevel(currentLevel);
        
    }

    public void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<audioManager>();

    }


    public void LoadLevel(int levelIndex)
    {
        HidePanel(successPanel);
        HidePanel(failPanel);

        StopAllCoroutines();

        cardChosen = false;
        clickedIndex = -1;
        bombIndexes.Clear();

        levelIndex = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
        LevelData level = levels[levelIndex];

        levelIndicatorImage.sprite = level.levelIndicator;

        timeRemaining = level.levelTime;
        timeUsed = 0f;

        StartCoroutine(LevelTimer());

        while (bombIndexes.Count < level.bombCount)
        {
            int r = Random.Range(0, cardButtons.Length);
            if (!bombIndexes.Contains(r))
                bombIndexes.Add(r);
        }

        for (int i = 0; i < cardButtons.Length; i++)
        {
            int index = i;

            cardButtons[i].image.sprite = faceDownSprites[i];
            cardButtons[i].transform.localRotation = Quaternion.identity;

            cardButtons[i].onClick.RemoveAllListeners();
            cardButtons[i].onClick.AddListener(() => OnCardClicked(index));
        }
    }

    IEnumerator LevelTimer()
    {
        while (timeRemaining > 0 && !cardChosen)
        {
            timeRemaining -= Time.deltaTime;
            timeUsed += Time.deltaTime;
            UpdateTimerUI(timeRemaining);
            yield return null;
        }

        if (!cardChosen)
        {
            ShowLoseInfo();
            ShowPanel(failPanel);
        }

    }

    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        countdownText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    void ShowLoseInfo()
    {
        int minutes = Mathf.FloorToInt(timeUsed / 60);
        int seconds = Mathf.FloorToInt(timeUsed % 60);

        loseFinishedTimeText.text = "Finished: " +
            minutes.ToString("00") + ":" + seconds.ToString("00");

        loseLevelText.text = "Level: " + (currentLevel + 1);
    }


    void OnCardClicked(int index)
    {
        if (cardChosen) return;

        cardChosen = true;
        clickedIndex = index;

        StartCoroutine(FlipCard(index));
        StartCoroutine(RevealOthersAfterDelay());
    }

    IEnumerator RevealOthersAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (i == clickedIndex) continue;
            StartCoroutine(FlipCard(i));
        }

        yield return new WaitForSeconds(1f);

        if (bombIndexes.Contains(clickedIndex))
        {
            ShowLoseInfo();
            ShowPanel(failPanel);
        }
        else
        {
            // If last level → skip win panel and show congratulations
            if (currentLevel >= levels.Length - 1)
            {
                ShowCongratulations();
            }
            else
            {
                ShowSuccessInfo();
                ShowPanel(successPanel);
            }
        }

    }

    IEnumerator FlipCard(int index)
    {
        RectTransform card = cardButtons[index].transform as RectTransform;

        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float yRot = Mathf.Lerp(0, 90, elapsed / duration);
            card.localRotation = Quaternion.Euler(0, yRot, 0);
            yield return null;
        }

        card.localRotation = Quaternion.Euler(0, 90, 0);

        cardButtons[index].image.sprite =
            bombIndexes.Contains(index) ? bombSprite : safeSprite;

        elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float yRot = Mathf.Lerp(90, 180, elapsed / duration);
            card.localRotation = Quaternion.Euler(0, yRot, 0);
            yield return null;
        }

        card.localRotation = Quaternion.Euler(0, 180, 0);
    }

    void ShowSuccessInfo()
    {
        int minutes = Mathf.FloorToInt(timeUsed / 60);
        int seconds = Mathf.FloorToInt(timeUsed % 60);

        finishedTimeText.text = "Finised: " +
            minutes.ToString("00") + ":" + seconds.ToString("00");

        levelText.text = "Level: " + (currentLevel + 1);
    }

    public void NextLevel()
    {
        if (currentLevel >= levels.Length - 1)
        {
            ShowCongratulations();
            return;
        }

        currentLevel++;
        LoadLevel(currentLevel);
    }


    void ShowCongratulations()
    {
        // Hide win & lose panels
        HidePanel(successPanel);
        HidePanel(failPanel);

        // Show congratulations panel
        congratsPanel.SetActive(true);

        RectTransform rt = congratsPanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localScale = Vector3.zero;
            rt.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic);
        }
    }



    public void RetryLevel()
    {
        LoadLevel(currentLevel);
    }

    void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.localScale = Vector3.zero;
        rt.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic);
    }

    void HidePanel(GameObject panel)
    {
        if (!panel.activeSelf) return;

        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.DOScale(Vector3.zero, 0.3f)
          .SetEase(Ease.InBack)
          .OnComplete(() => panel.SetActive(false));
    }

    private void OnEnable()
    {
        RestartGame();
    }

    public void RestartGame()
    {
        currentLevel = 0;
        LoadLevel(currentLevel);
    }


    public void RestartFromBeginning()
    {
        congratsPanel.SetActive(false);
        currentLevel = 0;
        LoadLevel(currentLevel);
    }

    public void GoHome()
    {
        congratsPanel.SetActive(false);

        if (canvasManager != null && menuPanel != null)
        {
            canvasManager.ShowOnly(menuPanel);
        }
    }



}
