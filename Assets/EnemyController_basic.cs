using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_basic : MonoBehaviour
{
    public bool dead;
    public bool pushed;
    public bool bossMode;
    public bool bossMode_director;

    [SerializeField] private int hp = 1;
    [SerializeField] private float timeBetweenMoves = 0.5f;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private bool useSmoothStep = false;
    [SerializeField] private bool useEaseOut = false;
    [SerializeField] private Animator animator = null;
    [SerializeField] private bool useMoveAnticipation = false;
    [SerializeField] private float anticipationTime = 0.0f;
    [SerializeField] private audios movesound = audios.None;
    [SerializeField] private audios stompedSound = audios.None;
    [SerializeField] private audios attackedSound = audios.None;
    [SerializeField] private audios moveAnticipationSound = audios.None;

    private Rigidbody rb;
    Transform model;
    public List<Vector3> targets;
    int currentTarget = -1;
    Coroutine moveRoutine = null;
    private Collider col, stompCol, kickedCol;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pushed = dead = false;
        model = transform.GetChild(0);
        animator = GetComponentInChildren<Animator>();
        kickedCol = GetComponent<Collider>();
        col = transform.GetChild(2).GetComponent<Collider>();
        stompCol = transform.GetChild(1).GetComponent<Collider>();

        if (bossMode_director)
        {
            moveRoutine = StartCoroutine(BossDirectorCoroutine());
            return;
        }
            
        var moveTargets = transform.Find("moveTargets");
        for (int i = 0; i < moveTargets.childCount; i++)
        {
            targets.Add(moveTargets.GetChild(i).position);
            moveTargets.GetChild(i).gameObject.SetActive(false);
        }
        if (targets.Count > 0)
        {
            targets.Add(transform.position);
            currentTarget = 0;
            if (bossMode)
                moveRoutine = StartCoroutine(BossMoveCoroutine());
            else
                moveRoutine = StartCoroutine(MoveCoroutine());
        }
    }

    IEnumerator MoveCoroutine()
    {
        Vector3 startpos = transform.position;
        Vector3 endpos = targets[currentTarget];
        Vector3 dir = endpos - startpos;
        float maxT = (startpos - endpos).magnitude;
        float t = 0;
        float perc;

        if (useMoveAnticipation)
        {
            AudioManager.instance.PlayClip(moveAnticipationSound, transform.position);
            animator.Play("attackAnticipation", 0, 0);
            Quaternion startrot = model.rotation;
            while (t < anticipationTime)
            {
                model.rotation = Quaternion.Slerp(startrot, Quaternion.LookRotation(dir, Vector3.up), t / anticipationTime);
                col.transform.rotation = stompCol.transform.rotation = Quaternion.Slerp(startrot, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z), Vector3.up), t / anticipationTime);
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;
        }
        
        animator.Play("move", 0, 0);
        AudioManager.instance.PlayClip(movesound, transform.position);
        while (t < maxT)
        {
            perc = t / maxT;
            if (useSmoothStep)
                perc = perc * perc * (3f - 2f * perc);
            else if (useEaseOut)
                perc = Mathf.Sin(perc * Mathf.PI * 0.5f);

            //Vector3 startRot = model.forward;
            //model.rotation = Quaternion.LookRotation(Vector3.RotateTowards(startRot, dir, Time.deltaTime, Time.deltaTime));
            model.rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.position = Vector3.Lerp(startpos, endpos, perc);
            col.transform.rotation = stompCol.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z), Vector3.up);
            t += Time.deltaTime * moveSpeed;
            yield return null;
        }

        if (timeBetweenMoves > 0)
        {
            animator.CrossFade("idle",0.2f,0,0);
            t = 0;
            while (t < timeBetweenMoves)
            {
                t += Time.deltaTime;
                yield return null;
            }
        }

        currentTarget++;
        if (currentTarget >= targets.Count)
        {
            currentTarget = 0;
        }

        moveRoutine = StartCoroutine(MoveCoroutine());
    }


    public void GotAttacked()
    {
        if (!dead)
        {
            dead = true;
            animator.Play("die", 0);
            AudioManager.instance.PlayClip(attackedSound, transform.position);
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }
            StartCoroutine(Die());
        }
    }
    public void GotStomped()
    {
        hp--;
        animator.Play("squash", 0);
        AudioManager.instance.PlayClip(stompedSound, transform.position);

        if (hp > 0)
        {
            return;
        }

        if (!dead)
        {
            dead = true;
            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
            }
            StartCoroutine(Die());
        }
    }

    public void GotKicked(Transform kicker, float pushforce)
    {
        if (pushed)
            return;

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        rb.isKinematic = false;
        rb.useGravity = true;
        stompCol.enabled = false;
        col.enabled = false;
        kickedCol.enabled = true;
        rb.AddForce((kicker.forward + Vector3.up * 0.3f) * pushforce * rb.mass, ForceMode.Impulse);
        animator.Play("idle");
        AudioManager.instance.PlayClip(stompedSound, transform.position);
        pushed = true;
    }

    public IEnumerator Die()
    {
        col.enabled = stompCol.enabled = kickedCol.enabled = false;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!dead && pushed && collision.collider != this.col && collision.collider != this.kickedCol && collision.collider != this.stompCol && !collision.collider.CompareTag("Player"))
        {
            AudioManager.instance.PlayClip(attackedSound, transform.position);
            DamageInstancePool.instance.SpawnDamageInstance_Sphere(transform.position, Quaternion.identity, 1.5f, 0.2f, 1, true, false);
            if (!dead)
            {
                StartCoroutine(Die());
            }
        }
    }





    IEnumerator BossMoveCoroutine()
    {
        Vector3 startpos = transform.position;
        Vector3 endpos = targets[currentTarget];
        Vector3 dir = endpos - startpos;
        float maxT = (startpos - endpos).magnitude;
        float t = 0;
        float perc;

        if (useMoveAnticipation)
        {
            animator.Play("attackAnticipation", 0, 0);
            Quaternion startrot = model.rotation;
            while (t < anticipationTime)
            {
                model.rotation = Quaternion.Slerp(startrot, Quaternion.LookRotation(dir, Vector3.up), t / anticipationTime);
                col.transform.rotation = stompCol.transform.rotation = Quaternion.Slerp(startrot, Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z), Vector3.up), t / anticipationTime);
                t += Time.deltaTime;
                yield return null;
            }
            t = 0;
        }

        animator.Play("move", 0, 0);
        AudioManager.instance.PlayClip(movesound, transform.position);
        while (t < maxT)
        {
            perc = t / maxT;
            if (useSmoothStep)
                perc = perc * perc * (3f - 2f * perc);
            else if (useEaseOut)
                perc = Mathf.Sin(perc * Mathf.PI * 0.5f);

            //Vector3 startRot = model.forward;
            //model.rotation = Quaternion.LookRotation(Vector3.RotateTowards(startRot, dir, Time.deltaTime, Time.deltaTime));
            model.rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.position = Vector3.Lerp(startpos, endpos, perc);
            col.transform.rotation = stompCol.transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z), Vector3.up);
            t += Time.deltaTime * moveSpeed;
            yield return null;
        }

        StartCoroutine(Die());
    }

    public GameObject walrusSpawn;
    IEnumerator BossDirectorCoroutine()
    {
        print("start director");
        Vector3 spawnPos = transform.position - transform.right * 2 - transform.up;
        float t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime;
            yield return null;
        }

        Instantiate(walrusSpawn, spawnPos, Quaternion.LookRotation(-transform.right, Vector3.up));
        moveRoutine =  StartCoroutine(BossDirectorCoroutine());
    }
}
