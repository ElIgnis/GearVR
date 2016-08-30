using UnityEngine;
using System.Collections;

public class DartBoard : MonoBehaviour
{

    #region [ Private Vars ]
    int _score;
    #endregion

    #region [ Public Vars ]
    public int[] scoreAmount;
    public TextMesh displayScore;
    public AudioClip dartHitSound;
    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Dart")
        {
            AudioSource.PlayClipAtPoint(dartHitSound, other.transform.position);
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;

            //Calculate score based on angle and position
            Vector3 direction = other.gameObject.transform.position - gameObject.transform.position;
            //float distance = direction.magnitude;
            
            //Angle calculation
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle += 180.0f;

            int finalAngle = (int)angle / scoreAmount.Length;
            _score = scoreAmount[finalAngle];

            ////Distance calculation - Outer Bullseye
            //if(distance < 0.5f)
            //{
            //    _score = 50;
            //}
            ////Distance calculation - Inner Bullseye
            //else if(distance >= 0.5f && distance < 1.0f)
            //{
            //    _score = 25;
            //}
            ////Distance calculation - Outer Ring
            //else if (distance >= 0.5f && distance < 1.0f)
            //{
            //    _score *= 2;
            //}
            ////Distance calculation - Inner Ring
            //else if (distance >= 0.5f && distance < 1.0f)
            //{
            //    _score *= 3;
            //}

            displayScore.text = (int)( _score) + "";
        }
    }
}
