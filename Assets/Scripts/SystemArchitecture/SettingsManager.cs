using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class SettingsData
{
    public float ambientVolume = 1f;
    public float sfxVolume = 1f;
    public float dialogFontSize = 24f;
    public float brightness = 1f;
    public int sceneDuration = 0;
    public int controlMode = 0; // 0 = arrastrar dedo, 1 = giroscopio
    public float agitationThreshold = 10f;
}

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    public SettingsData CurrentSettings { get; private set; }
    private string SavePath => Path.Combine(Application.persistentDataPath, "settings.json"); //Guarda la configuración de los ajustes de la aplicación
    public string MetricsPath => Path.Combine(Application.persistentDataPath, "metrics"); //Guarda las métricas de juego recogidas durante la partida

    private readonly float[] sceneDurations = { 120f, 300f, 600f }; //Guarda las posibles duraciones de la escena

    public static event System.Action<float> OnFontSizeChanged;

    public static event System.Action<int> OnControlModeChanged;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!Directory.Exists(MetricsPath))
        {
            Directory.CreateDirectory(MetricsPath);
        }

        CurrentSettings = LoadSettings();
        ApplySettings();
    }

    
    #if UNITY_EDITOR //Esto sirve para crearlo en caso de que haciendo pruebas no se lance la aplicación desde el bootstrap, sino desde una escena del juego. No hace falta que el script esté asociado a ningun gameobject de la escena.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoInitialize()
        {
            if (Instance != null) return; // ya existe, no hace nada

            GameObject obj = new GameObject("SettingsManager_Auto");
            obj.AddComponent<SettingsManager>();
        }
    #endif
    

    private SettingsData LoadSettings()  //Si existe un JSON lo lee y devuelve sus valores, de lo contrario crea uno y devuelve valores default
    {
        if (!File.Exists(SavePath))
            return new SettingsData();
        try
        {
            return JsonUtility.FromJson<SettingsData>(File.ReadAllText(SavePath));
        }
        catch
        {
            Debug.LogWarning("Settings corruptos, cargando valores por defecto.");
            return new SettingsData();
        }
    }

    public void SaveSettings()
    {
        File.WriteAllText(SavePath, JsonUtility.ToJson(CurrentSettings, prettyPrint: true));
    }

    public void ApplySettings()
    {
        SetAmbientVolume(CurrentSettings.ambientVolume);
        SetSfxVolume(CurrentSettings.sfxVolume);
        SetBrightness(CurrentSettings.brightness);
    }

    public void SetAmbientVolume(float value)
    {
        Debug.Log($"Ambient volume set to {value}");
        CurrentSettings.ambientVolume = value;
        AudioManager.Instance?.SetAmbientVolume(value);
    }

    public void SetSfxVolume(float value)
    {
        CurrentSettings.sfxVolume = value;
        AudioManager.Instance?.SetSfxVolume(value);
    }

    public void SetDialogFontSize(float value)
    {
        CurrentSettings.dialogFontSize = value;
        OnFontSizeChanged?.Invoke(value);
    }

    public void SetBrightness(float value)
    {
        CurrentSettings.brightness = value;
        BrightnessOverlay.Instance?.SetBrightness(value);
    }

    public void SetAgitationThreshold(float value)
    {
        CurrentSettings.agitationThreshold = value;
    }

    public void SetSceneDuration(int index)
    {
        CurrentSettings.sceneDuration = index;
    }

    public float GetSceneDuration()
    {
        return sceneDurations[CurrentSettings.sceneDuration];
    }

    //Botones del giroscopio

    public void SetControlMode(int mode)
    {
        CurrentSettings.controlMode = mode;
        OnControlModeChanged?.Invoke(mode);
    }

    public void ToggleControlMode()
    {
        SetControlMode(CurrentSettings.controlMode == 0 ? 1 : 0);
    }
}
