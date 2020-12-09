using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FrameRate : MonoBehaviour
{

    //private static FrameRate _FpsOlcer = null;
    int avgFrameRate;
    public TMPro.TextMeshProUGUI display_Text;
    public bool freezeFrameRate = true;
    // Use this for initialization
    void Awake()
    {
        if(freezeFrameRate)
        {
            Application.targetFrameRate = 300;
        }
    }
    public void Update()
    {
            float current = 0;
            current = (int)(1f / Time.unscaledDeltaTime);
            avgFrameRate = (int)current;
            display_Text.text = avgFrameRate.ToString() + " FPS";
    }
}

