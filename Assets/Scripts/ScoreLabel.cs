using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ScoreLabel : MonoBehaviour
{
    [System.Serializable]
    public struct FontThreshold
    {
        public TMP_FontAsset font;
        public int threshold;
    }

    public TextMeshProUGUI label;
    public FontThreshold[] fonts;
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        label.text = ""+score;
        label.font = fonts[0].font;
        foreach(FontThreshold ft in fonts)
        {
            if(score >= ft.threshold) label.font = ft.font;
            else break;
        }

        score += 1; // debugging purposes
    }
}
