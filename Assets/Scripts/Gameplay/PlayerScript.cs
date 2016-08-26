using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Drinks
{
    EMPTY,

    // Waste
    WASTE,

    // Dorinks
    BLUE,
    GREEN,
    YELLOW,

    // Dorinks
    BEER,
    VODKA,
    WHISKY,
    SOJU,
    SAKE,
    RUM,
    TEQUILA,
    MOSCATO,

    // Specials
    GRAVITY,
    WATER,
    BUBBLE,
    LIFE,
    STRENGTH,
    GRASS,
    FIRE,
    GLITTER,
    GLOWING,
    SQUARE,
    RAINBOW,

    // Total
    DORINKS_MAX
};

public class ChartData
{
    public Drinks _src1;
    public Drinks _src2;

    public Drinks _compost;

    public ChartData(Drinks src1, Drinks src2, Drinks compost)
    {
        _src1 = src1;
        _src2 = src2;
        _compost = compost;
    }
};

public class PlayerScript : MonoBehaviour
{
    #region FOR_MOVE
    public float speed;
    #endregion

    #region FOR_TOUCH
    private Color touchColor;
    public float distance = 0.1f;


    #endregion


    public Text _txtSrc1, _txtSrc2, _txtCompost, _txtCurrentAlcoholContent;

    private Drinks _src1, _src2, _compost;
    private int _currentAlcoholContent;

    private DrinkStatusScript _temp = null;

    public List<ChartData> _cd = new List<ChartData>();

    void Start()
    {
        #region INIT_CHART_DATA

        _cd.Add(new ChartData(Drinks.BLUE, Drinks.BEER, Drinks.BEER));
        _cd.Add(new ChartData(Drinks.BLUE, Drinks.BEER, Drinks.BEER));
        _cd.Add(new ChartData(Drinks.BLUE, Drinks.VODKA, Drinks.GRAVITY));
        _cd.Add(new ChartData(Drinks.BLUE, Drinks.WHISKY, Drinks.WHISKY));
        _cd.Add(new ChartData(Drinks.BLUE, Drinks.SOJU, Drinks.WATER));
        _cd.Add(new ChartData(Drinks.BLUE, Drinks.SAKE, Drinks.BUBBLE));
        _cd.Add(new ChartData(Drinks.BLUE, Drinks.RUM, Drinks.RUM));
        _cd.Add(new ChartData(Drinks.BLUE, Drinks.TEQUILA, Drinks.TEQUILA));
        _cd.Add(new ChartData(Drinks.BLUE, Drinks.MOSCATO, Drinks.LIFE));

        _cd.Add(new ChartData(Drinks.GREEN, Drinks.BEER, Drinks.BEER));
        _cd.Add(new ChartData(Drinks.GREEN, Drinks.VODKA, Drinks.VODKA));
        _cd.Add(new ChartData(Drinks.GREEN, Drinks.WHISKY, Drinks.STRENGTH));
        _cd.Add(new ChartData(Drinks.GREEN, Drinks.SOJU, Drinks.GRASS));
        _cd.Add(new ChartData(Drinks.GREEN, Drinks.SAKE, Drinks.SAKE));
        _cd.Add(new ChartData(Drinks.GREEN, Drinks.RUM, Drinks.RUM));
        _cd.Add(new ChartData(Drinks.GREEN, Drinks.TEQUILA, Drinks.FIRE));
        _cd.Add(new ChartData(Drinks.GREEN, Drinks.MOSCATO, Drinks.MOSCATO));
 
        _cd.Add(new ChartData(Drinks.YELLOW, Drinks.BEER, Drinks.GLITTER));
        _cd.Add(new ChartData(Drinks.YELLOW, Drinks.VODKA, Drinks.VODKA));
        _cd.Add(new ChartData(Drinks.YELLOW, Drinks.WHISKY, Drinks.GLOWING));
        _cd.Add(new ChartData(Drinks.YELLOW, Drinks.SOJU, Drinks.SOJU));
        _cd.Add(new ChartData(Drinks.YELLOW, Drinks.SAKE, Drinks.SQUARE));
        _cd.Add(new ChartData(Drinks.YELLOW, Drinks.RUM, Drinks.RUM));
        _cd.Add(new ChartData(Drinks.YELLOW, Drinks.TEQUILA, Drinks.TEQUILA));
        _cd.Add(new ChartData(Drinks.YELLOW, Drinks.MOSCATO, Drinks.RAINBOW));

        #endregion

        speed = 5.0f;

        _src1 = _src2 = _compost = Drinks.EMPTY;

        _txtSrc1.text = "Src1 = Nothing";
        _txtSrc2.text = "Src2 = Nothing";
        _txtCompost.text = "Compost = Nothing";
        _txtCurrentAlcoholContent.text = "CAC = Nothing";
    }

