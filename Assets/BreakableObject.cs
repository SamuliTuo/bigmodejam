using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviourID
{
    public int melonsInside = 3;

    private Transform box_noMelons, box_melons;
    private Rigidbody rb;
    private bool wasPushed = false;

    void Start()
    {
        box_noMelons = transform.GetChild(0);
        box_melons = transform.GetChild(1);
        rb = GetComponent<Rigidbody>();
        WasIBrokenAlready();
    }

    public void DestroyMe()
    {
        ParticleEffects.instance.PlayParticle(Particles.CRATE_BREAK, transform.position, Quaternion.identity);
        if (melonsInside > 0)
        {
            SaveGameManager.instance.GetCollector().CollectMelon(transform.position, melonsInside);
        }
        SaveGameManager.instance.IWasBroken_box(ID);
        Destroy(gameObject);
    }

    public void PushMe(Vector3 pusherForward, float pushForce)
    {
        if (wasPushed)
            return;
        wasPushed = true;

        //rb.AddForce(pusherForward * pushForce * rb.mass, ForceMode.Impulse);
        rb.AddForce((pusherForward + Vector3.up * 0.3f) * pushForce * rb.mass, ForceMode.Impulse);
        //StartCoroutine(Pushed(pusherForward, pushForce));
    }


    void WasIBrokenAlready()
    {
        if (melonsInside == 0 || SaveGameManager.instance.WasIBrokenAlready(ID))
        {
            box_melons.gameObject.SetActive(false);
            box_noMelons.gameObject.SetActive(true);
            melonsInside = 0;
        }
        //SaveGameManager.
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (wasPushed)
        {
            wasPushed = false;
            DestroyMe();
        }
    }
}
