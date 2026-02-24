using UnityEngine;
using System.Collections;

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

    [Header("DangerZone Managers (drag the root of each prefab)")]
    public DangerZoneManager sandManager;
    public DangerZoneManager basicManager;

    // PlayerPrefs key — persists the chosen theme across sessions
    private const string PREF_KEY = "SelectedTheme";
    private const string SAND = "sand";
    private const string BASIC = "basic";

    private string selectedTheme;

    private void Awake()
    {
        // Load saved theme (defaults to basic if never set)
        selectedTheme = PlayerPrefs.GetString(PREF_KEY, BASIC);
    }

    private void Start()
    {
        // Always begin on the menu, both game panels are off
        gamePanelSand.SetActive(false);
        gamePanelBasic.SetActive(false);
        canvasManager.ShowOnly(menuPanel);
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

    // ─────────────────────────────────────────────────────────────────────────
    // THEME PANEL BUTTONS
    // ─────────────────────────────────────────────────────────────────────────

    // Wire to: SAND SELECT button → ThemeManager.OnSelectSand()
    public void OnSelectSand()
    {
        selectedTheme = SAND;
        PlayerPrefs.SetString(PREF_KEY, SAND);
        PlayerPrefs.Save();
        canvasManager.ShowOnly(menuPanel);
    }

    // Wire to: BASIC SELECT button → ThemeManager.OnSelectBasic()
    public void OnSelectBasic()
    {
        selectedTheme = BASIC;
        PlayerPrefs.SetString(PREF_KEY, BASIC);
        PlayerPrefs.Save();
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
        // 1. Turn everything off
        canvasManager.HideAll();

        // 2. Turn the chosen game panel ON
        panel.SetActive(true);

        // 3. Wait 2 frames — Unity needs this to fully wake up the panel
        //    and ALL child components before any coroutine can run on them
        yield return null;
        yield return null;

        // 4. Start the game — guaranteed safe now
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