using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum DRINK_TYPE
{
    /*
     
     If you wanna add new drink, please add to the last
     
     */

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

enum ARM_MOTION
{
    DRINKING,
    POURING
}
public class ChartData
{
    public DRINK_TYPE _src1;
    public DRINK_TYPE _src2;

    public DRINK_TYPE _compost;

    public ChartData(DRINK_TYPE src1, DRINK_TYPE src2, DRINK_TYPE compost)
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
    InteractableObjects _currentLookingGlassCup;
    bool _holdingDrink;
    bool _ableToDrink;

    #endregion

    #region [ Public Vars ]
    public Transform handTransform;
    public Transform jointTransform;
    public GameObject humanModel;
    public GameObject objectPoolParent;

    public AudioClip pourSound;
    public AudioClip bottleDropSound;
    public AudioClip bubbleSound;
    public AudioClip throwSound;

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

    #region FOR_DEBUG
    public TextMesh debugText;
    public Text _txtSrc1, _txtSrc2, _txtCompost, _txtCurrentAlcoholContent, _txtAlcoholContentOfLookingThing;
    #endregion

    #endregion

    #region FOR_MOVE
    public float speed;
    private Vector3 forward;
    private Vector3 right;
    private Vector3 moveDirection;
    #endregion

    #region FOR_TOUCH

    public float distance = 0.1f;

    #endregion


    private DRINK_TYPE _src1, _src2, _compost;
    private int _alcoholContent1, _alcoholContent2;

    private InteractableObjects _temp = null;

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
                //Check if object is interactable
                if (hit.collider.gameObject.GetComponent<InteractableObjects>() != null && hit.collider.gameObject.tag != "Cocktail")
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
                    if (_temp = _currentObject.GetComponent<InteractableObjects>())
                        if (_src1 != DRINK_TYPE.EMPTY)
                        {
                            _src1 = _temp.GetDrinkType();
                            _alcoholContent1 = _temp.GetAlcoholContent();
                        }

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
            //Sound 
            AudioSource.PlayClipAtPoint(throwSound, transform.position);

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

        DeselectDrink();
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
        if (_interactableObject != null && _interactableObject.objectType == OBJECT_TYPE.DRINKS)
        {

            //Drink if looking upwards
            if (gameObject.transform.forward.y >= headTiltLevel)
            {
                //Rotate bottle mouth towards player
                if (_holdingDrink)
                {
                    rotateArmToDrink(ARM_MOTION.DRINKING, false);
                }
            }
            else
            {
                //Start drinking and modify alcohol level
                rotateArmToDrink(ARM_MOTION.DRINKING, true);
            }
        }
    }

    public void MoveObjectTowardsPlayer()
    {
        _currentObject.transform.position = Vector3.Lerp(_currentObject.transform.position, handTransform.position, grabSpeed * Time.deltaTime);
        _currentObject.transform.parent = handTransform;
        _holdingDrink = true;
        _currentObject.transform.forward = Camera.main.transform.forward;
    }

    public void DeselectDrink()
    {
        // Ver. If you can select one drink only.
        if (_src1 != DRINK_TYPE.EMPTY)
            _src1 = DRINK_TYPE.EMPTY;

        _alcoholContent1 = 0;
        // Ver. If you can select two drinks.
        //if (_src2 != DRINK_TYPE.EMPTY)
        //{
        //    _src2 = DRINK_TYPE.EMPTY;
        //    _compost = DRINK_TYPE.EMPTY;
        //}
        //else if (_src1 != DRINK_TYPE.EMPTY)
        //    _src1 = DRINK_TYPE.EMPTY;
    }

    public void PourCurrentDrink()
    {
        // Animetion
        rotateArmToDrink(ARM_MOTION.DRINKING, true);

        InteractableObjects currentObject = _currentObject.gameObject.GetComponent<InteractableObjects>();

        //Only can pour drinks
        if (currentObject.GetObjectType() == OBJECT_TYPE.DRINKS)
        {

            // Current I have thing.
            DRINK_TYPE src1 = currentObject.GetDrinkType();
            // Current I look thing.
            DRINK_TYPE src2 = _currentLookingGlassCup.GetDrinkType();

            switch (_currentLookingGlassCup.GetGlassState())
            {
                case GLASS_STATE.EMPTY:

                    //Sound 
                    AudioSource.PlayClipAtPoint(pourSound, transform.position);

                    // If I look cup is empty, I just pour.
                    _currentLookingGlassCup.SetDrinkType(src1);

                    _currentLookingGlassCup.AddAlcoholContent(src1);

                    _currentLookingGlassCup.SetGlassState(GLASS_STATE.HALF);

                    Debug.Log("Glass is " + currentObject.GetDrinkType());
                    Debug.Log("Glass is " + currentObject.GetGlassState());
                    Debug.Log("This AC is " + currentObject.GetAlcoholContent());
                    Debug.Log("POURING");
                    break;

                case GLASS_STATE.HALF:

                    //Sound 
                    AudioSource.PlayClipAtPoint(pourSound, transform.position);

                    // I pour the same thing
                    if (src1 == src2)
                    {
                        _currentLookingGlassCup.SetDrinkType(src1);
                        _currentLookingGlassCup.AddAlcoholContent(src1);
                    }


                    // I pour the waste. or  I pour any thing to the waste.
                    if (src1 == DRINK_TYPE.WASTE || src2 == DRINK_TYPE.WASTE)
                    {
                        _currentLookingGlassCup.SetDrinkType(DRINK_TYPE.WASTE);
                        _currentLookingGlassCup.AddAlcoholContent(src1);
                    }


                    // I investigate combination when I pour any drink to other drink.
                    foreach (ChartData data in _cd)
                    {
                        if (data._src1 == src1)
                        {
                            if (data._src2 == src2)
                            {
                                _currentLookingGlassCup.SetDrinkType(data._compost);
                                _currentLookingGlassCup.AddAlcoholContent(src1);
                            }
                        }

                        if (data._src1 == src2)
                        {
                            if (data._src2 == src1)
                            {
                                _currentLookingGlassCup.SetDrinkType(data._compost);
                                _currentLookingGlassCup.AddAlcoholContent(src1);
                            }
                        }

                    }

                    _currentLookingGlassCup.SetGlassState(GLASS_STATE.FULL);
                    Debug.Log("Glass is " + _currentLookingGlassCup.GetDrinkType());
                    Debug.Log("Glass is " + _currentLookingGlassCup.GetGlassState());
                    Debug.Log("This AC is " + currentObject.GetAlcoholContent());

                    Debug.Log("POURING");
                    break;

                case GLASS_STATE.FULL:
                    // Do nothing
                    break;

                default:
                    // This combination wasn't in chart data.
                    _currentLookingGlassCup.SetDrinkType(DRINK_TYPE.WASTE);
                    break;
            }
        }
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

                if (hit.collider.gameObject.tag == "Cocktail")
                {
                    Debug.Log("glass of" + hit.collider.gameObject.GetComponent<InteractableObjects>().GetDrinkType());
                    _lookingAtGlassCup = true;
                    _currentLookingGlassCup = hit.collider.gameObject.GetComponent<InteractableObjects>();

                    if (IsHoldingObject())
                    {
                        _src2 = _currentLookingGlassCup.GetDrinkType();
                    }
                }
                else
                {
                    _lookingAtGlassCup = false;
                }
            }
            else
            {
                //                debugText.text = "Not looking at glass cup";
                _lookingAtGlassCup = false;
            }
            //Check at preferred frame interval
            yield return new WaitForSeconds(frameInterval * Time.deltaTime);
        }

    }

    void rotateArmToDrink(ARM_MOTION type, bool reverse)
    {
        switch (type)
        {
            case ARM_MOTION.DRINKING:
                if (reverse)
                {
                    jointTransform.rotation = Quaternion.RotateTowards(jointTransform.rotation, Camera.main.transform.rotation, handRotateSpeed * Time.deltaTime);
                    _ableToDrink = false;
                }
                else
                {
                    jointTransform.localEulerAngles = Vector3.MoveTowards(jointTransform.localEulerAngles, new Vector3(-135, 0, 0), handRotateSpeed * Time.deltaTime);
                    _ableToDrink = true;
                }
                Debug.Log("DrinkMotion");
                break;

            case ARM_MOTION.POURING:
                if (reverse)
                {
                    jointTransform.rotation = Quaternion.RotateTowards(jointTransform.rotation, Camera.main.transform.rotation, handRotateSpeed * Time.deltaTime);
                    _ableToDrink = false;
                }
                else
                {
                    jointTransform.rotation = Quaternion.RotateTowards(jointTransform.rotation, Quaternion.AngleAxis(-135, Camera.main.transform.right), handRotateSpeed * Time.deltaTime);
                    _ableToDrink = true;
                }
                Debug.Log("PourMotion");

                break;
        }
    }

    #endregion

    void Start()
    {
        #region INIT_CHART_DATA

        _cd.Add(new ChartData(DRINK_TYPE.BLUE, DRINK_TYPE.BEER, DRINK_TYPE.BEER));
        _cd.Add(new ChartData(DRINK_TYPE.BLUE, DRINK_TYPE.BEER, DRINK_TYPE.BEER));
        _cd.Add(new ChartData(DRINK_TYPE.BLUE, DRINK_TYPE.VODKA, DRINK_TYPE.GRAVITY));
        _cd.Add(new ChartData(DRINK_TYPE.BLUE, DRINK_TYPE.WHISKY, DRINK_TYPE.WHISKY));
        _cd.Add(new ChartData(DRINK_TYPE.BLUE, DRINK_TYPE.SOJU, DRINK_TYPE.WATER));
        _cd.Add(new ChartData(DRINK_TYPE.BLUE, DRINK_TYPE.SAKE, DRINK_TYPE.BUBBLE));
        _cd.Add(new ChartData(DRINK_TYPE.BLUE, DRINK_TYPE.RUM, DRINK_TYPE.RUM));
        _cd.Add(new ChartData(DRINK_TYPE.BLUE, DRINK_TYPE.TEQUILA, DRINK_TYPE.TEQUILA));
        _cd.Add(new ChartData(DRINK_TYPE.BLUE, DRINK_TYPE.MOSCATO, DRINK_TYPE.LIFE));

        _cd.Add(new ChartData(DRINK_TYPE.GREEN, DRINK_TYPE.BEER, DRINK_TYPE.BEER));
        _cd.Add(new ChartData(DRINK_TYPE.GREEN, DRINK_TYPE.VODKA, DRINK_TYPE.VODKA));
        _cd.Add(new ChartData(DRINK_TYPE.GREEN, DRINK_TYPE.WHISKY, DRINK_TYPE.STRENGTH));
        _cd.Add(new ChartData(DRINK_TYPE.GREEN, DRINK_TYPE.SOJU, DRINK_TYPE.GRASS));
        _cd.Add(new ChartData(DRINK_TYPE.GREEN, DRINK_TYPE.SAKE, DRINK_TYPE.SAKE));
        _cd.Add(new ChartData(DRINK_TYPE.GREEN, DRINK_TYPE.RUM, DRINK_TYPE.RUM));
        _cd.Add(new ChartData(DRINK_TYPE.GREEN, DRINK_TYPE.TEQUILA, DRINK_TYPE.FIRE));
        _cd.Add(new ChartData(DRINK_TYPE.GREEN, DRINK_TYPE.MOSCATO, DRINK_TYPE.MOSCATO));

        _cd.Add(new ChartData(DRINK_TYPE.YELLOW, DRINK_TYPE.BEER, DRINK_TYPE.GLITTER));
        _cd.Add(new ChartData(DRINK_TYPE.YELLOW, DRINK_TYPE.VODKA, DRINK_TYPE.VODKA));
        _cd.Add(new ChartData(DRINK_TYPE.YELLOW, DRINK_TYPE.WHISKY, DRINK_TYPE.GLOWING));
        _cd.Add(new ChartData(DRINK_TYPE.YELLOW, DRINK_TYPE.SOJU, DRINK_TYPE.SOJU));
        _cd.Add(new ChartData(DRINK_TYPE.YELLOW, DRINK_TYPE.SAKE, DRINK_TYPE.SQUARE));
        _cd.Add(new ChartData(DRINK_TYPE.YELLOW, DRINK_TYPE.RUM, DRINK_TYPE.RUM));
        _cd.Add(new ChartData(DRINK_TYPE.YELLOW, DRINK_TYPE.TEQUILA, DRINK_TYPE.TEQUILA));
        _cd.Add(new ChartData(DRINK_TYPE.YELLOW, DRINK_TYPE.MOSCATO, DRINK_TYPE.RAINBOW));

        #endregion

        speed = 5.0f;

        _src1 = _src2 = _compost = DRINK_TYPE.EMPTY;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        throwAngle = 90 - throwAngle;
    }

    void Update()
    {
        #region MOVE_METHOD
        //For stop
        //humanModel.transform.localEulerAngles = new Vector3(0, Camera.main.gameObject.transform.localEulerAngles.y, 0);

        // For walk
        //forward = Camera.main.transform.TransformDirection(Vector3.forward);
        //right = Camera.main.transform.TransformDirection(Vector3.right);
        //moveDirection = Input.GetAxis("Horizontal") * right + Input.GetAxis("Vertical") * forward;
        //moveDirection *= speed;

        //transform.Translate(moveDirection);

        // For fly
        //transform.Translate(speed * Input.GetAxis("Horizontal") * Time.deltaTime, 0f, speed * Input.GetAxis("Vertical") * Time.deltaTime);

        //if (Input.GetKey(KeyCode.Escape))
        //{
        //    SceneManager.LoadScene("RyoheiTest");
        //}
        #endregion

        //Not use
        #region TOUCH_METHOD

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
        //            if (_src1 == DRINK_TYPE.EMPTY){
        //                _src1 = _temp.GetDrinkType();
        //                _alcoholContent1 = _temp.GetAlcoholContent();}
        //            else if (_src2 == DRINK_TYPE.EMPTY) { 
        //                _src2 = _temp.GetDrinkType();
        //                _alcoholContent2 = _temp.GetAlcoholContent();}

        //        #endregion
        //}}

        #endregion

        DisplaySelectedDrinks();

    }


    #region Ryohei

    public void DisplaySelectedDrinks()
    {
        int sum = _alcoholContent1 + _alcoholContent2;
        _txtSrc1.text = "SRC1 = " + DrinkExt.DisplayName(_src1);
        _txtSrc2.text = "SRC2 = " + DrinkExt.DisplayName(_src2);
        _txtCompost.text = "COMP = " + DrinkExt.DisplayName(_compost);
        _txtCurrentAlcoholContent.text = "CUAC = " + sum.ToString();
    }
    #endregion
}

static class DrinkExt
{
    public static string DisplayName(this DRINK_TYPE drink)
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