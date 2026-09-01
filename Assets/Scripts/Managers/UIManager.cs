using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PopupManager popupManager;
    [SerializeField] private JSFunctCalls jsFunctCalls;
    [SerializeField] private OrientationChange orientationChange;

    [Header("Loading & Intro")]
    [SerializeField] private GameObject gameScreen;

    [Header("Bet Controls")]
    [SerializeField] private TMP_Text betAmountText;
    [SerializeField] private Button betPlusButton;
    [SerializeField] private Button betMinusButton;
    [Header("Bet Controls - Portrait")]
    [SerializeField] private TMP_Text betAmountTextPortrait;
    [SerializeField] private Button betPlusButtonPortrait;
    [SerializeField] private Button betMinusButtonPortrait;

    [Header("Balance & Win")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text winAmountText;
    [SerializeField] private GameObject winTextObject;
    [SerializeField] private GameObject goodLuckObject;
    [Header("Balance & Win - Portrait")]
    [SerializeField] private TMP_Text balanceTextPortrait;
    [SerializeField] private TMP_Text winAmountTextPortrait;
    [SerializeField] private GameObject winTextObjectPortrait;
    [SerializeField] private GameObject goodLuckObjectPortrait;

    [Header("Spin Button")]
    [SerializeField] private Button spinButton;
    [SerializeField] private Button stopButton;
    [Header("Spin Button - Portrait")]
    [SerializeField] private Button spinButtonPortrait;
    [SerializeField] private Button stopButtonPortrait;

    [Header("Auto Play Stop Control")]
    [SerializeField] private Button autoSpinStopButton;
    [SerializeField] private TMP_Text autoSpinRemainingText;
    [Header("Auto Play Stop Control - Portrait")]
    [SerializeField] private Button autoSpinStopButtonPortrait;
    [SerializeField] private TMP_Text autoSpinRemainingTextPortrait;

    [Header("Auto Play Panel")]
    [SerializeField] private GameObject autoPlayPanel;
    [SerializeField] private RectTransform autoPlayPanelRect;
    [SerializeField] private Button autoPlayCloseButton;
    [Header("Auto Play Selection Buttons")]
    [SerializeField] private Button autoPlay10Button;
    [SerializeField] private Button autoPlay50Button;
    [SerializeField] private Button autoPlay100Button;
    [SerializeField] private Button autoPlay200Button;
    [SerializeField] private Button autoPlay500Button;
    [SerializeField] private Button autoPlayInfiniteButton;

    [Header("Auto Play Panel - Portrait")]
    [SerializeField] private GameObject autoPlayPanelPortrait;
    [SerializeField] private RectTransform autoPlayPanelRectPortrait;
    [SerializeField] private Button autoPlayCloseButtonPortrait;
    [SerializeField] private Button autoPlay10ButtonPortrait;
    [SerializeField] private Button autoPlay50ButtonPortrait;
    [SerializeField] private Button autoPlay100ButtonPortrait;
    [SerializeField] private Button autoPlay200ButtonPortrait;
    [SerializeField] private Button autoPlay500ButtonPortrait;
    [SerializeField] private Button autoPlayInfiniteButtonPortrait;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private RectTransform settingsPanelRect;
    [SerializeField] private Button settingsOpenButton;
    [SerializeField] private Button settingsCloseButton;
    [SerializeField] private Button settingsBgCloseButton;
    [SerializeField] private Button gameQuitButton;
    [Header("Settings Panel - Portrait")]
    [SerializeField] private GameObject settingsPanelPortrait;
    [SerializeField] private RectTransform settingsPanelRectPortrait;
    [SerializeField] private Button settingsOpenButtonPortrait;
    [SerializeField] private Button settingsCloseButtonPortrait;
    [SerializeField] private Button settingsBgCloseButtonPortrait;
    [SerializeField] private Button gameQuitButtonPortrait;

    [Header("Speed Buttons (Three-Layer Toggle)")]
    [SerializeField] private Button normalSpeedButton;
    [SerializeField] private Button turboSpeedButton;
    [SerializeField] private Button quickSpeedButton;
    [Header("Speed Buttons - Portrait")]
    [SerializeField] private Button normalSpeedButtonPortrait;
    [SerializeField] private Button turboSpeedButtonPortrait;
    [SerializeField] private Button quickSpeedButtonPortrait;

    [Header("Sound Panel")]
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private RectTransform soundPanelRect;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button soundPanelCloseButton;
    [SerializeField] private Button soundPanelOpenButton;
    [SerializeField] private Button soundPanelOpenButtonPortrait;

    [Header("Game Rules Panel")]
    [SerializeField] private GameObject gameRulesPanel;
    [SerializeField] private RectTransform gameRulesPanelRect;
    [SerializeField] private Button gameRulesOpenButton;
    [SerializeField] private Button gameRulesBackButton;
    [Header("Game Rules Panel - Portrait")]
    [SerializeField] private Button gameRulesOpenButtonPortrait;

    [Header("Guide Panel")]
    [SerializeField] private GameObject guidePanel;
    [SerializeField] private RectTransform guidePanelRect;
    [SerializeField] private Button guideOpenButton;
    [SerializeField] private Button guideBackButton;
    [Header("Guide Panel - Portrait")]
    [SerializeField] private Button guideOpenButtonPortrait;

    [Header("Game Rules Dynamic Texts - 6 Symbols")]
    [SerializeField] private TMP_Text totalLineCountText;
    [SerializeField] private TMP_Text ruleRed7Text;
    [SerializeField] private TMP_Text rulePurple7Text;
    [SerializeField] private TMP_Text ruleBlue7Text;
    [SerializeField] private TMP_Text ruleWhiteBar7Text;
    [SerializeField] private TMP_Text ruleTripleBarText;
    [SerializeField] private TMP_Text ruleDoubleBarText;

    [Header("Game Rules Dynamic Texts - Combination Features")]
    [SerializeField] private TMP_Text ruleAnyBarsText;
    [SerializeField] private TMP_Text ruleBarWhite7Text;
    [SerializeField] private TMP_Text ruleMixedSevensText;


    [Header("Ping Display")]
    [SerializeField] private TMP_Text pingText;
    [SerializeField] private TMP_Text pingTextPortrait;

    [Header("Platform Jackpot")]
    [SerializeField] private TMP_Text grandJackpotText;
    [SerializeField] private TMP_Text majorJackpotText;
    [SerializeField] private TMP_Text minorJackpotText;
    [SerializeField] private TMP_Text miniJackpotText;

    [Header("Platform Jackpot - Portrait")]
    [SerializeField] private TMP_Text grandJackpotTextPortrait;
    [SerializeField] private TMP_Text majorJackpotTextPortrait;
    [SerializeField] private TMP_Text minorJackpotTextPortrait;
    [SerializeField] private TMP_Text miniJackpotTextPortrait;

    [Header("Platform Jackpot Animation - Portrait")]
    [SerializeField] private RectTransform grandJackpotPortraitParent;
    [SerializeField] private RectTransform majorJackpotPortraitParent;
    [SerializeField] private RectTransform minorJackpotPortraitParent;
    [SerializeField] private RectTransform miniJackpotPortraitParent;
    [SerializeField] private bool enableJackpotPortraitLevitation = true;
    [SerializeField] private float jackpotLevitateHeight = 10f;
    [SerializeField] private float jackpotLevitateDuration = 1.4f;
    [SerializeField] private float jackpotStaggerDelay = 0.15f;

    private readonly Dictionary<Transform, Vector3> jackpotInitialLocalPositions = new Dictionary<Transform, Vector3>();
    private readonly List<Tween> jackpotPortraitTweens = new List<Tween>();

    [Header("Expand-Shrink Controls")]
    [SerializeField] private Button expandButton;
    [SerializeField] private Button shrinkButton;
    [Header("Expand-Shrink Controls - Portrait")]
    [SerializeField] private Button expandButtonPortrait;
    [SerializeField] private Button shrinkButtonPortrait;

    private bool isExpanded = false;
    private bool isSettingsPanelOpen = false;

    private Tween balanceTween;
    private Tween winTween;

    [SerializeField] private float rapidStopCooldown = 1f;
    private float lastRapidStopTime = -99f;

    [Header("UI State")]
    private double currentWinDisplayValue = 0;



    private void Awake()
    {
        if (jsFunctCalls != null)
        {
            jsFunctCalls.RegisterVisibilityListener(gameObject.name);
        }
    }



    public void OnFocusChanged(string value)
    {
        bool focused = value == "1";
        AudioManager.Instance?.SetMuteAll(!focused);
        if (gameManager != null && gameManager.socketManager != null)
        {
            gameManager.socketManager.HandleFocusChange(focused);
        }
    }

    private void Start()
    {
        SetupButtons();
        SetupAutoPlayPanel();
        SetupSettingsPanel();
        SetupGameRulesPanel();
        SetupGuidePanel();

        InitializeExpandShrink();

        if (gameScreen) gameScreen.SetActive(true);
        InitializeUI();
        StartCoroutine(WaitForInitialization());
        RegisterFullscreenListener();
        UpdateJackpotPortraitLevitationFromCurrentOrientation();
    }

    private void OnEnable()
    {
        OrientationChange.OnOrientationChanged += HandleOrientationChanged;
        var oc = GetOrientationChange();
        if (oc != null)
        {
            oc.OnOrientationChangedInstance += HandleOrientationChanged;
        }
    }

    private void OnDisable()
    {
        OrientationChange.OnOrientationChanged -= HandleOrientationChanged;
        if (orientationChange != null)
        {
            orientationChange.OnOrientationChangedInstance -= HandleOrientationChanged;
        }
        StopJackpotPortraitLevitation();
    }

    private void HandleOrientationChanged(OrientationChange.OrientationMode mode, int width, int height)
    {
        UpdateJackpotPortraitLevitation(mode);
    }

    private OrientationChange GetOrientationChange()
    {
        if (orientationChange == null)
        {
            orientationChange = Object.FindFirstObjectByType<OrientationChange>();
        }
        return orientationChange;
    }

    private void InitializeUI()
    {
        if (soundPanel) soundPanel.SetActive(false);
        SetGameObjectActive(autoPlayPanel, autoPlayPanelPortrait, false);
        if (autoPlayPanelRect) autoPlayPanelRect.anchoredPosition = new Vector2(autoPlayPanelRect.anchoredPosition.x, -600f);
        if (autoPlayPanelRectPortrait) autoPlayPanelRectPortrait.anchoredPosition = new Vector2(autoPlayPanelRectPortrait.anchoredPosition.x, -600f);
        SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, false);

        SetSpinStopButtonStates(isSpinningState: false, isInteractable: true);
        UpdateSpeedButtonsVisibility(gameManager.currentSpinSpeed);

        isSettingsPanelOpen = false;
        SetGameObjectActive(settingsPanel, settingsPanelPortrait, false);
        if (gameRulesPanel) gameRulesPanel.SetActive(false);
        if (guidePanel) guidePanel.SetActive(false);
        UpdatePingDisplay("-- ms");
    }

    #region Loading & Intro Sequence

    private IEnumerator WaitForInitialization()
    {
        float initializationTimeout = 20f;
        float timer = 0f;
        while (!gameManager.isInitialized && !gameManager.initializationFailed && timer < initializationTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (gameManager.initializationFailed || !gameManager.isInitialized)
        {
            if (gameManager.socketManager != null)
            {
                gameManager.socketManager.SetRaycastBlocker(false);
            }

            if (popupManager != null)
            {
                string errorMsg = gameManager.initializationFailed ? "Game failed to initialize." : "Initialization timed out. Please check your connection.";
                popupManager.ShowErrorPopup("Connection Error", errorMsg, true);
            }
        }
        else
        {
            AudioManager.Instance?.PlayBgMusic();
        }
    }

    #endregion

    #region UI Synchronization Helpers

    private void SetTMPText(TMP_Text text1, TMP_Text text2, string content)
    {
        if (text1) text1.text = content;
        if (text2) text2.text = content;
    }

    private void SetGameObjectActive(GameObject obj1, GameObject obj2, bool active)
    {
        if (obj1) obj1.SetActive(active);
        if (obj2) obj2.SetActive(active);
    }

    private void SetButtonInteractable(Button btn1, Button btn2, bool interactable)
    {
        if (btn1) btn1.interactable = interactable;
        if (btn2) btn2.interactable = interactable;
    }

    private void SetButtonActive(Button btn1, Button btn2, bool active)
    {
        if (btn1) btn1.gameObject.SetActive(active);
        if (btn2) btn2.gameObject.SetActive(active);
    }

    #endregion

    #region Button Setup

    private void SetupButtons()
    {
        if (betPlusButton) betPlusButton.onClick.AddListener(() => gameManager.IncreaseBet());
        if (betMinusButton) betMinusButton.onClick.AddListener(() => gameManager.DecreaseBet());
        if (betPlusButtonPortrait) betPlusButtonPortrait.onClick.AddListener(() => gameManager.IncreaseBet());
        if (betMinusButtonPortrait) betMinusButtonPortrait.onClick.AddListener(() => gameManager.DecreaseBet());

        if (spinButton)
        {
            var holdHandler = spinButton.GetComponent<SpinButtonHoldHandler>();
            if (holdHandler != null)
            {
                holdHandler.OnClick.AddListener(OnSpinButtonPressed);
                holdHandler.OnHoldThreeSeconds.AddListener(OnSpinButtonHeld);
            }
            else
            {
                spinButton.onClick.AddListener(OnSpinButtonPressed);
            }
        }
        if (spinButtonPortrait)
        {
            var holdHandler = spinButtonPortrait.GetComponent<SpinButtonHoldHandler>();
            if (holdHandler != null)
            {
                holdHandler.OnClick.AddListener(OnSpinButtonPressed);
                holdHandler.OnHoldThreeSeconds.AddListener(OnSpinButtonHeld);
            }
            else
            {
                spinButtonPortrait.onClick.AddListener(OnSpinButtonPressed);
            }
        }

        if (stopButton) stopButton.onClick.AddListener(OnStopButtonPressed);
        if (stopButtonPortrait) stopButtonPortrait.onClick.AddListener(OnStopButtonPressed);

        if (autoSpinStopButton)
        {
            autoSpinStopButton.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayAutoplayStop();
                gameManager.StopAutoPlay();
            });
        }
        if (autoSpinStopButtonPortrait)
        {
            autoSpinStopButtonPortrait.onClick.AddListener(() =>
            {
                AudioManager.Instance?.PlayAutoplayStop();
                gameManager.StopAutoPlay();
            });
        }

        if (autoPlayCloseButton) autoPlayCloseButton.onClick.AddListener(CloseAutoPlayPanel);
        if (autoPlayCloseButtonPortrait) autoPlayCloseButtonPortrait.onClick.AddListener(CloseAutoPlayPanel);

        if (gameQuitButton) gameQuitButton.onClick.AddListener(OnExitButtonPressed);
        if (gameQuitButtonPortrait) gameQuitButtonPortrait.onClick.AddListener(OnExitButtonPressed);

        if (expandButton) expandButton.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); OnExpand(); });
        if (shrinkButton) shrinkButton.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); OnShrink(); });
        if (expandButtonPortrait) expandButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); OnExpand(); });
        if (shrinkButtonPortrait) shrinkButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); OnShrink(); });

        if (normalSpeedButton) normalSpeedButton.onClick.AddListener(() => { AudioManager.Instance?.PlayTurboButtonClick(); SetSpeedMode(SpinSpeed.Turbo); });
        if (turboSpeedButton) turboSpeedButton.onClick.AddListener(() => { AudioManager.Instance?.PlayTurboButtonClick(); SetSpeedMode(SpinSpeed.QuickSpin); });
        if (quickSpeedButton) quickSpeedButton.onClick.AddListener(() => { AudioManager.Instance?.PlayTurboButtonClick(); SetSpeedMode(SpinSpeed.Normal); });
        if (normalSpeedButtonPortrait) normalSpeedButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayTurboButtonClick(); SetSpeedMode(SpinSpeed.Turbo); });
        if (turboSpeedButtonPortrait) turboSpeedButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayTurboButtonClick(); SetSpeedMode(SpinSpeed.QuickSpin); });
        if (quickSpeedButtonPortrait) quickSpeedButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayTurboButtonClick(); SetSpeedMode(SpinSpeed.Normal); });
    }

    private void SetupAutoPlayPanel()
    {
        if (autoPlay10Button) autoPlay10Button.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(10); });
        if (autoPlay50Button) autoPlay50Button.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(50); });
        if (autoPlay100Button) autoPlay100Button.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(100); });
        if (autoPlay200Button) autoPlay200Button.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(200); });
        if (autoPlay500Button) autoPlay500Button.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(500); });
        if (autoPlayInfiniteButton) autoPlayInfiniteButton.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(-1); });

        if (autoPlay10ButtonPortrait) autoPlay10ButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(10); });
        if (autoPlay50ButtonPortrait) autoPlay50ButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(50); });
        if (autoPlay100ButtonPortrait) autoPlay100ButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(100); });
        if (autoPlay200ButtonPortrait) autoPlay200ButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(200); });
        if (autoPlay500ButtonPortrait) autoPlay500ButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(500); });
        if (autoPlayInfiniteButtonPortrait) autoPlayInfiniteButtonPortrait.onClick.AddListener(() => { AudioManager.Instance?.PlayButton(); StartAutoplayWithRounds(-1); });
    }

    private void SetupSettingsPanel()
    {
        if (settingsOpenButton) settingsOpenButton.onClick.AddListener(() =>
        {
            if (isSettingsPanelOpen)
                CloseSettingsPanel();
            else
                OpenSettingsPanel();
        });
        if (settingsOpenButtonPortrait) settingsOpenButtonPortrait.onClick.AddListener(() =>
        {
            if (isSettingsPanelOpen)
                CloseSettingsPanel();
            else
                OpenSettingsPanel();
        });

        if (settingsCloseButton) settingsCloseButton.onClick.AddListener(CloseSettingsPanel);
        if (settingsCloseButtonPortrait) settingsCloseButtonPortrait.onClick.AddListener(CloseSettingsPanel);
        if (settingsBgCloseButton) settingsBgCloseButton.onClick.AddListener(CloseSettingsPanel);
        if (settingsBgCloseButtonPortrait) settingsBgCloseButtonPortrait.onClick.AddListener(CloseSettingsPanel);

        SetButtonActive(settingsOpenButton, settingsOpenButtonPortrait, true);
        SetButtonActive(settingsCloseButton, settingsCloseButtonPortrait, false);
        SetButtonActive(settingsBgCloseButton, settingsBgCloseButtonPortrait, false);

        if (soundPanelOpenButton) soundPanelOpenButton.onClick.AddListener(OpenSoundPanel);
        if (soundPanelOpenButtonPortrait) soundPanelOpenButtonPortrait.onClick.AddListener(OpenSoundPanel);
        if (soundPanelCloseButton) soundPanelCloseButton.onClick.AddListener(CloseSoundPanel);

        if (musicSlider)
        {
            if (AudioManager.Instance != null) musicSlider.value = AudioManager.Instance.MusicVolume;
            musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }
        if (sfxSlider)
        {
            if (AudioManager.Instance != null) sfxSlider.value = AudioManager.Instance.SfxVolume;
            sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        }
    }

    private void SetupGameRulesPanel()
    {
        if (gameRulesOpenButton) gameRulesOpenButton.onClick.AddListener(OpenGameRulesPanel);
        if (gameRulesOpenButtonPortrait) gameRulesOpenButtonPortrait.onClick.AddListener(OpenGameRulesPanel);

        if (gameRulesBackButton) gameRulesBackButton.onClick.AddListener(CloseGameRulesPanel);
    }

    private void SetupGuidePanel()
    {
        if (guideOpenButton) guideOpenButton.onClick.AddListener(OpenGuidePanel);
        if (guideOpenButtonPortrait) guideOpenButtonPortrait.onClick.AddListener(OpenGuidePanel);

        if (guideBackButton) guideBackButton.onClick.AddListener(CloseGuidePanel);
    }

    #endregion

    #region Game Events

    internal void OnGameInitialized()
    {
        currentWinDisplayValue = 0;
        UpdateBetDisplay();
        UpdateBalanceDisplay();
        UpdateWinDisplay(0);
    }

    internal void OnSpinStarted()
    {
        AudioManager.Instance?.PlaySpinStart();

        SetSpinStopButtonStates(isSpinningState: true, isInteractable: true);
        SetBetControlsEnabled(false);
        SetButtonInteractable(settingsOpenButton, settingsOpenButtonPortrait, true);

        UpdateBalanceDisplay();
        UpdateWinDisplay(0);

        CloseAutoPlayPanelImmediate();
    }

    internal void OnSpinResultReceived()
    {
        SetSpinStopButtonStates(isSpinningState: true, isInteractable: true);
    }

    internal void OnSpinStopping(SpinResult result = null)
    {
        UpdateBalanceDisplay();
        if (result != null)
        {
            UpdateWinDisplay(result.winAmount);
        }
    }

    internal void OnSpinCompleted(SpinResult result = null)
    {
        if (result != null)
        {
            UpdateWinDisplay(result.winAmount);
        }
        UpdateBalanceDisplay();

        if (gameManager.isAutoPlaying)
        {
            SetSpinStopButtonStates(isSpinningState: true, isInteractable: true);
        }
        else
        {
            SetSpinStopButtonStates(isSpinningState: false, isInteractable: true);

            SetBetControlsEnabled(true);
            SetButtonInteractable(settingsOpenButton, settingsOpenButtonPortrait, true);
        }
    }

    #endregion

    #region Spin Button

    public void OnSpinButtonPressed()
    {
        if (gameManager.isAutoPlaying)
        {
            AudioManager.Instance?.PlayAutoplayStop();
            gameManager.StopAutoPlay();
            return;
        }

        if (!gameManager.IsSpinning())
        {
            gameManager.RequestSpin();
        }
    }

    private void OnStopButtonPressed()
    {
        if (gameManager.isAutoPlaying)
        {
            AudioManager.Instance?.PlayAutoplayStop();
            gameManager.StopAutoPlay();
            return;
        }

        if (gameManager.IsSpinning())
        {
            if (Time.unscaledTime - lastRapidStopTime < rapidStopCooldown)
                return;

            lastRapidStopTime = Time.unscaledTime;
            AudioManager.Instance?.PlaySpinStop();
            gameManager.RequestStop();
        }
    }

    internal void DisableSpinButtonDuringStop()
    {
        if (gameManager.isAutoPlaying)
        {
            SetButtonActive(spinButton, spinButtonPortrait, false);
            SetButtonActive(stopButton, stopButtonPortrait, false);
            SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, true);
            SetButtonInteractable(autoSpinStopButton, autoSpinStopButtonPortrait, false);
        }
        else
        {
            SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, false);
            SetButtonActive(spinButton, spinButtonPortrait, false);
            SetButtonActive(stopButton, stopButtonPortrait, true);
            SetButtonInteractable(stopButton, stopButtonPortrait, false);
        }
    }

    internal void SetSpinStopButtonStates(bool isSpinningState, bool isInteractable)
    {
        if (gameManager.isAutoPlaying)
        {
            SetButtonActive(spinButton, spinButtonPortrait, false);
            SetButtonActive(stopButton, stopButtonPortrait, false);
            SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, true);
            SetButtonInteractable(autoSpinStopButton, autoSpinStopButtonPortrait, isInteractable);
        }
        else
        {
            SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, false);

            if (isSpinningState)
            {
                SetButtonActive(spinButton, spinButtonPortrait, false);
                SetButtonActive(stopButton, stopButtonPortrait, true);
                SetButtonInteractable(stopButton, stopButtonPortrait, isInteractable);
            }
            else
            {
                SetButtonActive(stopButton, stopButtonPortrait, false);
                SetButtonActive(spinButton, spinButtonPortrait, true);
                SetButtonInteractable(spinButton, spinButtonPortrait, isInteractable);
            }
        }
    }

    #endregion

    #region Bet Controls

    internal void UpdateBetDisplay()
    {
        if (gameManager.gameConfig == null) return;

        double totalPay = gameManager.GetTotalPay();

        if (betAmountText) betAmountText.text = FormatAmount(totalPay);
        if (betAmountTextPortrait) betAmountTextPortrait.text = "TOTAL PAY : " + FormatAmount(totalPay);

        UpdateBetButtonStates();
        UpdateGameRulesDynamicTexts();
    }

    private void UpdateBetButtonStates()
    {
        SetButtonInteractable(betMinusButton, betMinusButtonPortrait, true);
        SetButtonInteractable(betPlusButton, betPlusButtonPortrait, true);
    }

    #endregion

    #region Auto Play Panel

    public void OnSpinButtonHeld()
    {
        if (gameManager.currentState == GameState.Idle && !gameManager.isAutoPlaying)
        {
            AudioManager.Instance?.PlayButton();
            OpenAutoPlayPanel();
        }
    }

    private void OpenAutoPlayPanel()
    {
        AudioManager.Instance?.PlayAutoplayPanelOpen();
        if (isSettingsPanelOpen)
            CloseSettingsPanelImmediate();

        SetGameObjectActive(autoPlayPanel, autoPlayPanelPortrait, true);
        if (autoPlayPanelRect)
        {
            autoPlayPanelRect.anchoredPosition = new Vector2(autoPlayPanelRect.anchoredPosition.x, -600f);
            autoPlayPanelRect.DOAnchorPosY(0f, 0.35f).SetEase(Ease.OutCubic);
        }
        if (autoPlayPanelRectPortrait)
        {
            autoPlayPanelRectPortrait.anchoredPosition = new Vector2(autoPlayPanelRectPortrait.anchoredPosition.x, -600f);
            autoPlayPanelRectPortrait.DOAnchorPosY(0f, 0.35f).SetEase(Ease.OutCubic);
        }
    }

    private void CloseAutoPlayPanel()
    {
        AudioManager.Instance?.PlayPopupClose();

        if (autoPlayPanelRect)
        {
            autoPlayPanelRect.DOAnchorPosY(-600f, 0.35f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (autoPlayPanel) autoPlayPanel.SetActive(false);
            });
        }
        else
        {
            if (autoPlayPanel) autoPlayPanel.SetActive(false);
        }

        if (autoPlayPanelRectPortrait)
        {
            autoPlayPanelRectPortrait.DOAnchorPosY(-600f, 0.35f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (autoPlayPanelPortrait) autoPlayPanelPortrait.SetActive(false);
            });
        }
        else
        {
            if (autoPlayPanelPortrait) autoPlayPanelPortrait.SetActive(false);
        }
    }

    private void StartAutoplayWithRounds(int rounds)
    {
        CloseAutoPlayPanel();
        gameManager.StartAutoPlay(rounds);
    }

    internal void OnAutoPlayStarted()
    {
        UpdateAutoPlayCount();
        SetSpinStopButtonStates(isSpinningState: true, isInteractable: true);
        SetBetControlsEnabled(false);
    }

    internal void OnAutoPlayStopped()
    {
        SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, false);

        bool isRoundActive = gameManager.IsSpinning() || gameManager.lastResult != null;

        if (!isRoundActive)
        {
            SetSpinStopButtonStates(isSpinningState: false, isInteractable: true);
            SetBetControlsEnabled(true);
            SetButtonInteractable(settingsOpenButton, settingsOpenButtonPortrait, true);
        }
        else if (isRoundActive)
        {
            SetButtonActive(spinButton, spinButtonPortrait, false);
            SetButtonActive(stopButton, stopButtonPortrait, false);
            SetButtonActive(autoSpinStopButton, autoSpinStopButtonPortrait, true);
            SetButtonInteractable(autoSpinStopButton, autoSpinStopButtonPortrait, false);
        }
    }

    internal void UpdateAutoPlayCount()
    {
        string displayStr = "";
        if (gameManager.autoPlayTotalRounds == -1 || gameManager.autoPlayRemainingRounds < 0)
        {
            displayStr = "∞";
        }
        else
        {
            displayStr = $"{gameManager.autoPlayRemainingRounds}";
        }

        SetTMPText(autoSpinRemainingText, autoSpinRemainingTextPortrait, displayStr);
    }

    #endregion

    #region Spin Speed Universal Toggle Logic

    public void SetSpeedMode(SpinSpeed speed)
    {
        gameManager.SetSpinSpeed(speed);
        UpdateSpeedButtonsVisibility(speed);
    }

    private void UpdateSpeedButtonsVisibility(SpinSpeed speed)
    {
        SetButtonActive(normalSpeedButton, normalSpeedButtonPortrait, speed == SpinSpeed.Normal);
        SetButtonActive(turboSpeedButton, turboSpeedButtonPortrait, speed == SpinSpeed.Turbo);
        SetButtonActive(quickSpeedButton, quickSpeedButtonPortrait, speed == SpinSpeed.QuickSpin);
    }

    #endregion

    #region Sound Panel

    private void OpenSoundPanel()
    {
        AudioManager.Instance?.PlayPopupOpen();
        if (soundPanel == null) return;
        soundPanel.SetActive(true);
        if (soundPanelRect != null)
        {
            AnimatePopupOpen(soundPanelRect);
        }
        if (musicSlider && AudioManager.Instance != null)
        {
            musicSlider.value = AudioManager.Instance.MusicVolume;
        }
        if (sfxSlider && AudioManager.Instance != null)
        {
            sfxSlider.value = AudioManager.Instance.SfxVolume;
        }
    }

    private void CloseSoundPanel()
    {
        if (soundPanel == null || !soundPanel.activeSelf) return;
        if (soundPanelRect != null)
        {
            AnimatePopupClose(soundPanelRect, () =>
            {
                soundPanel.SetActive(false);
            });
        }
        else
        {
            AudioManager.Instance?.PlayPopupClose();
            soundPanel.SetActive(false);
        }
    }

    private void OnMusicSliderChanged(float val)
    {
        AudioManager.Instance?.SetMusicVolume(val);
    }

    private void OnSfxSliderChanged(float val)
    {
        AudioManager.Instance?.SetSfxVolume(val);
    }

    #endregion

    #region Settings Panel

    private void OpenSettingsPanel()
    {
        if ((autoPlayPanel && autoPlayPanel.activeSelf) || (autoPlayPanelPortrait && autoPlayPanelPortrait.activeSelf))
            CloseAutoPlayPanelImmediate();

        AudioManager.Instance?.PlayButton();
        isSettingsPanelOpen = true;

        SetButtonActive(settingsOpenButton, settingsOpenButtonPortrait, false);
        SetButtonActive(settingsCloseButton, settingsCloseButtonPortrait, true);
        SetButtonActive(settingsBgCloseButton, settingsBgCloseButtonPortrait, true);

        if (settingsPanel)
        {
            settingsPanel.SetActive(true);
            CanvasGroup cg = settingsPanel.GetComponent<CanvasGroup>();
            if (cg == null) cg = settingsPanel.AddComponent<CanvasGroup>();
            cg.DOKill();
            cg.DOFade(1f, 0.35f);
        }

        if (settingsPanelPortrait)
        {
            settingsPanelPortrait.SetActive(true);
            CanvasGroup cg = settingsPanelPortrait.GetComponent<CanvasGroup>();
            if (cg == null) cg = settingsPanelPortrait.AddComponent<CanvasGroup>();
            cg.DOKill();
            cg.DOFade(1f, 0.35f);
        }
    }

    private void CloseSettingsPanel()
    {
        AudioManager.Instance?.PlayButton();
        isSettingsPanelOpen = false;

        SetButtonActive(settingsOpenButton, settingsOpenButtonPortrait, true);
        SetButtonActive(settingsCloseButton, settingsCloseButtonPortrait, false);
        SetButtonActive(settingsBgCloseButton, settingsBgCloseButtonPortrait, false);

        if (settingsPanel)
        {
            CanvasGroup cg = settingsPanel.GetComponent<CanvasGroup>();
            if (cg == null) cg = settingsPanel.AddComponent<CanvasGroup>();
            cg.DOKill();
            cg.DOFade(0f, 0.35f).OnComplete(() =>
            {
                settingsPanel.SetActive(false);
            });
        }

        if (settingsPanelPortrait)
        {
            CanvasGroup cg = settingsPanelPortrait.GetComponent<CanvasGroup>();
            if (cg == null) cg = settingsPanelPortrait.AddComponent<CanvasGroup>();
            cg.DOKill();
            cg.DOFade(0f, 0.35f).OnComplete(() =>
            {
                settingsPanelPortrait.SetActive(false);
            });
        }
    }

    private void CloseSettingsPanelImmediate()
    {
        isSettingsPanelOpen = false;

        SetButtonActive(settingsOpenButton, settingsOpenButtonPortrait, true);
        SetButtonActive(settingsCloseButton, settingsCloseButtonPortrait, false);
        SetButtonActive(settingsBgCloseButton, settingsBgCloseButtonPortrait, false);

        if (settingsPanel)
        {
            CanvasGroup cg = settingsPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOKill();
                cg.alpha = 0f;
            }
            settingsPanel.SetActive(false);
        }

        if (settingsPanelPortrait)
        {
            CanvasGroup cg = settingsPanelPortrait.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOKill();
                cg.alpha = 0f;
            }
            settingsPanelPortrait.SetActive(false);
        }
    }

    private void CloseAutoPlayPanelImmediate()
    {
        if (autoPlayPanelRect) autoPlayPanelRect.localScale = Vector3.one;
        if (autoPlayPanelRectPortrait) autoPlayPanelRectPortrait.localScale = Vector3.one;
        SetGameObjectActive(autoPlayPanel, autoPlayPanelPortrait, false);
    }

    #endregion

    #region Game Rules Panel

    private void OpenGameRulesPanel()
    {
        if (isSettingsPanelOpen)
        {
            CloseSettingsPanelImmediate();
        }
        ShowGameRulesPanel();
    }

    private void ShowGameRulesPanel()
    {
        if (gameRulesPanel == null) return;
        AudioManager.Instance?.PlayButton();
        gameRulesPanel.SetActive(true);
        ResetPanelScrollToTop(gameRulesPanel);
    }

    private void CloseGameRulesPanel()
    {
        if (gameRulesPanel == null) return;
        AudioManager.Instance?.PlayButton();
        gameRulesPanel.SetActive(false);
    }

    #endregion

    #region Guide Panel

    private void OpenGuidePanel()
    {
        if (isSettingsPanelOpen)
        {
            CloseSettingsPanelImmediate();
        }
        ShowGuidePanel();
    }

    private void ShowGuidePanel()
    {
        if (guidePanel == null) return;
        AudioManager.Instance?.PlayButton();
        guidePanel.SetActive(true);
        ResetPanelScrollToTop(guidePanel);
    }

    private void CloseGuidePanel()
    {
        if (guidePanel == null) return;
        AudioManager.Instance?.PlayButton();
        guidePanel.SetActive(false);
    }

    private void ResetPanelScrollToTop(GameObject panel)
    {
        ScrollRect scrollRect = panel.GetComponentInChildren<ScrollRect>(true);
        if (scrollRect == null) return;

        scrollRect.StopMovement();
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
        StartCoroutine(ConfirmPanelScrollAtTop(scrollRect));
    }

    private IEnumerator ConfirmPanelScrollAtTop(ScrollRect scrollRect)
    {
        yield return null;
        if (scrollRect == null) yield break;

        Canvas.ForceUpdateCanvases();
        scrollRect.StopMovement();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    #endregion



    #region Expand / Shrink

    private void InitializeExpandShrink()
    {
        SetExpandShrinkButtons(isExpanded: false);
    }

    private void OnExpand()
    {
        isExpanded = true;
        jsFunctCalls?.RequestExpandGame();
        SetExpandShrinkButtons(isExpanded: true);
    }

    private void OnShrink()
    {
        isExpanded = false;
        jsFunctCalls?.RequestShrinkGame();
        SetExpandShrinkButtons(isExpanded: false);
    }

    private void SetExpandShrinkButtons(bool isExpanded)
    {
        SetButtonActive(expandButton, expandButtonPortrait, !isExpanded);
        SetButtonActive(shrinkButton, shrinkButtonPortrait, isExpanded);
    }

    private void RegisterFullscreenListener()
    {
        jsFunctCalls?.RegisterFullscreenListener(gameObject.name);
    }

    internal void OnFullscreenChanged(string isFullscreen)
    {
        bool newExpandedState = isFullscreen == "1";

        if (isExpanded != newExpandedState)
        {
            isExpanded = newExpandedState;
            SetExpandShrinkButtons(isExpanded);
        }
    }

    #endregion

    #region Popup Animations (Generic)

    private void AnimatePopupOpen(RectTransform popupRect)
    {
        if (!popupRect) return;
        popupRect.localScale = Vector3.zero;
        popupRect.DOScale(1.4f, 0.3f).SetEase(Ease.OutBack);
    }

    private void AnimatePopupClose(RectTransform popupRect, System.Action onComplete)
    {
        if (!popupRect) return;

        AudioManager.Instance?.PlayPopupClose();

        Sequence closeSeq = DOTween.Sequence();
        closeSeq.Append(popupRect.DOScale(1.5f, 0.1f));
        closeSeq.Append(popupRect.DOScale(0f, 0.2f).SetEase(Ease.InBack));
        closeSeq.OnComplete(() =>
        {
            popupRect.localScale = Vector3.one * 1.4f;
            onComplete?.Invoke();
        });
    }

    #endregion

    #region Display Updates

    internal void UpdatePingDisplay(int pingMs)
    {
        SetTMPText(pingText, pingTextPortrait, $"{pingMs} ms");
    }

    internal void UpdatePingDisplay(string content)
    {
        SetTMPText(pingText, pingTextPortrait, content);
    }

    internal void UpdateJackpotDisplay(JackpotValues values)
    {
        if (values == null) return;

        SetTMPText(grandJackpotText, grandJackpotTextPortrait, FormatJackpotValue(values.grandJackpot));
        SetTMPText(majorJackpotText, majorJackpotTextPortrait, FormatJackpotValue(values.majorJackpot));
        SetTMPText(minorJackpotText, minorJackpotTextPortrait, FormatJackpotValue(values.minorJackpot));
        SetTMPText(miniJackpotText, miniJackpotTextPortrait, FormatJackpotValue(values.miniJackpot));
    }

    private string FormatJackpotValue(string val)
    {
        if (string.IsNullOrEmpty(val)) return "$0.00";
        return val.StartsWith("$") ? val : "$" + val;
    }

    internal void UpdateBalanceDisplay()
    {
        SetTMPText(balanceText, balanceTextPortrait, "BALANCE : " + FormatAmount(gameManager.playerData.balance));
    }

    private void UpdateWinDisplay(double amount)
    {
        currentWinDisplayValue = amount;
        if (winAmountText) winAmountText.text = FormatAmount(amount);
        if (winAmountTextPortrait) winAmountTextPortrait.text = "WIN " + FormatAmount(amount);

        bool showWinText = amount > 0;

        if (showWinText)
        {
            SetGameObjectActive(goodLuckObject, goodLuckObjectPortrait, false);
            SetGameObjectActive(winTextObject, winTextObjectPortrait, true);
        }
        else
        {
            SetGameObjectActive(goodLuckObject, goodLuckObjectPortrait, true);
            SetGameObjectActive(winTextObject, winTextObjectPortrait, false);
        }
    }

    private void AnimateBalanceUpdate(double newBalance, double startBalance = -1f, float durationOverride = -1f)
    {
        if (balanceTween != null) balanceTween.Kill();
        SetTMPText(balanceText, balanceTextPortrait, "BALANCE : " + FormatAmount(newBalance));
    }

    private void AnimateWinUpdate(double targetWin, float duration = 0.8f)
    {
        if (winTween != null) winTween.Kill();
        UpdateWinDisplay(targetWin);
    }

    #endregion

    #region Helper Methods

    private string FormatAmount(double amount)
    {
        return amount.ToString("0.###");
    }

    private void SetBetControlsEnabled(bool enabled)
    {
        SetButtonInteractable(betPlusButton, betPlusButtonPortrait, enabled);
        SetButtonInteractable(betMinusButton, betMinusButtonPortrait, enabled);
    }

    #endregion

    #region Dynamic Game Rules Updates

    private void UpdateGameRulesDynamicTexts()
    {
        if (gameManager == null || gameManager.gameConfig == null) return;

        if (totalLineCountText != null)
        {
            totalLineCountText.text = gameManager.gameConfig.paylineCount.ToString();
        }

        SetRuleSymbolText(1, ruleRed7Text);
        SetRuleSymbolText(2, rulePurple7Text);
        SetRuleSymbolText(3, ruleBlue7Text);
        SetRuleSymbolText(4, ruleWhiteBar7Text);
        SetRuleSymbolText(5, ruleTripleBarText);
        SetRuleSymbolText(6, ruleDoubleBarText);

        ServerFeatures features = gameManager.gameConfig.features;
        SetRuleFeatureText(features?.anyBars, ruleAnyBarsText);
        SetRuleFeatureText(features?.barWhite7, ruleBarWhite7Text);
        SetRuleFeatureText(features?.mixedSevens, ruleMixedSevensText);
    }

    private void SetRuleSymbolText(int symbolId, TMP_Text textComponent)
    {
        if (textComponent == null || gameManager.gameConfig.symbols == null) return;
        var symbol = gameManager.gameConfig.symbols.Find(s => s.id == symbolId);
        if (symbol != null && symbol.multipliers != null && symbol.multipliers.Count > 0)
        {
            double value = symbol.isWild ? symbol.wildMultiplier : symbol.multipliers[0];
            textComponent.text = $"BET x {value.ToString("0.###")}";
        }
    }

    private void SetRuleFeatureText(SymbolCombinationFeature feature, TMP_Text textComponent)
    {
        if (textComponent != null && feature != null && feature.enabled)
        {
            textComponent.text = $"BET x {feature.payout.ToString("0.###")}";
        }
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        StopJackpotPortraitLevitation();
        if (balanceTween != null) balanceTween.Kill();
        if (winTween != null) winTween.Kill();
        DOTween.KillAll();
    }
    #endregion
    #region Jackpot Portrait Levitation Animation

    private void UpdateJackpotPortraitLevitationFromCurrentOrientation()
    {
        var oc = GetOrientationChange();
        if (oc != null)
        {
            UpdateJackpotPortraitLevitation(oc.CurrentMode);
        }
    }

    private void UpdateJackpotPortraitLevitation(OrientationChange.OrientationMode mode)
    {
        bool isPortrait = (mode == OrientationChange.OrientationMode.MobilePortrait || mode == OrientationChange.OrientationMode.DesktopPortrait);
        if (isPortrait)
        {
            StartJackpotPortraitLevitation();
        }
        else
        {
            StopJackpotPortraitLevitation();
        }
    }

    private List<Transform> GetJackpotPortraitTransforms()
    {
        List<Transform> list = new List<Transform>();

        Transform grandTr = grandJackpotPortraitParent != null ? grandJackpotPortraitParent : (grandJackpotTextPortrait != null ? grandJackpotTextPortrait.transform.parent : null);
        Transform majorTr = majorJackpotPortraitParent != null ? majorJackpotPortraitParent : (majorJackpotTextPortrait != null ? majorJackpotTextPortrait.transform.parent : null);
        Transform minorTr = minorJackpotPortraitParent != null ? minorJackpotPortraitParent : (minorJackpotTextPortrait != null ? minorJackpotTextPortrait.transform.parent : null);
        Transform miniTr = miniJackpotPortraitParent != null ? miniJackpotPortraitParent : (miniJackpotTextPortrait != null ? miniJackpotTextPortrait.transform.parent : null);

        if (grandTr != null) list.Add(grandTr);
        if (majorTr != null) list.Add(majorTr);
        if (minorTr != null) list.Add(minorTr);
        if (miniTr != null) list.Add(miniTr);

        return list;
    }

    private void StartJackpotPortraitLevitation()
    {
        if (!enableJackpotPortraitLevitation) return;

        StopJackpotPortraitLevitation();

        List<Transform> portraitJackpots = GetJackpotPortraitTransforms();
        if (portraitJackpots.Count == 0) return;

        for (int i = 0; i < portraitJackpots.Count; i++)
        {
            Transform tr = portraitJackpots[i];
            if (tr == null) continue;

            if (!jackpotInitialLocalPositions.ContainsKey(tr))
            {
                jackpotInitialLocalPositions[tr] = tr.localPosition;
            }

            Vector3 startPos = jackpotInitialLocalPositions[tr];
            tr.localPosition = startPos;

            float targetY = startPos.y + jackpotLevitateHeight;
            float delay = i * jackpotStaggerDelay;

            Tween posTween = tr.DOLocalMoveY(targetY, jackpotLevitateDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(delay);

            jackpotPortraitTweens.Add(posTween);
        }
    }

    private void StopJackpotPortraitLevitation()
    {
        for (int i = 0; i < jackpotPortraitTweens.Count; i++)
        {
            if (jackpotPortraitTweens[i] != null && jackpotPortraitTweens[i].IsActive())
            {
                jackpotPortraitTweens[i].Kill();
            }
        }
        jackpotPortraitTweens.Clear();

        foreach (var kvp in jackpotInitialLocalPositions)
        {
            if (kvp.Key != null)
            {
                DOTween.Kill(kvp.Key);
                kvp.Key.localPosition = kvp.Value;
            }
        }
    }

    #endregion



    #region Connection Popup Management

    private void OnExitButtonPressed()
    {
        if (popupManager != null)
        {
            popupManager.ShowExitGamePopup();
        }
        else if (gameManager != null)
        {
            gameManager.ExitGame();
        }
    }

    #endregion

}
