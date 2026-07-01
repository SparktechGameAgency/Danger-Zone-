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
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject signupPanel;

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

    void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        signupButton.onClick.AddListener(OnSignupButtonClicked);
        goToSignupButton.onClick.AddListener(() => SwitchPanel(true));
        goToLoginButton.onClick.AddListener(() => SwitchPanel(false));

        SwitchPanel(false); // default to login panel
    }

    void SwitchPanel(bool showSignup)
    {
        loginPanel.SetActive(!showSignup);
        signupPanel.SetActive(showSignup);
        loginErrorText.text = "";
        signupErrorText.text = "";
    }

    // ---------------- SIGNUP ----------------

    void OnSignupButtonClicked()
    {
        string email = signupEmailInput.text.Trim();
        string password = signupPasswordInput.text;
        string confirmPassword = signupConfirmPasswordInput.text;

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
        TrackPlayerLogin(email, isNewAccount: true);

        Debug.Log("PlayFab signup successful. PlayFabId: " + result.PlayFabId);
        // TODO: proceed to main menu / gameplay
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
        TrackPlayerLogin(email, isNewAccount: false);

        Debug.Log("PlayFab login successful. PlayFabId: " + result.PlayFabId);
        // TODO: proceed to main menu / gameplay
    }

    void OnLoginFailure(PlayFabError error)
    {
        loginButton.interactable = true;
        loginErrorText.text = "Login failed: " + error.ErrorMessage;
        Debug.LogError("PlayFab login failed: " + error.GenerateErrorReport());
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