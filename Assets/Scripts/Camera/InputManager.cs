using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public UnityStandardAssets.ImageEffects.MotionBlur motionblur;
    PlayerActions _playerActions;
    
    private RaycastHit _hit;
    private Ray _ray;
    public GameObject _targetObject;
    public GameObject _reticle;

    // Use this for initialization
    void Start()
    {
        OVRTouchpad.Create();
        OVRTouchpad.TouchHandler += UpdateInput;
        _playerActions = GetComponent<PlayerActions>();

        Cursor.visible = false;
        
        Debug.Log("InputManagerStarted");
    }

    void UpdateInput(object sender, System.EventArgs arg)
    {
        OVRTouchpad.TouchArgs touchArg = (OVRTouchpad.TouchArgs)arg;

        switch (touchArg.TouchType)
        {
            case OVRTouchpad.TouchEvent.SingleTap:
                UpdateInteraction();
                break;
            case OVRTouchpad.TouchEvent.Up:
                motionblur.blurAmount += 0.1f;
                Debug.Log(motionblur.blurAmount);
                break;
            case OVRTouchpad.TouchEvent.Down:
                motionblur.blurAmount -= 0.1f;
                Debug.Log(motionblur.blurAmount);
                break;
            case OVRTouchpad.TouchEvent.Left:
                _playerActions.ThrowObject();
                break;
            case OVRTouchpad.TouchEvent.Right:
                break;
        }
    }

    void UpdateInteraction()
    {
        //  debugText.text = _playerActions.lookingAtGlassCup + "";
        if (_playerActions.IsHoldingObject())
        {

            //Release the object if not looking at glass cup
            if (!_playerActions.lookingAtGlassCup)
            {

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
        //Alternate controls (to be remapped if there is time tomorrow)
        if (Input.GetKeyDown(KeyCode.A))
            UpdateInteraction();

        if (Input.GetKeyDown(KeyCode.S))
            _playerActions.ThrowObject();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            //SceneManager.UnloadScene(0);
            SceneManager.LoadScene(0);
        }
            

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

