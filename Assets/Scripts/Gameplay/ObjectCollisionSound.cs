using UnityEngine;
using System.Collections;

public class ObjectCollisionSound : MonoBehaviour {

    public AudioClip bottleSmashSound;

    void OnCollisionEnter(Collision other)
    {
        //Sound 
        AudioSource.PlayClipAtPoint(bottleSmashSound, transform.position);
    }
}
