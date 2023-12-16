using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrashcanController : MonoBehaviour
{
    [SerializeField] private float dotProdTreshold = -0.2f;
    [SerializeField] private PhysicMaterial frictionlessMat = null;

    private Coroutine rollingRoutine = null;
    private Rigidbody rb;
    private Collider col1, col2;
    private bool gotKicked = false;
    private bool rolling = false;
    private bool amIGonnaBreak = false;
    private Vector3 rollDirection;
    private Animator animator;
    private AnimatorClipInfo[] animatorinfo;
    private bool hitsEnemy, hitsPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        col1 = transform.GetChild(1).GetComponent<Collider>();
        col2 = transform.GetChild(1).GetChild(0).GetComponent<Collider>();
    }

    public void GetSpinnedOn()
    {
        animatorinfo = this.animator.GetCurrentAnimatorClipInfo(0);
        if (animatorinfo[0].clip.name != "hit")
            animator.Play("hit");
    }

    public void GetKicked(Transform kicker, float pushForce)
    {
        if (gotKicked)
            return;

        gotKicked = true;
        rolling = true;

        gameObject.layer = LayerMask.NameToLayer("TrashcanKICKED");
        transform.GetChild(1).gameObject.layer = LayerMask.NameToLayer("TrashcanKICKED");
        transform.GetChild(1).GetChild(0).gameObject.layer = LayerMask.NameToLayer("TrashcanKICKED");

        if (rollingRoutine != null) 
        { 
            StopCoroutine(rollingRoutine);
        }

        var rollDir = Random.Range(0, 2) == 0 ? kicker.right : -kicker.right;
        if (rollDir == -kicker.right)
            animator.Play("trashcanSpin_counterClockwise");
        else
            animator.Play("trashcanSpin_clockwise");
        transform.rotation = Quaternion.LookRotation(rollDir, Vector3.up);
        col1.material = col2.material = frictionlessMat;
        hitsEnemy = true;
        hitsPlayer = false;
        StartCoroutine(RollingRoutine(kicker.forward, pushForce, 0.45f));
    }

    public void GotThrown(Transform kicker, float speed, float upFactor)
    {
        Start();
        rolling = true;
        var rollDir = Random.Range(0, 2) == 0 ? kicker.right : -kicker.right;
        if (rollDir == -kicker.right)
            animator.Play("trashcanSpin_counterClockwise");
        else
            animator.Play("trashcanSpin_clockwise");
        transform.rotation = Quaternion.LookRotation(rollDir, Vector3.up);
        col1.material = col2.material = frictionlessMat;
        hitsPlayer = true;
        hitsEnemy = true;
        StartCoroutine(RollingRoutine(kicker.forward, speed, upFactor));
    }
    public void GotThrown(Vector3 position, Vector3 forward, Vector3 right, float speed, float upFactor)
    {
        Start();
        rolling = true;
        var rollDir = Random.Range(0, 2) == 0 ? right : -right;
        if (rollDir == -right)
            animator.Play("trashcanSpin_counterClockwise");
        else
            animator.Play("trashcanSpin_clockwise");
        transform.rotation = Quaternion.LookRotation(rollDir, Vector3.up);
        col1.material = col2.material = frictionlessMat;
        hitsPlayer = true;
        hitsEnemy = true;
        StartCoroutine(RollingRoutine(forward, speed, upFactor));
    }

    IEnumerator RollingRoutine(Vector3 pusherForward, float pushForce, float upFactor)
    {
        rb.velocity = Vector3.zero;
        rb.freezeRotation = true;
        rb.AddForce((pusherForward + Vector3.up * upFactor) * pushForce * rb.mass, ForceMode.Impulse);
        rollDirection = pusherForward;
        yield return null;
    }

    void OnCollisionEnter(Collision col)
    {
        if (rolling == false) 
            return;

        if (col.collider.CompareTag("BOSS") && gotKicked)
            col.transform.root.GetComponent<BossController>().TrashcanHit();

        ContactPoint[] contact = new ContactPoint[col.contactCount];
        int points = col.GetContacts(contact);

        if (points > 0) 
        {
            foreach (ContactPoint point in contact)
            {
                if (Vector3.Dot(rollDirection, point.normal) < dotProdTreshold)
                {
                    amIGonnaBreak |= true;
                }
            }
        }

        if (amIGonnaBreak)
        {
            if (rollingRoutine != null)
            {
                StopCoroutine(rollingRoutine);
            }
            DestroyMe();
        }
    }

    bool destroying = false;
    public void DestroyMe()
    {
        if (destroying) 
            return;
        destroying = true;

        DamageInstancePool.instance.SpawnDamageInstance_Sphere(transform.position, Quaternion.identity, 2f, 0.2f, 1, hitsEnemy, hitsPlayer);
        ParticleEffects.instance.PlayParticle(Particles.TRASHCAN_BREAK, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
