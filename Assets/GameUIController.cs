using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Sprite sickoMode = null;
    [SerializeField] private Sprite urbanMode = null;

    private TextMeshProUGUI melonsCount;
    private Image modeUI;


    private void Awake()
    {
        melonsCount = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        modeUI = transform.GetChild(1).GetComponent<Image>();
    }
    
    public void SetMelonCount(int count)
    {
        melonsCount.text = count.ToString();
    }
    public void SetMode(PlayerModes mode)
    {
        if (mode == PlayerModes.NORMAL)
        {
            modeUI.sprite = sickoMode;
        }
        else if (mode == PlayerModes.URBAN)
        {
            modeUI.sprite = urbanMode;
        }
    }
}
