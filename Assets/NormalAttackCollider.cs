using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackCollider : MonoBehaviour
{
    private PlayerAttacks attScript;
    private List<GameObject> hitObjects = new List<GameObject>();
    private bool attacking = false;

    private void Start()
    {
        attScript = GetComponentInParent<PlayerAttacks>();
    }

    public void InitAttack()
    {
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
            hitObjects.Add(other.gameObject);

            if (other.CompareTag("Breakable"))
            {
                other.GetComponent<BreakableObject>()?.DestroyMe();
            }

            else if (other.CompareTag("Enemy"))
            {
                other.GetComponentInParent<EnemyController_basic>()?.GotAttacked();
            }

            else if (other.CompareTag("Trashcan"))
            {
                other.GetComponentInParent<TrashcanController>()?.GetSpinnedOn();
            }

            else if (other.CompareTag("BOSS"))
            {
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
}
