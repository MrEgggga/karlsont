using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSlider : MonoBehaviour
{
    public string settingName;
    public Settings settings;

    public void OnValueChanged(float value)
    {
        settings.SetSetting(settingName, value);
    }
}
