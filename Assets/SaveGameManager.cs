using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager instance;
    public ZoneLevelScriptable zone01;
    public ZoneLevelScriptable zone02;
    public ZoneLevelScriptable zone03;

    public AudioClipPacket level01BGM;
    public AudioClipPacket level02BGM;
    public AudioClipPacket level03BGM;

    public AudioClipPacket zone01BGM;
    public AudioClipPacket zone02BGM;
    public AudioClipPacket zone03BGM;

    public ObjectCollector playerCollector;
    public int currentLevel = 1;
    public bool inZone = false;
    public Vector3 cpPos = Vector3.zero;
    public Quaternion cpRot = Quaternion.identity;

    private List<string> lvl1_melons = new List<string>();
    private List<string> lvl1_crates = new List<string>();

    private List<string> lvl2_melons = new List<string>();
    private List<string> lvl2_crates = new List<string>();

    private int melonCount = 0;
    private GameUIController gameUIController;
    private List<PlayerModes> unlockedModes = new List<PlayerModes>();

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
        DontDestroyOnLoad(this);
        unlockedModes.Add(PlayerModes.NORMAL);
    }

    // ===== CHECKPOINTS =====
    public void SetCurrentCheckpoint(Vector3 cpPos, Quaternion cpRot)
    {
        this.cpPos = cpPos;
        this.cpRot = cpRot;
    }

    // ===== PLAYER MODES =====
    public void AddPlayerMode(PlayerModes mode)
    {
        if (!unlockedModes.Contains(mode))
            unlockedModes.Add(mode);
    }
    public List<PlayerModes> GetUnlockedModes()
    {
        return unlockedModes;
    }

    // ===== MELONS ======
    public void IWasLooted_melon(string ID)
    {
        if (currentLevel == 1)
            lvl1_melons.Add(ID);
        else if (currentLevel == 2)
            lvl2_melons.Add(ID);
    }
    public bool WasILootedAlready_melon(string ID)
    {
        if (inZone)
            return false;

        if (currentLevel == 1)
        {
            if (lvl1_melons.Contains(ID))
            {
                return true;
            }
            return false;
        }
        else
        {
            if (lvl2_melons.Contains(ID))
            {
                return true;
            }
            return false;
        }
    }
    public void AddMelon()
    {
        if (gameUIController == null)
        {
            gameUIController = GameObject.Find("Canvas").GetComponentInChildren<GameUIController>();
        }
        melonCount++;
        gameUIController?.SetMelonCount(melonCount);
    }


    // ====== CRATES ======
    public void IWasBroken_box(string ID)
    {
        if (currentLevel == 1)
            lvl1_crates.Add(ID);
        else if (currentLevel == 2)
            lvl2_crates.Add(ID);
    }
    public bool WasIBrokenAlready(string ID)
    {
        if (currentLevel == 1)
        {
            if (lvl1_crates.Contains(ID))
            {
                return true;
            }
            return false;
        }
        else
        {
            if (lvl2_crates.Contains(ID))
            {
                return true;
            }
            return false;
        }
    }
    public ObjectCollector GetCollector()
    {
        if (playerCollector == null)
        {
            playerCollector = GameObject.Find("Player").GetComponent<ObjectCollector>();
        }
        return playerCollector;
    }

    public ZoneLevelScriptable GetCorrectZoneLevel()
    {
        if (currentLevel == 1)
            return zone01;

        else if (currentLevel == 2)
            return zone02;

        else return zone03;
    }
    public AudioClipPacket GetCorrectZoneBGM()
    {
        if (currentLevel == 1)
            return zone01BGM;

        else if (currentLevel == 2)
            return zone02BGM;

        else return zone03BGM;
    }
    public AudioClipPacket GetCorrectBGM()
    {
        if (currentLevel == 1)
            return level01BGM;

        else if (currentLevel == 2)
            return level02BGM;

        else return level03BGM;
    }
}
