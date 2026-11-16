using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;

public class UIInteraction : MonoBehaviour
{
    [Header("LogInPanel")]

    [Space(10)]

    [SerializeField] private GameObject logInPanel;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private Button quitButton;

    [Header("MainMenuPanel")]

    [Space(10)]

    [SerializeField] private GameObject menuPanel;

    [Space(10)]

    [SerializeField] private Button settingsButton;
    [SerializeField] private Button selectSceneButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button logOutButton;


    [Header("SettingsPanel")]

    [Space(10)]

    [SerializeField] private List<GameObject> settingsPanels;
    [SerializeField] private List<Button> settingsButtons;

    [Space(10)]

    [SerializeField] private GameObject settingsPanel;

    [Space(10)]

    [SerializeField] private Slider ambientVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Space(10)]

    [SerializeField] private Slider dialogFontSizeSlider;
    [SerializeField] private TMP_Text sampleText;

    [Space(10)]

    [SerializeField] private Slider brightnessSlider;

    [Space(10)]

    [SerializeField] private Button[] sceneDurationButtons;
    [SerializeField] private Sprite[] activeSprites;
    [SerializeField] private Sprite[] inactiveSprites;

    [Space(10)]

    [SerializeField] private TMP_Text metricsPathText;
    [SerializeField] private Button displayAllPathPanel;
    [SerializeField] private TMP_Text completePathText;
    [SerializeField] private Button showMetricsPathButton;
    [SerializeField] private Button saveMetricsButton;

    [Space(10)]

    [SerializeField] private TMP_InputField agitationThresholdInput;

    [Space(10)]

    [SerializeField] private Button settingsBackButton;

    [Header("SelectScenePanel")]

    [Space(10)]

    [SerializeField] private GameObject selectScenePanel;

    [Space(10)]

    [SerializeField] private Button selectSceneBackButton;

    [Space(10)]

    [SerializeField] private Button receptionButton;
    [SerializeField] private Button waitingRoomButton;
    [SerializeField] private Button consultationButton;

    [Header("ToggleImageButton")]

    [Space(10)]

    [SerializeField] private Button toggleImageButton;
    [SerializeField] private GameObject imageToToggle;

    private void Start()
    {
        BrightnessOverlay.Instance.FadeFromBlack(1f);

        LoadCurrentValues();
        SubscribeListeners();
        SetupSceneDurationButtons();
        ShowMetricsPath();

        agitationThresholdInput.contentType = TMP_InputField.ContentType.DecimalNumber;

        imageToToggle.SetActive(false);
    }

    private void LoadCurrentValues()
    {
        SettingsData s = SettingsManager.Instance.CurrentSettings;

        //Cargar los valores del settings data almacenado
        ambientVolumeSlider.value = s.ambientVolume;
        sfxVolumeSlider.value = s.sfxVolume;
        dialogFontSizeSlider.value = s.dialogFontSize;
        sampleText.fontSize = s.dialogFontSize;
        brightnessSlider.value = s.brightness;
        UpdateSceneDurationButtons(s.sceneDuration);

        //Al volver al menu principal, si hay un JSON de datos creados no aparecerá de nuevo el panel de log in
        if (MetricsManager.Instance.Current != null)
        {
            logInPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
        errorText.gameObject.SetActive(false);

        agitationThresholdInput.text = SettingsManager.Instance.CurrentSettings.agitationThreshold.ToString();
    }

    private void SubscribeListeners()
    {
        ambientVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetAmbientVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSfxVolume);
        brightnessSlider.onValueChanged.AddListener(SettingsManager.Instance.SetBrightness);
        dialogFontSizeSlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SetDialogFontSize(value);
            sampleText.fontSize = value;
        });

        agitationThresholdInput.onEndEdit.AddListener(value => // Validar que el valor introducido es un número válido, esta función cambia el umbral con el que se registra el estres a través del movimiento del giroscopio
        {
            if (float.TryParse(value, out float result))
                SettingsManager.Instance.SetAgitationThreshold(result);
            else
                agitationThresholdInput.text = SettingsManager.Instance.CurrentSettings.agitationThreshold.ToString();
        });

        settingsBackButton.onClick.AddListener(OnSettingsBackButton);
        settingsButton.onClick.AddListener(OnSettingsButton);
        selectSceneButton.onClick.AddListener(OnSelectSceneButton);
        selectSceneBackButton.onClick.AddListener(OnSelectSceneBackButton);
        receptionButton.onClick.AddListener(OnReceptionButton);
        waitingRoomButton.onClick.AddListener(OnWaitingRoomButton);
        consultationButton.onClick.AddListener(OnConsultationButton);
        playButton.onClick.AddListener(OnPlayButton);
        quitButton.onClick.AddListener(OnQuitButton);
        saveMetricsButton.onClick.AddListener(OnExportButton);
        logOutButton.onClick.AddListener(OnLogOutButton);
        showMetricsPathButton.onClick.AddListener(ShowMetricsButton);
        displayAllPathPanel.onClick.AddListener(HideMetricsButton);
        acceptButton.onClick.AddListener(OnAcceptButton);
        toggleImageButton.onClick.AddListener(OnToggleImageButton);
    }

    private void SetupSceneDurationButtons()
    {
        for (int i = 0; i < sceneDurationButtons.Length; i++)
        {
            int index = i;
            sceneDurationButtons[i].onClick.AddListener(() =>
            {
                SettingsManager.Instance.SetSceneDuration(index);
                UpdateSceneDurationButtons(index);
            });
        }
    }

    private void ShowMetricsPath() //Se llama en el start
    {
        string path = SettingsManager.Instance.MetricsPath;
        metricsPathText.text = path;
        completePathText.text = path;
        displayAllPathPanel.gameObject.SetActive(false);
    }

    private void UpdateSceneDurationButtons(int activeIndex)
    {
        for (int i = 0; i < sceneDurationButtons.Length; i++)
        {
            Image buttonImage = sceneDurationButtons[i].GetComponent<Image>();
            buttonImage.sprite = (i == activeIndex) ? activeSprites[i] : inactiveSprites[i];
        }
    }

    private void OnSettingsButton()
    {
        menuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    private void OnSettingsBackButton()
    {
        SettingsManager.Instance.SaveSettings();
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    private void OnSelectSceneButton()
    {
        menuPanel.SetActive(false);
        selectScenePanel.SetActive(true);
    }

    private void OnSelectSceneBackButton()
    {
        menuPanel.SetActive(true);
        selectScenePanel.SetActive(false);
    }

    private void OnReceptionButton()
    {
        MetricsManager.Instance.StartGameSession("Reception");
        BrightnessOverlay.Instance.FadeToBlack(1f, () =>
        {
            SceneManager.LoadScene("Reception");
        });
    }

    private void OnWaitingRoomButton()
    {
        MetricsManager.Instance.StartGameSession("WaitingRoom");
        BrightnessOverlay.Instance.FadeToBlack(1f, () =>
        {
            SceneManager.LoadScene("WaitingRoom");
        });
    }

    private void OnConsultationButton()
    {
        MetricsManager.Instance.StartGameSession("Consultation");
        BrightnessOverlay.Instance.FadeToBlack(1f, () =>
        {
            SceneManager.LoadScene("Consultation");
        });
    }

    private void OnLogOutButton()
    {
        menuPanel.SetActive(false);
        logInPanel.SetActive(true);
    }

    private void OnPlayButton()
    {
        MetricsManager.Instance.StartGameSession("Reception");
        BrightnessOverlay.Instance.FadeToBlack(1f, () =>
        {
            SceneManager.LoadScene("Reception");
        });
    }

    private void OnQuitButton()
    {
        Application.Quit();
    }

    private void OnAcceptButton()
    {
        string pseudonym = usernameInput.text.Trim();

        if (string.IsNullOrEmpty(pseudonym))
        {
            errorText.text = "INTRODUCE UN PSEUDONIMO PARA CONTINUAR.";
            errorText.gameObject.SetActive(true);
            return;
        }

        MetricsManager.Instance.StartSession(pseudonym);
        logInPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void OnExportButton()
    {
        string path = MetricsManager.Instance.MetricsFilePath;

        if (!File.Exists(path))
        {
            Debug.LogWarning("No hay métricas guardadas todavía.");
            return;
        }

        new NativeShare()
            .SetSubject("Métricas del juego")
            .AddFile(path)
            .Share();
    }

    private void ShowMetricsButton()
    {
        displayAllPathPanel.gameObject.SetActive(true);
    }

    private void HideMetricsButton()
    {
        displayAllPathPanel.gameObject.SetActive(false);
    }

    private void OnToggleImageButton()
    {
        imageToToggle.SetActive(!imageToToggle.activeSelf);
    }

    public void ChangeSettingsPanel(int option)
    {
        if (option < 0 || option >= settingsPanels.Count)
            return;

        for (int i = 0; i < settingsPanels.Count; i++)
        {
            settingsPanels[i].SetActive(i == option);
            settingsButtons[i].interactable = (i != option);
        }
    }
}
