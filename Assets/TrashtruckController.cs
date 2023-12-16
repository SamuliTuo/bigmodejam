using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class TrashtruckController : MonoBehaviour
{
    [SerializeField] private int hp = 3;
    [SerializeField] private GameObject trashcan = null;
    [SerializeField] private Transform trashThrowSpot1 = null;
    [SerializeField] private Transform trashThrowSpot2 = null;
    [SerializeField] private Transform trashThrowSpot3 = null;
    [SerializeField] private float interval = 1.0f;
    [SerializeField] private float throwSpeed = 10;
    [SerializeField] private float upFactor = 0.3f;
    [SerializeField] private audios spitSound = audios.None;
    [SerializeField] private audios dieSound = audios.None;
    public int barrelsBeforeDie = 0;
    private Animator anim;
    private Coroutine throwing = null;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
    public void TakeDamage()
    {
        print("damg");
        //Animator.playe(dmg);
        hp--;
        AudioManager.instance.PlayClip(audios.TRUCK_DIE, transform.position);
        if (hp == 0 && !dying)
        {
            StartCoroutine(Die());
        }
    }

    public void InitBossMode(float throwForce, float throwSpeed)
    {
        this.throwSpeed = throwForce;
        this.interval = throwSpeed;
        throwing = StartCoroutine(Trashthrow());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (throwing == null && other.CompareTag("Player"))
        {
            throwing = StartCoroutine(Trashthrow());
        }
    }

    IEnumerator Trashthrow()
    {
        float t = 0;
        var clone = Instantiate(trashcan, trashThrowSpot1.position, Quaternion.LookRotation(trashThrowSpot1.right));
        clone.GetComponent<TrashcanController>().GotThrown(trashThrowSpot1, throwSpeed, upFactor);
        AudioManager.instance.PlayClip(audios.TRUCK_SPIT, transform.position);
        anim.Play("left");
        CheckForDying();
        while (t < interval) 
        { 
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        var clone2 = Instantiate(trashcan, trashThrowSpot2.position, Quaternion.LookRotation(trashThrowSpot1.right));
        clone2.GetComponent<TrashcanController>().GotThrown(trashThrowSpot2, throwSpeed, upFactor);
        AudioManager.instance.PlayClip(audios.TRUCK_SPIT, transform.position);
        CheckForDying();
        anim.Play("right");
        while (t < interval)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        var clone3 = Instantiate(trashcan, trashThrowSpot3.position, Quaternion.LookRotation(trashThrowSpot3.right));
        clone3.GetComponent<TrashcanController>().GotThrown(trashThrowSpot3, throwSpeed, upFactor);
        AudioManager.instance.PlayClip(audios.TRUCK_SPIT, transform.position);
        CheckForDying();
        anim.Play("mid");
        while (t < interval)
        {
            t += Time.deltaTime;
            yield return null;
        }

        throwing = StartCoroutine(Trashthrow());
    }

    void CheckForDying()
    {
        if (barrelsBeforeDie > 0)
        {
            barrelsBeforeDie--;
            if (barrelsBeforeDie == 0)
            {
                if (!dying)
                    StartCoroutine(Die());
            }
        }
    }

    bool dying = false;
    IEnumerator Die()
    {
        dying = true;
        AudioManager.instance.PlayClip(audios.TRUCK_DIE, transform.position);
        StopCoroutine(throwing);
        anim.Play("die");

        float t = 0;
        while (t < 1.3f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
