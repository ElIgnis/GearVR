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

public class PlayerActions : MonoBehaviour
{

    #region [ Private Vars ]
    GameObject _currentObject;
    Rigidbody _currentRB;
    InteractableObjects _interactableObject;
    float _grabTimer;
    bool _lookingAtGlassCup;
    bool _holdingDrink;
    bool _ableToDrink;

    #endregion

    #region [ Public Vars ]
    public Transform handTransform;
    public Transform jointTransform;
    public GameObject objectPoolParent;

    [Range(1, 100)]
    public int frameInterval;

    [Range(0, 5)]
    public float grabSpeed;

    [Range(0, 1)]
    public float headTiltLevel;

    [Range(0, 200)]
    public float handRotateSpeed;

    public float throwForce;
    public float throwAngle;

    public bool lookingAtGlassCup { get { return _lookingAtGlassCup; } }
    public InteractableObjects interactableObject { get { return _interactableObject; } }
    public GameObject currentObject { get { return _currentObject; } }

    public TextMesh debugText;
    public TextMesh _txtSrc1, _txtSrc2, _txtCompost, _txtCurrentAlcoholContent;

    
    #endregion

    #region FOR_MOVE
    public float speed;
    #endregion

    #region FOR_TOUCH

    public float distance = 0.1f;

    #endregion

    #region FOR_DEBUG

    //public TextMesh _txtSrc1, _txtSrc2, _txtCompost, _txtCurrentAlcoholContent;

    #endregion

    private Drinks _src1, _src2, _compost;
    private int _alcoholContent1, _alcoholContent2;

    private DrinkStatusScript _temp = null;

    public List<ChartData> _cd = new List<ChartData>();

    #region [ Public Methods ]
    public void GrabObject()
    {
        // If you don't have anything
        if (_currentObject == null)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);

            int touchableDistance = 1000;

