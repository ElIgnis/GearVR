using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputManager : MonoBehaviour
{
    
	public TextMesh debugText;
	PlayerActions _playerActions;

    private RaycastHit _hit;
    private Ray _ray;
    public GameObject _targetObject;
    public GameObject _reticle;

	// Use this for initialization
	void Start ()
	{
		OVRTouchpad.Create ();
		OVRTouchpad.TouchHandler += UpdateInput;
        _playerActions = GetComponent<PlayerActions> ();

        Cursor.visible = false;

        Debug.Log("InputManagerStarted");

	}

	void UpdateInput (object sender, System.EventArgs arg)
	{
		OVRTouchpad.TouchArgs touchArg = (OVRTouchpad.TouchArgs)arg;

		switch (touchArg.TouchType) {
		case OVRTouchpad.TouchEvent.SingleTap:
			UpdateInteraction ();
//			debugText.text = "Tap";
			break;
		case OVRTouchpad.TouchEvent.Up:
//			debugText.text = "Up";
			break;
		case OVRTouchpad.TouchEvent.Down:
//			debugText.text = "Down";
			break;
		case OVRTouchpad.TouchEvent.Left:
			_playerActions.ThrowObject ();
//			debugText.text = "Left";
			break;
		case OVRTouchpad.TouchEvent.Right:
//			debugText.text = "Right";
			break;
		}
	}

	void UpdateInteraction ()
	{
    //  debugText.text = _playerActions.lookingAtGlassCup + "";
        if (_playerActions.IsHoldingObject())
        {

            //Release the object if not looking at glass cup
            if (!_playerActions.lookingAtGlassCup)            {

                Debug.Log("RELEASING");
                _playerActions.ReleaseObject();
            }
            else
            {
                _playerActions.PourCurrentDrink();
            }
        }
        else
        {
            _playerActions.GrabObject();
        }

	}

    float dis;

    void Update()
    {
        _playerActions.DrinkObject();

        if (_playerActions.IsHoldingObject())
            _playerActions.MoveObjectTowardsPlayer();
        
        //FPS
        //debugText.text = "DT = " + Time.deltaTime + "\nFPS = " + 1 / Time.deltaTime;

        //humanModel.transform.localEulerAngles = new Vector3(humanModel.transform.rotation.x, Camera.main.transform.rotation.y, humanModel.transform.rotation.z);

        _ray = new Ray(transform.position, transform.forward);

        float touchableDistance = 100;
        //Assign object as current object
        if (Physics.Raycast(_ray, out _hit, touchableDistance))
        {
            //Check if object is interactable

            GameObject hitObject = _hit.collider.gameObject;

            if (hitObject.GetComponent<InteractableObjects>() != null)
            {

            }
                _targetObject = _hit.collider.gameObject;

            Vector3 Apos = GameObject.Find("Main Camera").transform.position;
            Vector3 Bpos = _targetObject.transform.position;

            dis = Vector3.Distance(Apos, Bpos);
            touchableDistance += dis;

        }
         //   _reticle.transform.localScale = new Vector3(touchableDistance / 100, touchableDistance / 100, 1);
    }
}