    void Update()
    {
        #region MOVE_METHOD
        transform.Translate(speed * Input.GetAxis("Horizontal") * Time.deltaTime, 0f, speed * Input.GetAxis("Vertical") * Time.deltaTime);

        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("RyoheiTest");
        }
        #endregion

        #region TOUCH_METHOD

        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(r, out hit, distance))
            {
                // Get object when you touch it
                GameObject obj = hit.collider.gameObject;

                #region GET_OBJ_COLOR_METHOD
                touchColor = obj.GetComponent<Renderer>().material.color;
                Debug.Log(touchColor);
                #endregion

                #region GET_OBJ_ALCOHOL_TYPE_METHOD

                if (_temp = obj.GetComponent<DrinkStatusScript>())
                    if (_src1 == Drinks.EMPTY){
                        _src1 = _temp.GetAlcoholType();
                        _currentAlcoholContent += _temp.GetAlcoholContent();}
                    else if (_src2 == Drinks.EMPTY){
                        _src2 = _temp.GetAlcoholType();
                       _currentAlcoholContent += _temp.GetAlcoholContent();}

                #endregion

                DrinkStatusScript t = obj.GetComponent<DrinkStatusScript>();

                if (t != null)
                {
                    Debug.Log(_temp.GetAlcoholType());
                }
            }

            DisplaySelectedDrinks();
        }

        #endregion

        #region CHECK_COMBINATION

        if (_src2 != Drinks.EMPTY)
            if (_src1 != Drinks.EMPTY) {
                Debug.Log("Let's mix");
                _compost = MixDrinks(_src1, _src2);
                DisplaySelectedDrinks();}
        
            

        #endregion
        }

    public void DeselectDrink()
    {
        if (_src2 != Drinks.EMPTY){
            _src2 = Drinks.EMPTY;
            _compost = Drinks.EMPTY;}
        else if (_src1 != Drinks.EMPTY)
            _src1 = Drinks.EMPTY;


        DisplaySelectedDrinks();
    }

    public Drinks MixDrinks(Drinks src1, Drinks src2){
        _currentAlcoholContent = AddAlcoholContent(src1, src2);

        if (src1 == src2)
            return src1;

        if (src1 == Drinks.WASTE || src2 == Drinks.WASTE)
            return Drinks.WASTE;

        Debug.Log("Before searching.");

        foreach(ChartData data in _cd){
            Debug.Log("Start searching.");
            if (data._src1 == src1){
                if (data._src2 == src2){
                    return data._compost;}}

            if (data._src1 == src2){
                if (data._src2 == src1){
                    return data._compost;}}

            Debug.Log("Go to next data");}

        return Drinks.WASTE;
    }

    private int AddAlcoholContent(Drinks src1, Drinks src2)
    {
        _temp = GetComponent<DrinkStatusScript>();
        _temp.GetAlcoholContent();
        return 0;
    }

   public void DisplaySelectedDrinks() {
        _txtSrc1.text = "Src1 = " + DrinkExt.DisplayName(_src1);
        _txtSrc2.text = "Src2 = " + DrinkExt.DisplayName(_src2);
        _txtCompost.text = "Compost = " + DrinkExt.DisplayName(_compost);
        _txtCurrentAlcoholContent.text = "CAC = " + _currentAlcoholContent;
   }
}

static class DrinkExt{
    public static string DisplayName(this Drinks drink){

        #region INIT_ALCOHOL_NAMES  
        string[] _names = {   
                             "EMPTY",
                             "WASTE",
                             "BLUE",
                             "GREEN",
                             "YELLOW",
                             "BEER",
                             "VODKA",
                             "WHISKY",
                             "SOJU",
                             "SAKE",
                             "RUM",
                             "TEQUILA",
                             "MOSCATO",
                             "GRAVITY",
                             "WATER",
                             "BUBBLE",
                             "LIFE",
                             "STRENGTH",
                             "GRASS",
                             "FIRE",
                             "GLITTER",
                             "GLOWING",
                             "SQUARE",
                             "RAINBOW",
                             "DORINKS_MAX"
                         };
        #endregion

        return _names[(int)drink];}
};
