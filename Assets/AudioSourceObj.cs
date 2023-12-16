using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AudioSourceObj : MonoBehaviour
{
    public void SetPool(IObjectPool<GameObject> pool) => this.pool = pool;

    private IObjectPool<GameObject> pool;
    private AudioClipPacket clipPack;
    private AudioSource source;


    public void InitAudioSourceObj(AudioClipPacket audioPack, Vector3 position)
    {
        transform.position = position;
        clipPack = audioPack;
        StartCoroutine(PlayClipOneShot());
    }

    IEnumerator PlayClipOneShot()
    {
        if (source == null)
        {
            source = GetComponent<AudioSource>();
        }

        AudioClip clip = clipPack.clip[0];
        if (clipPack.clip.Count > 1)
        {
            clip = clipPack.clip[Random.Range(0, clipPack.clip.Count)];
        }

        source.clip = clip;
        source.pitch = Random.Range(clipPack.minPitch, clipPack.maxPitch);
        source.volume = clipPack.volume;

        float t = clip.length + 0.5f;
        source.Play();
        
        while (t > 0) 
        {
            t -= Time.deltaTime;
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
