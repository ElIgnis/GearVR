using UnityEngine;
using System.Collections;

public class ModelAnimation : MonoBehaviour {

    Animation anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(anim.isPlaying == false)
        {
            anim.Play();
        }
	}

    void OnCollisionEnter()
    {
        anim.Play("SideGuy_Hit", PlayMode.StopAll);
    }
}
