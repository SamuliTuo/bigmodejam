using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MixerChannel { BGM, SFX }
[CreateAssetMenu(fileName = "AudioClipPacket", menuName = "ScriptableObjects/AudioClipPacket", order = 2)]
public class AudioClipPacket : ScriptableObject
{
    public List<AudioClip> clip = new List<AudioClip>();
    [Range(0, 1)] public float volume = 1;
    [Header("Normal pitch is 1")]
    [Range(-3, 2)] public float minPitch = 1;
    [Range(-3, 2)] public float maxPitch = 1;
    public MixerChannel channel = MixerChannel.SFX;
}
