using UnityEngine;
using System.Collections;

public class ObjectCollisionSound : MonoBehaviour {

    public AudioClip soundToPlay;

    void OnCollisionEnter(Collision other)
    {
        //Sound 
        AudioSource.PlayClipAtPoint(soundToPlay, transform.position);
    }
}
