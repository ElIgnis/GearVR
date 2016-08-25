using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputManager : MonoBehaviour
{

	public TextMesh debugText;
	PlayerActions _playerActions;

	// Use this for initialization
	void Start ()
	{
		OVRTouchpad.Create ();
		OVRTouchpad.TouchHandler += UpdateInput;
		_playerActions = GetComponent<PlayerActions> ();
	}

	void UpdateInput (object sender, System.EventArgs arg)
	{
		OVRTouchpad.TouchArgs touchArg = (OVRTouchpad.TouchArgs)arg;

		switch (touchArg.TouchType) {
		case OVRTouchpad.TouchEvent.SingleTap:
			UpdateInteraction ();
			//debugText.text = "Tap";
			break;
		case OVRTouchpad.TouchEvent.Up:
			debugText.text = "Up";
			break;
		case OVRTouchpad.TouchEvent.Down:
			debugText.text = "Down";
			break;
		case OVRTouchpad.TouchEvent.Left:
			_playerActions.ThrowObject ();
			debugText.text = "Left";
			break;
		case OVRTouchpad.TouchEvent.Right:
			debugText.text = "Right";
			break;
		}
	}

	void UpdateInteraction ()
	{
        debugText.text = _playerActions.lookingAtGlassCup + "";
		if (_playerActions.IsHoldingObject ()) {

			//Release the object if not looking at glass cup
			if (!_playerActions.lookingAtGlassCup)
            {
                
                _playerActions.ReleaseObject();
            }
			else
            {
                _playerActions.MixDrinks();
            }
		}
			
		else
			_playerActions.GrabObject ();
	}

    void Update()
    {
        _playerActions.DrinkObject();

        if (_playerActions.IsHoldingObject())
            _playerActions.MoveObjectTowardsPlayer();
        //FPS
        //debugText.text = "DT = " + Time.deltaTime + "\nFPS = " + 1 / Time.deltaTime;

        debugText.text = gameObject.transform.forward + "";
    }
}
