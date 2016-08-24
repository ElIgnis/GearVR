using UnityEngine;
using System.Collections;

public class GeneratorScript : MonoBehaviour {
	
	public GeneratorInfo[] objects = new GeneratorInfo[1];
	private int createCount = 0;
	private float elapsedTime = 0.0f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(createCount <= objects.Length)
			return;
		
		elapsedTime += Time.deltaTime;
		
		GeneratorInfo obj = objects[createCount];			
		if(elapsedTime >= obj.time)
		{
			GameObject instance = (GameObject) Instantiate(obj.typeOfObject, Vector3.zero, Quaternion.identity);
			Vector3 dir = instance.transform.position - obj.target.transform.position;
			instance.GetComponent<NegativeScript>().SetDirection(dir);
			
			createCount++;
			if(createCount >= objects.Length)
			{
				createCount = 0;
				elapsedTime = 0.0f;
			}
		}
	}
}