            //Assign object as current object
            if (Physics.Raycast(ray, out hit, touchableDistance))
            {

                Debug.Log("aaaa");
                //Check if object is interactable
                if (hit.collider.gameObject.GetComponent<InteractableObjects>() != null)
                {
                    _currentObject = hit.collider.gameObject;
                    _currentObject.transform.rotation = Quaternion.identity;
                    _currentRB = _currentObject.GetComponent<Rigidbody>();
                    _currentRB.detectCollisions = false;
                    _currentRB.useGravity = false;
                    _interactableObject = _currentObject.GetComponent<InteractableObjects>();
                    MoveObjectTowardsPlayer();
                    StartCoroutine("CheckLookObject");
                    ///////////////////////////////////////////////
                    if (_temp = _currentObject.GetComponent<DrinkStatusScript>())
                        if (_src1 == Drinks.EMPTY)
                        {
                            _src1 = _temp.GetDrinkType();
                            _alcoholContent1 = _temp.GetAlcoholContent();
                        }

                    DisplaySelectedDrinks();
                    /////////////////////////////////////////////////
                }
            }
        }
        else
        {

        }

        
    }

    public void ThrowObject()
    {
        //Stop function if there is no object
        if (_currentObject == null)
            return;
        else
        {
            //Apply force and throw object in an arc
            Vector3 throwDirection = Quaternion.AngleAxis(throwAngle, gameObject.transform.right) * gameObject.transform.up;
            _currentRB.AddForce(throwDirection * throwForce);
            ReleaseObject();
        }
    }

    public void ReleaseObject()
    {
        _currentRB.detectCollisions = true;
        StopCoroutine("CheckLookObject");
        _lookingAtGlassCup = false;

        _currentObject.transform.parent = objectPoolParent.transform;
        _currentRB.useGravity = true;
        _currentObject = null;
        _currentRB = null;
    }

    public bool IsHoldingObject()
    {
        if (_currentObject == null)
            return false;
        else
            return true;
    }

    public void DrinkObject()
    {
        //Only can mix drinks
        if (_interactableObject != null && _interactableObject.objectType == InteractableObjects.OBJECT_TYPE.DRINKS)
        {

            //Drink if looking upwards
            if (gameObject.transform.forward.y >= headTiltLevel)
            {
                //Rotate bottle mouth towards player
                if (_holdingDrink)
                {
                    rotateArmToDrink(false);
                }
            }
            else
            {
                //Start drinking and modify alcohol level
                rotateArmToDrink(true);
            }
        }
    }

    public void MoveObjectTowardsPlayer()
    {
        _currentObject.transform.position = Vector3.Lerp(_currentObject.transform.position, handTransform.position, grabSpeed * Time.deltaTime);
        _currentObject.transform.parent = handTransform;
        _holdingDrink = true;
    }

    public void DeselectDrink()
    {
        if (_src2 != Drinks.EMPTY)
        {
            _src2 = Drinks.EMPTY;
            _compost = Drinks.EMPTY;
        }
        else if (_src1 != Drinks.EMPTY)
            _src1 = Drinks.EMPTY;
    }

    public Drinks MixDrinks(Drinks src1, Drinks src2)
    {

        if (src1 == src2)
            return src1;

        if (src1 == Drinks.WASTE || src2 == Drinks.WASTE)
            return Drinks.WASTE;

        Debug.Log("Before searching.");

        foreach (ChartData data in _cd)
        {
            Debug.Log("Start searching.");
            if (data._src1 == src1)
            {
                if (data._src2 == src2)
                {
                    return data._compost;
                }
            }

            if (data._src1 == src2)
            {
                if (data._src2 == src1)
                {
                    return data._compost;
                }
            }

            Debug.Log("Go to next data");
        }

        return Drinks.WASTE;
    }

    #endregion

    #region [ Private Methods ]
    

    IEnumerator CheckLookObject()
    {
        while (_currentObject != null)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);

            //Assign object as current object
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.gameObject.name == "Glass Cup")
                {
                    _lookingAtGlassCup = true;
                }
                else
                {
                    _lookingAtGlassCup = false;
                }
            }
            else
            {
                debugText.text = "Not looking at glass cup";
                _lookingAtGlassCup = false;
            }
            //Check at preferred frame interval
            yield return new WaitForSeconds(frameInterval * Time.deltaTime);
        }
        
    }

    void rotateArmToDrink(bool reverse)
    {
        if (reverse)
        {
            Debug.Log("reverse");
            jointTransform.rotation = Quaternion.RotateTowards(jointTransform.rotation, Camera.main.transform.rotation, handRotateSpeed * Time.deltaTime);
            _ableToDrink = false;
        }

        else
        {
            Debug.Log("changing");
            jointTransform.rotation = Quaternion.RotateTowards(jointTransform.rotation, Quaternion.AngleAxis(-135, Camera.main.transform.right), handRotateSpeed * Time.deltaTime);
            _ableToDrink = true;
        }
    }

    #endregion

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

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        //     DisplaySelectedDrinks();

        throwAngle = 90 - throwAngle;
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

        //#region TOUCH_METHOD

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit = new RaycastHit();

        //    if (Physics.Raycast(r, out hit, distance))
        //    {
        //        // Get object when you touch it
        //        GameObject obj = hit.collider.gameObject;

        //        #region GET_OBJ_ALCOHOL_TYPE_METHOD

        //        if (_temp = obj.GetComponent<DrinkStatusScript>())
        //            if (_src1 == Drinks.EMPTY){
        //                _src1 = _temp.GetDrinkType();
        //                _alcoholContent1 = _temp.GetAlcoholContent();}
        //            else if (_src2 == Drinks.EMPTY) { 
        //                _src2 = _temp.GetDrinkType();
        //                _alcoholContent2 = _temp.GetAlcoholContent();}

        //        #endregion
        //}}

        //#endregion

        #region CHECK_COMBINATION

        if (_src2 != Drinks.EMPTY)
            if (_src1 != Drinks.EMPTY)
            {
                Debug.Log("Let's mix");
                // If you forget this if statement, this game to become heavy
                if (_compost == Drinks.EMPTY)
                    _compost = MixDrinks(_src1, _src2);
            }

        #endregion

        //      DisplaySelectedDrinks();

    }


    #region Ryohei

    public void DisplaySelectedDrinks()
    {
        int sum = _alcoholContent1 + _alcoholContent2;
        _txtSrc1.text = "Src1 = " + DrinkExt.DisplayName(_src1);
        _txtSrc2.text = "Src2 = " + DrinkExt.DisplayName(_src2);
        _txtCompost.text = "Compost = " + DrinkExt.DisplayName(_compost);
        _txtCurrentAlcoholContent.text = "CAC = " + sum.ToString();
    }
    #endregion
}

static class DrinkExt
{
    public static string DisplayName(this Drinks drink)
    {

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

        return _names[(int)drink];
    }
};