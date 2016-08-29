using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputManager : MonoBehaviour
{
    public GameObject humanModel;
	public TextMesh debugText;
	PlayerActions _playerActions;

    private RaycastHit _hit;
    private Ray _ray;
    public GameObject _targetObject;
    public GameObject _reticle;
    public RawImage _reticleImage;

	// Use this for initialization
	void Start ()
	{
		OVRTouchpad.Create ();
		OVRTouchpad.TouchHandler += UpdateInput;
		_playerActions = GetComponent<PlayerActions> ();
        _reticle = GameObject.Find("PlayerReticle");
        _reticleImage = _reticle.GetComponent<RawImage>();
        _reticleImage.material.SetColor("_Color", new Color(100, 0, 0, 1.0f));
        Debug.Log(_reticleImage.material.GetColor("_Color"));
        

	}

	void UpdateInput (object sender, System.EventArgs arg)
	{
		OVRTouchpad.TouchArgs touchArg = (OVRTouchpad.TouchArgs)arg;

		switch (touchArg.TouchType) {
		case OVRTouchpad.TouchEvent.SingleTap:
			UpdateInteraction ();
			debugText.text = "Tap";
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
        //_playerActions.DrinkObject();

        //if (_playerActions.IsHoldingObject())
        //    _playerActions.MoveObjectTowardsPlayer();
        //FPS
        debugText.text = "DT = " + Time.deltaTime + "\nFPS = " + 1 / Time.deltaTime;

        //debugText.text = gameObject.transform.forward + "";
        //humanModel.transform.localEulerAngles = new Vector3(humanModel.transform.rotation.x, Camera.main.transform.rotation.y, humanModel.transform.rotation.z);

            _ray = new Ray(transform.position, transform.forward);


            int touchableDistance = 1000;
            //Assign object as current object
            if (Physics.Raycast(_ray, out _hit, touchableDistance))
            {

                Debug.Log("Hit anything");
                //Check if object is interactable
                if (_hit.collider.gameObject.GetComponent<InteractableObjects>() != null)
                {
                    Debug.Log("Hit Interactable thing");

                    Debug.Log(_reticle);
                    _targetObject = _hit.collider.gameObject;
                    _reticleImage.material.SetColor("_Color", new Color(0, 0, 0, 0.0f));
                    
                }

                _reticleImage.material.SetColor("_Color", new Color(0, 0, 0, 1.0f));

            }
    }
}
