using UnityEngine;
using UnityEngine.UI;

public class Preferences : MonoBehaviour
{
    public static Preferences Instance { get; private set; }
    public Scrollbar CarFrequencyScrollbar;
    public Scrollbar RefillingTimeScrollbar;

    public float CarFrequency
    {
        get { return PlayerPrefs.GetFloat(PrefKey.CarFrequency); }
        set { PlayerPrefs.SetFloat(PrefKey.CarFrequency, value); }
    }

    public float RefillingTime
    {
        get { return PlayerPrefs.GetFloat(PrefKey.RefuelingTime); }
        set { PlayerPrefs.SetFloat(PrefKey.RefuelingTime, value); }
    }

    private void Awake()
    {
        Instance = this;
        if (!PlayerPrefs.HasKey(PrefKey.CarFrequency))
            CarFrequency = Constants.MaxCarFrequencyGen;
        if (!PlayerPrefs.HasKey(PrefKey.RefuelingTime))
            RefillingTime = Constants.MaxRefuelingTime;
        CarFrequencyScrollbar.value = CarFrequency;
        RefillingTimeScrollbar.value = RefillingTime;
    }
}