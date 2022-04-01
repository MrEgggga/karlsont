using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DisableDOF : MonoBehaviour
{
    private DepthOfField dof;
    private VolumeProfile profile;
    private Settings settings;

    // Start is called before the first frame update
    void Start()
    {
        profile = GetComponent<Volume>().profile;
        profile.TryGet(out dof);

        this.settings = MenuAndSettings.instance.settings;
    }

    void Update()
    {
        bool dofEnabled = settings.GetSetting("DEPTH_OF_FIELD") == 1f;
        dof.active = dofEnabled;
    }
}
