using UnityEngine;
using System;

[System.Serializable]
public class GeneratorInfo
{
	public float time = 0.0f;
	public GameObject typeOfObject = null;
	public float speed = 0.0f;
	public Transform target = null;
	
	public GeneratorInfo ()
	{
	}
}

