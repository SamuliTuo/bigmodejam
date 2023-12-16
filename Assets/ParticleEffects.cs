using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Particles 
{ 
    NULL,
    CRATE_BREAK,
    TRASHCAN_BREAK,
}

public class ParticleEffects : MonoBehaviour
{
    public static ParticleEffects instance { get; private set; }
    private Dictionary<Particles, ParticleSystem> systems = new Dictionary<Particles, ParticleSystem>();
    private List<ParticleQueueObj> particleQueue = new List<ParticleQueueObj>();



    [SerializeField] private ParticleSystem crateBreak = null;
    [SerializeField] private ParticleSystem trashcanBreak = null;
    void Start()
    {
        systems.Add(Particles.CRATE_BREAK, crateBreak);
        systems.Add(Particles.TRASHCAN_BREAK, trashcanBreak);
    }

    void Awake()
    {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
    }


    void Update()
    {
        if (particleQueue.Count > 0)
        {
            var obj = particleQueue[0];
            particleQueue.RemoveAt(0);
            InitParticle(obj);
        }
    }



    public void PlayParticle(Particles p, Vector3 pos, Quaternion rot = new Quaternion())
    {
        particleQueue.Add(new ParticleQueueObj(p, pos, rot));
    }



    void InitParticle(ParticleQueueObj p)
    {
        if (p.type != Particles.NULL)
        {
            ParticleSystem system;
            systems.TryGetValue(p.type, out system);
            system.transform.position = p.pos;
            system.transform.rotation = p.rot;
            system.Play();
        }
    }

    class ParticleQueueObj
    {
        public Particles type;
        public Vector3 pos;
        public Quaternion rot;
        public ParticleQueueObj(Particles p, Vector3 pos, Quaternion rot)
        {
            type = p;
            this.pos = pos;
            this.rot = rot;
        }
    }
}
