using UnityEngine;
using System.Collections;

public class InteractableObjects : MonoBehaviour {

	public enum OBJECT_TYPE{
		DRINKS,
		OTHERS
	}
		
	public float mass;

	public OBJECT_TYPE objectType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
