using UnityEngine;
using System.Collections;

public class ModelAnimation : MonoBehaviour {

    Animation anim;
    public string animName;
    public bool playIdle;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () {
        if (playIdle && anim.isPlaying == false)
        {
            anim.Play();
        }
	}

    void OnCollisionEnter()
    {
        anim.Play(animName, PlayMode.StopAll);
    }
}
