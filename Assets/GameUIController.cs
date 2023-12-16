using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{

    private TextMeshProUGUI melonsCount;


    private void Awake()
    {
        melonsCount = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    
    public void SetMelonCount(int count)
    {
        melonsCount.text = count.ToString();
    }
}
