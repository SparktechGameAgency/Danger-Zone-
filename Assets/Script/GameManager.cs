using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class DangerZoneManager : MonoBehaviour
{
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

    private int currentLevel = 0;
    private bool cardChosen = false;

    private List<int> bombIndexes = new List<int>();
    private int clickedIndex = -1;

    void Start()
    {
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int levelIndex)
    {
        HidePanel(successPanel);
        HidePanel(failPanel);

        cardChosen = false;
        clickedIndex = -1;
        bombIndexes.Clear();

        levelIndex = Mathf.Clamp(levelIndex, 0, levels.Length - 1);
        LevelData level = levels[levelIndex];

        levelIndicatorImage.sprite = level.levelIndicator;

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
            cardButtons[i].transform.localRotation = Quaternion.Euler(0, 0, 0);

            cardButtons[i].onClick.RemoveAllListeners();
            cardButtons[i].onClick.AddListener(() => OnCardClicked(index));
        }
    }

    void OnCardClicked(int index)
    {
        if (cardChosen) return;

        cardChosen = true;
        clickedIndex = index;

        StartCoroutine(FlipCard(index, true));
        StartCoroutine(RevealOthersAfterDelay());
    }

    IEnumerator RevealOthersAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (i == clickedIndex) continue;
            StartCoroutine(FlipCard(i, false));
        }

        yield return new WaitForSeconds(1f);

        if (bombIndexes.Contains(clickedIndex))
            ShowPanel(failPanel);
        else
            ShowPanel(successPanel);
    }

    IEnumerator FlipCard(int index, bool immediateReveal)
    {
        RectTransform card = cardButtons[index].transform as RectTransform;

        Vector3 original = card.localScale;
        card.localScale = original * 0.9f;
        yield return new WaitForSeconds(0.05f);
        card.localScale = original;

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

        if (bombIndexes.Contains(index))
            cardButtons[index].image.sprite = bombSprite;
        else
            cardButtons[index].image.sprite = safeSprite;

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

    public void NextLevel()
    {
        currentLevel++;

        if (currentLevel >= levels.Length)
            currentLevel = levels.Length - 1;

        LoadLevel(currentLevel);
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

        rt.DOScale(Vector3.one, 0.6f)
          .SetEase(Ease.OutElastic);
    }

    void HidePanel(GameObject panel)
    {
        RectTransform rt = panel.GetComponent<RectTransform>();

        rt.DOScale(Vector3.zero, 0.3f)
          .SetEase(Ease.InBack)
          .OnComplete(() => panel.SetActive(false));
    }

    public void RestartGame()
    {
        currentLevel = 0;
        cardChosen = false;
        clickedIndex = -1;
        bombIndexes.Clear();

        LoadLevel(currentLevel);
    }

    private void OnEnable()
    {
        RestartGame();
    }
}
