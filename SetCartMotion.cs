//Copyright 2014 Ryan Hill
//do not redistribute without permission
using UnityEngine;
using System.Collections;
using MotionSim;

public class SetCartMotion : MonoBehaviour {
	public Transform direction;
	public Rigidbody cartb;
	public MotionStructure motion;

	public UDPSendMotion sender;

	public Vector3 prevvel;

	public float updateDelay=0.1f;

	public bool keepUpdating=true;
	// Use this for initialization
	void Start () {

		#if UNITY_STANDALONE

		motion = new MotionStructure();
		prevvel= new Vector3(0,0,0);
		updateDelay=  LoadConfigFile.GetInt("udp_output_rate")/1000f;
		#endif

	}


	void FixedUpdate() {
#if UNITY_STANDALONE

// get change in velocity since last fixedUpdate
        Vector3 accel = (prevvel-cartb.velocity)/Time.fixedDeltaTime;
		prevvel = cartb.velocity;

        // accel is magnitude of velocity change along each matching direction
		//Heave, Surge, Sway
		//Heave, surge and sway are the measurement of vertical and horizontal movements due to the waves. The heave is on the vertical axis, the surge is on the front-back axis and the sway is on the left-right axis
        motion._Heave = Vector3.Dot(direction.up,accel);
		motion._Surge = Vector3.Dot(direction.forward,accel);
		motion._Sway = Vector3.Dot(direction.right,accel);
		//set yaw according to direction of camera
		//Yaw
		Vector3 forr = direction.forward;
		motion._Yaw = -Mathf.Atan2 (forr.z,forr.x)*180/Mathf.PI;
		
///////////////	EDITED VERSION INI ///////////////
		//Pitch & Roll
		/*Getting Pitch float value (around local x-axis):*/
		var right = transform.right;
		right.y = 0;
			right *= Mathf.Sign(transform.up.y);
				var fwd = Vector3.Cross(right, Vector3.up).normalized;
						float pitch = Vector3.Angle(fwd, transform.forward) * Mathf.Sign(transform.forward.y);
								motion._Pitch= pitch;
		
		/*Getting Roll float value  (around local forward axis)*/
		var fwd = transform.forward;
			fwd.y = 0;
				fwd *= Mathf.Sign(transform.up.y);
					var right = Vector3.Cross(Vector3.up, fwd).normalized;
						float roll = Vector3.Angle(right, transform.right) * Mathf.Sign(transform.right.y);
									motion._Roll = roll;
		
///////////////	EDITED VERSION END ///////////////	

        //speed is speed of cart
		motion._Speed = cartb.velocity.magnitude;

		sender.motion = motion;
#endif
	
	}

    
}
