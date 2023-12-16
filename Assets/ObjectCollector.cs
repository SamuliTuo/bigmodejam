using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCollector : MonoBehaviour
{
    public float collectSpeed = 1.0f;
    public float collectFlySpeed = 1.0f;

    [Space(20)]
    [SerializeField] [Range(0,1)] private float lootPosXOffset = 0;
    [SerializeField] [Range(0,1)] private float lootPosYOffset = 0;
    [SerializeField] private GameObject melonPrefab = null;
    [SerializeField] private Camera cam;

    private List<GameObject> melonDummies = new List<GameObject>();
    private Vector2 screenSize;

    

    private void Start()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        melonDummies.Clear();
        for (int i = 0; i < 10; i++)
        {
            var clone = Instantiate(melonPrefab);
            melonDummies.Add(clone);
            clone.SetActive(false);
        }
    }

    public void CollectMelon(MelonController melon)
    {
        StartCoroutine(CollectRoutine(melon.transform.GetChild(0).position, melon.value));
    }
    public void CollectMelon(Vector3 pos, int count)
    {
        StartCoroutine(CollectRoutine(pos, count));
    }

    IEnumerator CollectRoutine(Vector3 pos, int count)
    {
        int c = count;
        while (c > 0)
        {
            float t = 0;
            StartCoroutine(CollectSingle(pos));

            while (t < 1)
            {
                t += Time.deltaTime * collectSpeed;
                yield return null;
            }
            SaveGameManager.instance.AddMelon();
            c--;
            yield return null;
        }
    }

    IEnumerator CollectSingle(Vector3 position)
    {
        AudioManager.instance.PlayClip(audios.MELON, position);
        Transform melonDummy = GetInactiveMelon().transform;
        melonDummy.transform.position = position;
        melonDummy.gameObject.SetActive(true);
        float t = 0;

        while (t < 1)
        {
            Vector3 endPos = cam.ScreenToWorldPoint(new Vector3(screenSize.x * lootPosXOffset, screenSize.y * lootPosYOffset, 10));
            melonDummy.transform.position = Vector3.Lerp(position, endPos, t);
            t += Time.deltaTime * collectFlySpeed;
            yield return null;
        }
        melonDummy.gameObject.SetActive(false);
    }


    GameObject GetInactiveMelon()
    {
        // get a free melon
        foreach (var m in melonDummies) 
        {
            if (!m.activeSelf)
            {
                return m;
            }
        }
        // create if no free melons
        var clone = Instantiate(melonPrefab);
        melonDummies.Add(clone);
        return clone;
    }
}
