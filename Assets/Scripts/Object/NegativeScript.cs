using UnityEngine;
using System.Collections;

public class NegativeScript : MonoBehaviour {
	
	public float healthPoint = 100;
	public int keyRed = 0;
	public int keyGreen = 0;
	public int keyBlue = 0;
	
	public bool activatePush = false;
	
	private Vector3 direction = Vector3.zero;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {	
		transform.position += direction * Time.deltaTime;
	}
	
	public void SetDirection(Vector3 dir)
	{
		direction = dir;
	}
	
	void OnCollisionEnter(Collision other)
	{
		
		
		if(other.collider.tag == "Item")
		{
			float damage = other.collider.gameObject.GetComponent<PositiveScript>().damageToOthers;
			healthPoint -= damage;
			
			Debug.Log("Test : " + healthPoint);
			if(healthPoint <= 0)
				Destroy(this.gameObject);
		}
	}
}
