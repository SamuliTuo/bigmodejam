using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DamageInstance : MonoBehaviour
{

    public void SetPool(IObjectPool<GameObject> pool) => this.pool = pool;

    private IObjectPool<GameObject> pool;
    private Collider[] hitCols;
    private List<Transform> affected;
    private float lifetime;
    private float dmg;
    private float t, radius;
    private bool hitsPlayer, hitsEnemies;

    void Awake()
    {
        affected = new List<Transform>();
    }

    public void Init(float lifetime, float dmg, float radius, bool hitsEnemies, bool hitsPlayer)
    {
        this.lifetime = lifetime;
        this.dmg = dmg;
        this.radius = radius;
        this.hitsPlayer = hitsPlayer;
        this.hitsEnemies = hitsEnemies;
        t = 0;
        StartCoroutine(UpdateDamageInstance());
    }

    IEnumerator UpdateDamageInstance()
    {
        affected.Clear();
        while (t < lifetime)
        {
            hitCols = Physics.OverlapSphere(transform.position, radius);
            for (int i = 0; i < hitCols.Length; i++)
            {
                if (hitCols[i].CompareTag("Breakable")) 
                {
                    hitCols[i].GetComponent<BreakableObject>().DestroyMe();
                }
                if (hitsEnemies) 
                {
                    if (hitCols[i].CompareTag("Enemy"))
                    {
                        if (!affected.Contains(hitCols[i].transform.root))
                        {
                            var cont = hitCols[i].transform.root.GetComponent<EnemyController_basic>();
                            if (cont != null)
                            {
                                cont.GotAttacked();
                                affected.Add(hitCols[i].transform.root);
                            }
                            else
                            {
                                hitCols[i].transform.root.GetComponent<TrashtruckController>().TakeDamage();
                                affected.Add(hitCols[i].transform.root);
                            }
                        }
                    }
                }
                if (hitsPlayer)
                {
                    if (hitCols[i].CompareTag("Player"))
                    {
                        if (!affected.Contains(hitCols[i].transform.root))
                        {
                            hitCols[i].transform.root.GetComponent<PlayerDamage>().TakeDamage();
                            affected.Add(hitCols[i].transform.root);
                        }
                    }
                }
            }

            t += Time.deltaTime;
            yield return null;
        }
        Deactivate();
    }

    void Deactivate()
    {
        if (pool != null)
        {
            pool.Release(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
