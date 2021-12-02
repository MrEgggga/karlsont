using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeableFOV : MonoBehaviour
{
    private Settings settings;
    public Camera cam;

    void Start()
    {
        GetSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if(settings != null)
        {
            cam.fieldOfView = settings.GetSetting("FIELD_OF_VIEW");
        }
    }

    void GetSettings()
    {
        settings = MenuAndSettings.instance.settings;
    }
}
