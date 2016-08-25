using UnityEngine;
using System.Collections;

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
    #endregion

    #region [ Public Methods ]
    public void GrabObject()
    {
        if (_currentObject == null)
        {

            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);

            //Assign object as current object
            if (Physics.Raycast(ray, out hit, 1000))
            {
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
                }
            }
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

    public void MixDrinks()
    {
        //Only can mix drinks
        if (_interactableObject != null && _interactableObject.objectType == InteractableObjects.OBJECT_TYPE.DRINKS)
        {

        }
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
        throwAngle = 90 - throwAngle;
    }

    void Update()
    {
        
    }
}
