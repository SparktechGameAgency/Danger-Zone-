using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DangerZoneManager : MonoBehaviour
{
    public Button[] cardButtons;

    public Sprite[] faceDownSprites;
    public Sprite safeSprite;
    public Sprite bombSprite;

    private int bombIndex;
    private bool cardChosen = false;
    private int clickedIndex = -1;

    void Start()
    {
        SetupLevel();
    }

    void SetupLevel()
    {
        cardChosen = false;

        bombIndex = Random.Range(0, cardButtons.Length);

        for (int i = 0; i < cardButtons.Length; i++)
        {
            int index = i;

            cardButtons[i].image.sprite = faceDownSprites[i];

            // IMPORTANT: reset rotation properly
            cardButtons[i].transform.localRotation = Quaternion.Euler(0, 0, 0);

            cardButtons[i].onClick.RemoveAllListeners();
            cardButtons[i].onClick.AddListener(() => OnCardClicked(index));
        }
    }

    void OnCardClicked(int index)
    {
        if (cardChosen)
            return;

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
            if (i == clickedIndex)
                continue;

            StartCoroutine(FlipCard(i, false));
        }
    }

    IEnumerator FlipCard(int index, bool showResultImmediately)
    {
        RectTransform card = cardButtons[index].transform as RectTransform;

        // PRESS animation
        Vector3 original = card.localScale;
        card.localScale = original * 0.9f;
        yield return new WaitForSeconds(0.05f);
        card.localScale = original;

        float duration = 0.25f;
        float elapsed = 0f;

        // 0° -> 90°
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float yRot = Mathf.Lerp(0, 90, elapsed / duration);
            card.localRotation = Quaternion.Euler(0, yRot, 0);
            yield return null;
        }

        // ensure exactly 90°
        card.localRotation = Quaternion.Euler(0, 90, 0);

        // 🔁 swap sprite at midpoint
        if (index == bombIndex)
            cardButtons[index].image.sprite = bombSprite;
        else
            cardButtons[index].image.sprite = safeSprite;

        // 90° -> 180°
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float yRot = Mathf.Lerp(90, 180, elapsed / duration);
            card.localRotation = Quaternion.Euler(0, yRot, 0);
            yield return null;
        }

        // ensure exactly 180°
        card.localRotation = Quaternion.Euler(0, 180, 0);
    }
}
