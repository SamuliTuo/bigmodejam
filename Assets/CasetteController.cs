using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasetteController : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 1.0f;
    [SerializeField] private float bobSpeed = 1.0f;
    [SerializeField] private float bobRange = 1.0f;
    [Space(10)]
    [SerializeField] private float teleporterMaxSize = 10;
    [SerializeField] private float teleporterGrowSpeed = 0.1f;
    [SerializeField] private Transform teleporter = null;
    [SerializeField] private Transform receiver = null;
    public Transform casetteBModel;

    private Vector3 startPos;
    private Transform casetteModel;
    private List<Vector3> teleporterPositions = new List<Vector3>();

    private void Start()
    {
        startPos = transform.position;
        casetteModel = transform.GetChild(0);
        teleporterPositions.Add(transform.position);
        if (receiver != null) 
            teleporterPositions.Add(receiver.position);
    }

    void Update()
    {
        float distance = Mathf.Sin(Time.timeSinceLevelLoad * bobSpeed);
        casetteModel.position = startPos + Vector3.up * distance * bobRange;
        casetteModel.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }


    Coroutine growRoutine = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && growRoutine == null)
        {
            casetteModel.gameObject.SetActive(false);
            casetteBModel.gameObject.SetActive(false);
            AudioManager.instance.ChangeBGM(SaveGameManager.instance.GetCorrectZoneBGM(), 5f);
            other.SendMessage("TeleporterPositions", teleporterPositions);
            StartCoroutine(TeleporterBigMode());
        }
    }

    IEnumerator TeleporterBigMode()
    {
        float t = 0;
        while (t < 1)
        {
            t += teleporterGrowSpeed * Time.deltaTime;

            teleporter.localScale = Vector3.Lerp(new Vector3(0.01f, 0.01f, 0.01f), new Vector3(teleporterMaxSize, teleporterMaxSize, teleporterMaxSize), t);
            yield return null;
        }
    }
}
