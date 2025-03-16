using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class AudioClipSO : ScriptableObject
{
    public AudioClip[] footStep;
    public AudioClip[] trash;
    public AudioClip[] deliverySuccess;
    public AudioClip[] deliveryFail;
    public AudioClip[] objectDrop;
    public AudioClip[] objectPickUp;
    public AudioClip[] warning;
    public AudioClip[] chop;
    public AudioClip sizzleLoop;
}
