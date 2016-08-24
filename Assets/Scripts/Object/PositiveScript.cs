using UnityEngine;
using System.Collections;

public class PositiveScript : MonoBehaviour {
	
	public int damageToOthers = 0;
	public int damageDistance = 10;

    [HideInInspector]
    public bool isSeen = false;

    private float elapsed = 0.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(isSeen)
        {
            elapsed += Time.deltaTime;
            if (elapsed > 1.0f)
            {
                isSeen = false;
                elapsed = 0.0f;

                this.transform.FindChild("glow_card").gameObject.SetActive(true);
                //this.transform.FindChild("red_glow").gameObject.SetActive(false);
            }                
            else
                return;
        }
        transform.position += new Vector3(0.0f, Mathf.Sin(Time.time) * 0.005f, 0.0f);
    }

    public void Targeted()
    {
        isSeen = true;

        this.transform.FindChild("glow_card").gameObject.SetActive(false);
        //this.transform.FindChild("red_glow").gameObject.SetActive(true);
    }

}
