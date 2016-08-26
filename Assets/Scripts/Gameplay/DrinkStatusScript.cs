using UnityEngine;
using System.Collections;

public class DrinkStatusScript : MonoBehaviour {

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

	// Use this for initialization
	private void Start () {
      
	}
	
	// Update is called once per frame
	private void Update () {
	
	}

    public Drinks GetAlcoholType()
    {
        return _type;
    }

    public string GetAlcoholTypeToString()
    {
        return DrinkExt.DisplayName(_type);
    }

    public int GetAlcoholContent()
    {
        return AlcoholContent[(int)_type];
    }
}