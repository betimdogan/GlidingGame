using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketmanController : MonoBehaviour
{
    [SerializeField]
    Animator stick_animator;
	Animator rocketman_animator;
    // At first (before we throw the Rocketman), bool ReadyToGlide is false
    bool ReadyToGlide;
    Rigidbody rb;

    [SerializeField]
    FixedJoint stick_FixedJoint;

    Vector2 startPos, endPos, direction; // touch start position, touch end position, swipe direction

	[SerializeField]
	float throwForceInXandY = 1f; // to control throw force in X and Y directions

    //This is the Main Camera in the Scene
    Camera m_MainCamera;
    //This is the second Camera and is assigned in inspector
    public Camera m_CameraTwo;
    bool onGround;
    bool areWingsOpened;
    float rotateValue = 500.0f;
    private Dictionary<int, Vector2> activeTouches = new Dictionary<int, Vector2>();
    public GameObject RestartPanel;

    void Start()
    {
		rocketman_animator = gameObject.GetComponent<Animator>();
        ReadyToGlide = false;
        rb = GetComponent<Rigidbody> ();
        //This gets the Main Camera from the Scene
        m_MainCamera = Camera.main;
        //This enables Main Camera
        m_MainCamera.enabled = true;
        //To disable secondary Camera
        m_CameraTwo.enabled = false;
        onGround = false;
        areWingsOpened = false;
        
    }

    void Update()
    {
        float x = GetPlayerInput().x;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;

        // At the beginning, throw the rocketman
		if(!ReadyToGlide)
		{
			swipe();
		}
		// When the Rocketman is in the air, and no fingers on the screen
		if(!onGround && ReadyToGlide && Input.touchCount == 0)
		{
			rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

			RotateRocketman();

			if(areWingsOpened)
			{
				rocketman_animator.SetTrigger("CloseWing");
				areWingsOpened = false;
			}
		}
        // When the Rocketman is in the air and we touch the screen 
		if( ReadyToGlide && Input.touchCount > 0 )
		{
			if(areWingsOpened)
			{
				transform.localRotation = Quaternion.Lerp( transform.localRotation, Quaternion.Euler(new Vector3(90, 0, 0)) , 0.1f);
				transform.localRotation = Quaternion.Lerp( transform.localRotation, (transform.localRotation * Quaternion.Euler(new Vector3(0, x * 20.0f, 0)) ), 0.1f);
				transform.localPosition = Vector3.Lerp( transform.localPosition, transform.localPosition + new Vector3(x, 0, 0) * 1.0f, 0.1f);
			}
			
			glide();
		}

        // If the Rocketman fell off the plane
        if(transform.position.y < 0)
        {
            onGround=true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
			RestartPanel.SetActive(true);
        }
        
    }

	private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "CylinderSurface")
        {
			rb.velocity = new Vector3(0,20,0);
			Debug.Log("CylinderSurface a çarptı");
			Handheld.Vibrate();
       	}
		if(other.gameObject.tag == "PrismSurface")
        {
			rb.velocity = new Vector3(0,10,0);
			Debug.Log("PrismSurface a çarptı");
			Handheld.Vibrate();
       	}
		if(other.gameObject.tag == "Ground")
		{
			Handheld.Vibrate();
			onGround = true;
			rb.constraints = RigidbodyConstraints.FreezeAll;
			Debug.Log("On Ground");
			RestartPanel.SetActive(true);
        	Time.timeScale = 0.0f;
		}
		if(areWingsOpened)
		{
			rocketman_animator.SetTrigger("CloseWing");
			areWingsOpened = false;
		}
    }

    void swipe()
	{
        rb.constraints = RigidbodyConstraints.FreezePositionX;
		// if you touch the screen
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began) 
		{
			startPos = Input.GetTouch (0).position;
			stick_animator.SetTrigger("Bended");
		}

		// if you release your finger
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) 
		{
			// getting release finger position
			endPos = Input.GetTouch (0).position;
			// calculating swipe direction in 2D space	
			direction = startPos - endPos;
			// add force to balls rigidbody in 3D space depending on swipe time, direction and throw forces
			rb.isKinematic = false;

			stick_FixedJoint.connectedBody = null;

			rb.AddForce (0, direction.y * throwForceInXandY, direction.x * throwForceInXandY);

			stick_animator.SetTrigger("Released");

			Invoke("ChangeCamera", 1.0f); 
            // Rocketman was thrown so we are ready to glide now.
			ReadyToGlide = true;

		}
	}

    void ChangeCamera()
	{
		//Check that the Main Camera is enabled in the Scene, then switch to the other Camera on a key press
       	if (m_MainCamera.enabled)
        {
            //Enable the second Camera
            m_CameraTwo.enabled = true;

            //The Main first Camera is disabled
            m_MainCamera.enabled = false;
        }
	}
	void RotateRocketman()
	{
		// Rotation
		transform.Rotate(rotateValue*Time.deltaTime, 0, 0);
	}
	void glide()
	{
		rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

		if(Input.GetTouch (0).phase == TouchPhase.Began)
		{
			if(!areWingsOpened)
				{
					rocketman_animator.SetTrigger("OpenWing");
					areWingsOpened = true;
				}
			rb.drag = 7f;
		}
		if((Input.GetTouch (0).phase == TouchPhase.Moved) || (Input.GetTouch (0).phase == TouchPhase.Stationary))
		{
				rb.drag = 7f;
		}
		if(Input.GetTouch (0).phase == TouchPhase.Ended)
		{
			rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			rb.drag = 0.5f;
		}		
	}
	public Vector3 GetPlayerInput()
    {
        Vector3 r = Vector3.zero;
        foreach (Touch touch in Input.touches)
        {
            // If we just started pressing on the screen
            // We are going to register that touch in the dictionary
            if(touch.phase == TouchPhase.Began)
            {
                // we store the starting position
                activeTouches.Add(touch.fingerId, touch.position); 
            }
            // if we remove the finger off the screen
            // we also remove that touch from the dictionary
            else if (touch.phase == TouchPhase.Ended)
            {
                if(activeTouches.ContainsKey(touch.fingerId)){
                    activeTouches.Remove(touch.fingerId);
                }
            }
            // Our finger is either moving, or stationary
            // in both cases, use the delta from the original position
            else
            {
                // magnitudeF is the distance for dragging finger across
                float magnitudeF = 0;
                // current position of the finger - finger position at the beginning
                r = (touch.position - activeTouches[touch.fingerId]);
                magnitudeF = r.magnitude / 300;
                r = r.normalized * magnitudeF;
            }
        }
        return r; 
    }

}
