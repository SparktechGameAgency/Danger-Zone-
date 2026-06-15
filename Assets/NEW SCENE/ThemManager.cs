using UnityEngine;
using System.Collections;
using UnityEngine.UI;



public class ThemeManager : MonoBehaviour
{
    [Header("Canvas Manager")]
    public CanvasManager canvasManager;

    [Header("Panels")]
    public GameObject menuPanel;
    public GameObject themePanel;
    public GameObject gamePanelSand;
    public GameObject gamePanelBasic;

    public GameObject Setting;
    public GameObject HowToPlay;


    [Header("Extra Panels")]
    public GameObject howToPlayPanel;
    public GameObject settingPanel;
    
    public GameObject aboutPanel;
    public GameObject removeadsPanel;

    [Header("DangerZone Managers (drag the root of each prefab)")]
    public DangerZoneManager sandManager;
    public DangerZoneManager basicManager;

    // PlayerPrefs key — persists the chosen theme across sessions
    private const string PREF_KEY = "SelectedTheme";
    private const string SAND = "sand";
    private const string BASIC = "basic";


    [Header("Theme Button Images")]
    public Image basicThemeImage;
    public Image sandThemeImage;

    public Sprite basicNormalSprite;
    public Sprite basicSelectedSprite;

    public Sprite sandNormalSprite;
    public Sprite sandSelectedSprite;

    private string selectedTheme;



    private void Awake()
    {
        selectedTheme = PlayerPrefs.GetString(PREF_KEY, SAND);
        UpdateThemeUI();
    }



    private void Start()
    {
        // Always begin on the menu, both game panels are off
        gamePanelSand.SetActive(false);
        gamePanelBasic.SetActive(false);
        //canvasManager.ShowOnly(menuPanel);
    }

    private void UpdateThemeUI()
    {
        if (selectedTheme == BASIC)
        {
            basicThemeImage.sprite = basicSelectedSprite;
            sandThemeImage.sprite = sandNormalSprite;
        }
        else
        {
            basicThemeImage.sprite = basicNormalSprite;
            sandThemeImage.sprite = sandSelectedSprite;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // MENU PANEL BUTTONS
    // ─────────────────────────────────────────────────────────────────────────

    // Wire to: PLAY button → ThemeManager.OnPlayPressed()
    public void OnPlayPressed()
    {
        if (selectedTheme == SAND)
            StartCoroutine(LaunchGame(gamePanelSand, sandManager));
        else
            StartCoroutine(LaunchGame(gamePanelBasic, basicManager));
    }

    // Wire to: THEME button → ThemeManager.OnThemePressed()
    public void OnThemePressed()
    {
        canvasManager.ShowOnly(themePanel);
    }


    public void SettingPressed()
    {
        canvasManager.ShowOnly(Setting);
    }

    public void AboutPressed()
    {
        canvasManager.ShowOnly(aboutPanel);
    }

    public void RemoveAdsPressed()
    {
        canvasManager.ShowOnly(removeadsPanel);
    }
    




    // ─────────────────────────────────────────────────────────────────────────
    // THEME PANEL BUTTONS
    // ─────────────────────────────────────────────────────────────────────────

    // Wire to: SAND SELECT button → ThemeManager.OnSelectSand()
    public void OnSelectSand()
    {
        selectedTheme = SAND;

        PlayerPrefs.SetString(PREF_KEY, SAND);
        PlayerPrefs.Save();

        UpdateThemeUI();
        canvasManager.ShowOnly(menuPanel);
    }

    // Wire to: BASIC SELECT button → ThemeManager.OnSelectBasic()
    public void OnSelectBasic()
    {
        selectedTheme = BASIC;

        PlayerPrefs.SetString(PREF_KEY, BASIC);
        PlayerPrefs.Save();

        UpdateThemeUI();
        canvasManager.ShowOnly(menuPanel);
    }

    // Wire to: BACK button in theme panel → ThemeManager.OnThemeBackPressed()
    public void OnThemeBackPressed()
    {
        canvasManager.ShowOnly(menuPanel);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // INTERNAL: Launch the game safely
    // ─────────────────────────────────────────────────────────────────────────
    private IEnumerator LaunchGame(GameObject panel, DangerZoneManager manager)
    {
        canvasManager.HideAll();

        panel.SetActive(true);

        yield return null;
        yield return null;

        // 🎵 Switch to Game Music
        FindObjectOfType<audioManager>().PlayGameMusic();

        manager.LoadLevel(0);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Called by DangerZoneManager.GoHome() to return to menu
    // ─────────────────────────────────────────────────────────────────────────
    public void GoToMenu()
    {
        gamePanelSand.SetActive(false);
        gamePanelBasic.SetActive(false);

        canvasManager.ShowOnly(menuPanel);

        // 🎵 Switch back to menu music
        FindObjectOfType<audioManager>().PlayMenuMusic();
    }

    // MENU BUTTON → Settings
    public void OnSettingsPressed()
    {
        canvasManager.ShowOnly(settingPanel);
    }

    // MENU BUTTON → How To Play
    public void OnHowToPlayPressed()
    {
        canvasManager.ShowOnly(howToPlayPanel);
    }

    // BACK button inside Settings
    public void OnSettingsBackPressed()
    {
        canvasManager.ShowOnly(menuPanel);
    }

    // BACK button inside How To Play
    public void OnHowToPlayBackPressed()
    {
        canvasManager.ShowOnly(menuPanel);
    }


}