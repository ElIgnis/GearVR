﻿using UnityEngine;
using System.Collections;

	public enum OBJECT_TYPE{
		DRINKS,
        POTION,
        COCKTAIL,
		OTHERS
	}

    public enum GLASS_STATE
    {
        EMPTY,
        HALF,
        FULL
    }

public class InteractableObjects : MonoBehaviour {

	public OBJECT_TYPE objectType;
    public DRINK_TYPE drinkType;

    public GLASS_STATE currentState;
    public int currentAlcoholContent = 0;

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


	// Use this for initialization
	void Start () {
        if (this.GetObjectType() == OBJECT_TYPE.DRINKS)
            currentAlcoholContent = AlcoholContent[(int)drinkType - 5];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public GLASS_STATE GetGlassState()
    {
        return currentState;
    }
    public void SetGlassState(GLASS_STATE nextState)
    {
        currentState = nextState;
    }

    public DRINK_TYPE GetDrinkType()
    {
        if (objectType == OBJECT_TYPE.DRINKS)
            return drinkType;
        else
            return DRINK_TYPE.EMPTY;
    }

    public void SetDrinkType(DRINK_TYPE pouringThing)
    {
        drinkType = pouringThing;
    }

    public OBJECT_TYPE GetObjectType()
    {
        return objectType;
    }

    public string GetAlcoholTypeToString()
    {
        return DrinkExt.DisplayName(drinkType);
    }

    public int GetAlcoholContent()
    {
        // The 5 is first number of Alcohol in enum statement.

        // If it is any alcohols
        if ((int)drinkType >= 5 || (int)drinkType <= 12)
        {
            Debug.Log(AlcoholContent[(int)drinkType - 5]);
            return AlcoholContent[(int)drinkType - 5];
        }
        // Non-alcohol
        return 0;
    }
    public void AddAlcoholContent(DRINK_TYPE drink)
    {
        currentAlcoholContent += AlcoholContent[(int)drink - 5];
    }
}
