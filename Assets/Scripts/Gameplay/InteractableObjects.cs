﻿using UnityEngine;
using System.Collections;

public class InteractableObjects : MonoBehaviour {

	public enum OBJECT_TYPE{
		DRINKS,
		OTHERS
	}

    public Drinks _type;

    private int[] AlcoholContent = { 
                             6,  // Beer
                             40, // Vodka 
                             50, // Whisky 
                             16, // Soju 
                             14, // Sake 
                             50, // Rum 
                             45, // Tequila 
                             5   // Moscato 
                           };

	public OBJECT_TYPE objectType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Drinks GetDrinkType()
    {
        return _type;
    }

    public string GetAlcoholTypeToString()
    {
        return DrinkExt.DisplayName(_type);
    }

    public int GetAlcoholContent()
    {
        // The 5 is first number of Alcohol in enum statement.

        // If it is any alcohols
        if ((int)_type >= 5 || (int)_type <= 12)
            return AlcoholContent[(int)_type - 5];

        // Non-alcohol
        return 0;
    }
}
