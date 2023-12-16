using UnityEngine;
using UnityEngine.Pool;

public class DamageInstancePool : MonoBehaviour
{
    public static DamageInstancePool instance;

    [SerializeField] private GameObject dmgInstanceSquare = null;
    [SerializeField] private GameObject dmgInstanceSphere = null;

    private IObjectPool<GameObject> dmgInstancePool;
    private GameObject clone;

    public void SpawnDamageInstance_Sphere(
        Vector3 position, Quaternion rotation, float radius,
        float lifetime, float dmg, bool hitsEnemy, bool hitsPlayer)
    {
        clone = dmgInstancePool.Get();
        clone.transform.position = position;
        clone.transform.localScale = new(radius, radius, radius);
        clone.transform.rotation = rotation;
        clone.GetComponent<DamageInstance>().Init(lifetime, dmg, radius, hitsEnemy, hitsPlayer);
        clone = null;
    }

    public void ClearPools()
    {
        dmgInstancePool.Clear();
    }

    void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
        dmgInstancePool = new ObjectPool<GameObject>(CreateDmgInstance, OnTakeDmgInstanceFromPool, OnReturnDmgInstanceToPool);
    }
    GameObject CreateDmgInstance()
    {
        var instance = Instantiate(dmgInstanceSphere) as GameObject;
        instance.GetComponent<DamageInstance>().SetPool(dmgInstancePool);
        return instance;
    }
    void OnTakeDmgInstanceFromPool(GameObject loot)
    {
        loot.SetActive(true);
    }
    void OnReturnDmgInstanceToPool(GameObject loot)
    {
        loot.SetActive(false);
    }
}
