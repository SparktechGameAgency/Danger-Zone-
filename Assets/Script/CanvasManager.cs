using UnityEngine;
using DG.Tweening;

public class CanvasManager : MonoBehaviour
{
    [Header("Panels managed by this canvas")]
    public GameObject[] panels;

    private GameObject currentPanel;

    [Header("Game Panels")]
    public GameObject basicGamePanel;   // Reference for Basic Game Panel
    public GameObject sandGamePanel;    // Reference for Sand Game Panel




    // Hide all panels instantly (no animation)
    public void HideAll()
    {
        foreach (var panel in panels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        currentPanel = null;
    }




    public void ActivateThemePanel(string theme)
    {
        // First, deactivate both game panels
        basicGamePanel.SetActive(false);
        sandGamePanel.SetActive(false);

        // Now, activate the selected theme panel
        if (theme == "basic")
        {
            basicGamePanel.SetActive(true);
        }
        else if (theme == "sand")
        {
            sandGamePanel.SetActive(true);
        }
    }



    // Smoothly transition to target panel
    public void ShowOnly(GameObject target)
    {
        if (target == null)
            return;

        // if the panel is already open, ignore
        if (currentPanel == target)
            return;

        // hide previous with animation
        if (currentPanel != null)
        {
            CanvasGroup oldCg = currentPanel.GetComponent<CanvasGroup>();
            RectTransform oldRt = currentPanel.GetComponent<RectTransform>();

            if (oldCg != null)
            {
                oldCg.interactable = false;
                oldCg.blocksRaycasts = false;

                oldCg.DOFade(0f, 0.25f);
            }

            if (oldRt != null)
            {
                oldRt.DOScale(0.9f, 0.25f)
                     .SetEase(Ease.InBack)
                     .OnComplete(() =>
                     {
                         currentPanel.SetActive(false);
                     });
            }
            else
            {
                currentPanel.SetActive(false);
            }
        }

        // show new panel
        target.SetActive(true);
        currentPanel = target;

        CanvasGroup newCg = target.GetComponent<CanvasGroup>();
        RectTransform newRt = target.GetComponent<RectTransform>();

        if (newCg != null)
        {
            newCg.alpha = 0f;
            newCg.interactable = true;
            newCg.blocksRaycasts = true;

            newCg.DOFade(1f, 0.35f);
        }

        if (newRt != null)
        {
            newRt.localScale = Vector3.one * 0.9f;

            newRt.DOScale(1f, 0.35f)
                 .SetEase(Ease.OutBack);
        }
    }

    // Show specific panel without hiding others (animated)
    public void Show(GameObject target)
    {
        if (target == null)
            return;

        target.SetActive(true);

        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        RectTransform rt = target.GetComponent<RectTransform>();

        if (cg != null)
        {
            cg.alpha = 0f;
            cg.DOFade(1f, 0.3f);
        }

        if (rt != null)
        {
            rt.localScale = Vector3.one * 0.9f;
            rt.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    // Hide specific panel (animated)
    public void Hide(GameObject target)
    {
        if (target == null)
            return;

        CanvasGroup cg = target.GetComponent<CanvasGroup>();
        RectTransform rt = target.GetComponent<RectTransform>();

        if (cg != null)
            cg.DOFade(0f, 0.25f);

        if (rt != null)
        {
            rt.DOScale(0.9f, 0.25f)
              .SetEase(Ease.InBack)
              .OnComplete(() =>
              {
                  target.SetActive(false);
              });
        }
        else
        {
            target.SetActive(false);
        }
    }

    // Toggle specific panel (animated)
    public void Toggle(GameObject target)
    {
        if (target == null)
            return;

        if (target.activeSelf)
            Hide(target);
        else
            Show(target);
    }

    //public void OnBackButtonPressed()
    //{
    //    // Hide the theme panel and show the menu panel
    //    HidePanel(themePanel);
    //    ShowPanel(menuPanel);
    //}

}
