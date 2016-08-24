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
	Vector3 throwAngle;

    #endregion

	#region [ Public Vars ]
	public GameObject offset;

	[Range(1, 100)]
	public int frameInterval;

	[Range(0, 5)]
	public float grabSpeed;

	public float throwForce;

	public bool lookingAtGlassCup { get { return _lookingAtGlassCup; } }

	public TextMesh debugText;
	#endregion

    #region [ Public Methods ]
    public void GrabObject()
    {
		if (_currentObject == null) {
			
			RaycastHit hit;
			Ray ray = new Ray (transform.position, transform.forward);

			//Assign object as current object
			if (Physics.Raycast (ray, out hit, 10000)) {
				//Check if object is interactable
				if (hit.collider.gameObject.GetComponent<InteractableObjects> () != null) {
					_currentObject = hit.collider.gameObject;
					_currentRB = _currentObject.GetComponent<Rigidbody> ();
					_interactableObject = _currentObject.GetComponent<InteractableObjects> ();
					StartCoroutine ("MoveObjectTowardsPlayer");
					StartCoroutine ("CheckLookObject");	
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
			Vector3 throwingAngle = new Vector3(0, 15, 0);
			throwingAngle.Normalize ();
			throwAngle = (gameObject.transform.forward + throwingAngle);
			throwAngle.Normalize ();
			_currentRB.AddForce (throwAngle * throwForce);
			ReleaseObject ();
        }
    }

	public void ReleaseObject()
	{
		_currentRB.useGravity = true;
		_currentObject = null;
		_currentRB = null;
		StopCoroutine ("CheckLookObject");
		StopCoroutine ("MoveObjectTowardsPlayer");
	}

	public bool IsHoldingObject()
	{
		if (_currentObject == null)
			return false;
		else
			return true;
	}

	public void PourDrinks()
	{
		//Only can mix drinks
		if (_interactableObject.objectType == InteractableObjects.OBJECT_TYPE.DRINKS) {
			
		}

	}

    #endregion

    #region [ Private Methods ]
	IEnumerator MoveObjectTowardsPlayer()
    {
		while (_currentObject.transform.position != offset.transform.position) {
			//_grabTimer += grabSpeed * Time.deltaTime;
			_currentRB.useGravity = false;
			_currentObject.transform.position = Vector3.Lerp (_currentObject.transform.position, offset.transform.position, grabSpeed * Time.deltaTime);

			yield return new WaitForSeconds(Time.deltaTime);
		}
		StopCoroutine ("MoveObjectTowardsPlayer");
    }

    #endregion

	IEnumerator CheckLookObject()
	{
		while (_currentObject != null) {
			RaycastHit hit;
			Ray ray = new Ray (transform.position, transform.forward);

			//Assign object as current object
			if (Physics.Raycast (ray, out hit, 10000)) {
				if (hit.collider.gameObject.name == "Glass Cup") {
					_lookingAtGlassCup = true;
				} 
			}
			else {
				_lookingAtGlassCup = false;
			}
			//Check at preferred frame interval
			yield return new WaitForSeconds(frameInterval * Time.deltaTime);
		}
	}

}
