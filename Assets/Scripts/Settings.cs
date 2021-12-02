using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [System.Serializable]
    public struct Setting {
        public string name;
        public float value;
        public Setting(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }

    private Dictionary<string, Setting> settings = new Dictionary<string, Setting>();
    public Setting[] startSettings;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Setting s in startSettings)
        {
            if(PlayerPrefs.HasKey(s.name))
            {
                settings.Add(s.name, new Setting(s.name, PlayerPrefs.GetFloat(s.name)));
            }
            else
            {
                settings.Add(s.name, s);
            }
        }
    }

    public float GetSetting(string name)
    {
        return settings[name].value;
    }

    public void SetSetting(string name, float value)
    {
        settings[name] = new Setting(name, value);
    }

    public void Save()
    {
        foreach(KeyValuePair<string, Setting> p in settings)
        {
            PlayerPrefs.SetFloat(p.Key, p.Value.value);
        }
        PlayerPrefs.Save();
    }
}
