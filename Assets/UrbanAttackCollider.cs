using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrbanAttackCollider : MonoBehaviour
{
    private PlayerAttacks attScript;
    private List<GameObject> hitObjects = new List<GameObject>();
    private bool attacking = false;
    private Transform root;
    float pushForce;

    private void Start()
    {
        attScript = GetComponentInParent<PlayerAttacks>();
        root = transform.parent;
    }

    bool playedAudio = false;
    public void InitAttack(float pushForce)
    {
        playedAudio = false;
        this.pushForce = pushForce;
        attacking = true;
        hitObjects.Clear();
    }
    public void StopAttack()
    {
        attacking = false;
        hitObjects.Clear();
    }


    private void OnTriggerStay(Collider other)
    {
        if (!hitObjects.Contains(other.gameObject) && !other.CompareTag("Player"))
        {
            if (other.CompareTag("Breakable"))
            {
                PlayAudio();
                hitObjects.Add(other.gameObject);
                other.GetComponent<BreakableObject>().PushMe(transform.forward, pushForce);// root.position + Vector3.up * 0.7f, pushForce);
            }
            else if (other.CompareTag("Trashcan"))
            {
                PlayAudio();
                var obj = other.GetComponentInParent<TrashcanController>();
                obj.GetKicked(transform, pushForce); //root.position + Vector3.up * 0.7f, pushForce);
                hitObjects.Add(obj.gameObject);
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    hitObjects.Add(obj.transform.GetChild(i).gameObject);
                }
            }
            else if (other.CompareTag("Enemy"))
            {
                PlayAudio();
                var script = other.transform.root.GetComponent<EnemyController_basic>();
                script?.GotKicked(transform, pushForce);
                hitObjects.Add(script.gameObject);
                for (int i = 0; i < script.transform.childCount; i++)
                {
                    hitObjects.Add(script.transform.GetChild(i).gameObject);
                }
            }
            else if (other.CompareTag("BOSS"))
            {
                PlayAudio();
                var bossController = other.transform.root.GetComponent<BossController>();
                if (bossController != null)
                {
                    if (bossController.stunned)
                    {
                        bossController.TakeDamage();
                    }
                }
            }
        }
    }
    void PlayAudio()
    {
        if (!playedAudio)
        {
            playedAudio = true;
            AudioManager.instance.PlayClip(audios.PLAYER_KICK_IMPACT, transform.position);
        }
    }
}
