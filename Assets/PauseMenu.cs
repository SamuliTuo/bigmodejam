using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance { get; private set; }
    private void Awake() { if (instance != null && instance != this) Destroy(this); else instance = this; }

    public bool paused = false;


    private Transform pausePanel;

    private void Start()
    {
        pausePanel = transform.GetChild(1);
    }

    public void GamePaused()
    {
        paused = true;
        pausePanel.gameObject.SetActive(true);
    }

    public void GameUnPaused()
    {
        paused = false;
        pausePanel.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    public void OnBGMValueChanged(float newValue)
    {
        AudioManager.instance.changeBGMVolume(newValue);
    }
    public void OnSFXValueChanged(float newValue)
    {
        AudioManager.instance.ChangeSFXVolume(newValue);
    }
    ThirdPersonController plrControl = null;
    public void OnCamSpeedValueChanged(float newValue)
    {
        if (plrControl == null)
        {
            plrControl = GameObject.Find("Player").GetComponent<ThirdPersonController>();
        }
        plrControl?.SetCameraSpeed(newValue);
    }

    public void OnContinue()
    {
        if (plrControl == null)
        {
            plrControl = GameObject.Find("Player").GetComponent<ThirdPersonController>();
        }
        plrControl?.ActualUnpause();
    }
}
