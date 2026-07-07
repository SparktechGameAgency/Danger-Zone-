using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Handles Login + Signup panels backed by PlayFab (email/password auth,
/// which enforces one-account-per-email natively). Basic tracking info
/// (email, first login time, last login time) is stored directly in
/// PlayFab's own Player Data, so no external database is needed.
/// </summary>
public class PlayFabAuthManager : MonoBehaviour
{
    [Header("Canvas Manager")]
    public CanvasManager canvasManager;

    [Header("Panels")]
    public GameObject loadingPanel;
    public GameObject loginPanel;
    public GameObject signupPanel;
    public GameObject namePanel;
    public GameObject menuPanel; // where the player lands after successful login/signup

    [Header("Name Panel Fields")]
    public TMP_InputField nameInput;
    public TMP_Text nameErrorText;
    public Button nameConfirmButton;

    [Header("Login Panel Fields")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;
    public TMP_Text loginErrorText;
    public Button loginButton;
    public Button goToSignupButton;

    [Header("Signup Panel Fields")]
    public TMP_InputField signupEmailInput;
    public TMP_InputField signupPasswordInput;
    public TMP_InputField signupConfirmPasswordInput;
    public TMP_Text signupErrorText;
    public Button signupButton;
    public Button goToLoginButton;

    private static readonly Regex GmailRegex =
        new Regex(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", RegexOptions.IgnoreCase);

    private const string SavedEmailKey = "DZ_SavedEmail";
    private const string SavedPasswordKey = "DZ_SavedPassword";

    void Start()
    {
        Debug.Log("[PlayFabAuthManager] Start() running — taking control of panel flow.");

        loginButton.onClick.AddListener(OnLoginButtonClicked);
        signupButton.onClick.AddListener(OnSignupButtonClicked);
        goToSignupButton.onClick.AddListener(() => SwitchPanel(true));
        goToLoginButton.onClick.AddListener(() => SwitchPanel(false));
        nameConfirmButton.onClick.AddListener(OnNameConfirmClicked);

        canvasManager.ShowOnly(loadingPanel);
        TryAutoLogin();
    }

    void TryAutoLogin()
    {
        if (PlayerPrefs.HasKey(SavedEmailKey) && PlayerPrefs.HasKey(SavedPasswordKey))
        {
            string savedEmail = PlayerPrefs.GetString(SavedEmailKey);
            string savedPassword = PlayerPrefs.GetString(SavedPasswordKey);

            var request = new LoginWithEmailAddressRequest
            {
                Email = savedEmail,
                Password = savedPassword
            };

            PlayFabClientAPI.LoginWithEmailAddress(request, OnAutoLoginSuccess, OnAutoLoginFailure);
        }
        else
        {
            canvasManager.ShowOnly(loginPanel);
        }
    }

    void OnAutoLoginSuccess(LoginResult result)
    {
        Debug.Log("Auto-login successful. PlayFabId: " + result.PlayFabId);
        TrackPlayerLogin(PlayerPrefs.GetString(SavedEmailKey), isNewAccount: false);
        AdManager.Instance?.FetchAdsRemovedStatus();
        canvasManager.ShowOnly(menuPanel);
    }

    void OnAutoLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Auto-login failed, clearing saved credentials: " + error.GenerateErrorReport());
        PlayerPrefs.DeleteKey(SavedEmailKey);
        PlayerPrefs.DeleteKey(SavedPasswordKey);
        canvasManager.ShowOnly(loginPanel);
    }

    void SaveCredentials(string email, string password)
    {
        PlayerPrefs.SetString(SavedEmailKey, email);
        PlayerPrefs.SetString(SavedPasswordKey, password);
        PlayerPrefs.Save();
    }

    void SwitchPanel(bool showSignup)
    {
        canvasManager.ShowOnly(showSignup ? signupPanel : loginPanel);
        loginErrorText.text = "";
        signupErrorText.text = "";
    }

    // ---------------- SIGNUP ----------------

    void OnSignupButtonClicked()
    {
        string email = signupEmailInput.text.Trim();
        string password = signupPasswordInput.text;
        string confirmPassword = signupConfirmPasswordInput.text;

        Debug.Log("[Signup] Raw email read from input field: '" + email + "' (length: " + email.Length + ")");

        if (!GmailRegex.IsMatch(email))
        {
            signupErrorText.text = "Email must be a valid address ending in @gmail.com";
            return;
        }

        if (password.Length < 6)
        {
            signupErrorText.text = "Password must be at least 6 characters.";
            return;
        }

        if (password != confirmPassword)
        {
            signupErrorText.text = "Passwords do not match.";
            return;
        }

        signupButton.interactable = false;
        signupErrorText.text = "Creating account...";

        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnSignupSuccess, OnSignupFailure);
    }

    void OnSignupSuccess(RegisterPlayFabUserResult result)
    {
        signupButton.interactable = true;
        signupErrorText.text = "Account created!";

        string email = signupEmailInput.text.Trim();
        string password = signupPasswordInput.text;

        TrackPlayerLogin(email, isNewAccount: true);
        SaveCredentials(email, password);

        Debug.Log("PlayFab signup successful. PlayFabId: " + result.PlayFabId);
        canvasManager.ShowOnly(namePanel);
    }

    void OnSignupFailure(PlayFabError error)
    {
        signupButton.interactable = true;

        if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
        {
            signupErrorText.text = "That email is already registered. Try logging in instead.";
        }
        else
        {
            signupErrorText.text = "Signup failed: " + error.ErrorMessage;
        }

        Debug.LogError("PlayFab signup failed: " + error.GenerateErrorReport());
    }

    // ---------------- LOGIN ----------------

    void OnLoginButtonClicked()
    {
        string email = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text;

        if (!GmailRegex.IsMatch(email))
        {
            loginErrorText.text = "Email must be a valid address ending in @gmail.com";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            loginErrorText.text = "Password cannot be empty.";
            return;
        }

        loginButton.interactable = false;
        loginErrorText.text = "Logging in...";

        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        loginButton.interactable = true;
        loginErrorText.text = "";

        string email = loginEmailInput.text.Trim();
        string password = loginPasswordInput.text;

        TrackPlayerLogin(email, isNewAccount: false);
        SaveCredentials(email, password);

        Debug.Log("PlayFab login successful. PlayFabId: " + result.PlayFabId);
        AdManager.Instance?.FetchAdsRemovedStatus();
        canvasManager.ShowOnly(menuPanel);
    }

    void OnLoginFailure(PlayFabError error)
    {
        loginButton.interactable = true;
        loginErrorText.text = "Login failed: " + error.ErrorMessage;
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
    }

    // ---------------- NAME PANEL (post-signup) ----------------

    void OnNameConfirmClicked()
    {
        if (!PlayFabClientAPI.IsClientLoggedIn())
        {
            nameErrorText.text = "Session expired — please log in again.";
            Debug.LogWarning("NamePanel reached without an active PlayFab session. Returning to Login.");
            canvasManager.ShowOnly(loginPanel);
            return;
        }

        string displayName = nameInput.text.Trim();

        if (displayName.Length < 3)
        {
            nameErrorText.text = "Name must be at least 3 characters.";
            return;
        }

        nameConfirmButton.interactable = false;
        nameErrorText.text = "Saving...";

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnNameSuccess, OnNameFailure);
    }

    void OnNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        nameConfirmButton.interactable = true;
        AdManager.Instance?.FetchAdsRemovedStatus();
        canvasManager.ShowOnly(menuPanel);
    }

    void OnNameFailure(PlayFabError error)
    {
        nameConfirmButton.interactable = true;
        nameErrorText.text = "Failed to save name: " + error.ErrorMessage;
        Debug.LogError("UpdateUserTitleDisplayName failed: " + error.GenerateErrorReport());
    }

    // ---------------- TRACKING (via PlayFab Player Data) ----------------

    /// <summary>
    /// Stores basic login tracking info directly on the player's PlayFab
    /// account using Player Data. Visible per-player in the dashboard under
    /// Players -> [PlayFabId] -> Data. No external database required, since
    /// PlayFab already logs the login itself (Players tab / Login History).
    /// </summary>
    void TrackPlayerLogin(string email, bool isNewAccount)
    {
        var data = new Dictionary<string, string>
        {
            { "email", email },
            { "lastLogin", DateTime.UtcNow.ToString("o") }
        };

        if (isNewAccount)
        {
            data["createdAt"] = DateTime.UtcNow.ToString("o");
        }

        var request = new UpdateUserDataRequest
        {
            Data = data,
            Permission = UserDataPermission.Private
        };

        PlayFabClientAPI.UpdateUserData(request,
            result => Debug.Log("PlayFab player data tracking updated."),
            error => Debug.LogError("PlayFab player data tracking failed: " + error.GenerateErrorReport())
        );
    }
}