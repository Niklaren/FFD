using UnityEngine;
using System.Collections;

public class CameraControlScript : MonoBehaviour {
	
	public float sensitivityX = 2F;
	public float sensitivityY = 2F;
	
	public float mHdg = 0F;
	public float mPitch = 45F;

	public Transform mTarget;
	
	void Start()
	{
		// owt?
	}
	
	void Update()
	{
		float scroll = Input.GetAxis ("Mouse ScrollWheel");

		if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) && (scroll == 0))
			return;
		
		float deltaX = Input.GetAxis("Mouse X") * sensitivityX;
		float deltaY = Input.GetAxis("Mouse Y") * sensitivityY;


		if (Input.GetMouseButton (2)) {
			Strafe (-deltaX * 0.5f);
			ChangeHeight (-deltaY);
		} else if (Input.GetMouseButton (0) && Input.GetMouseButton (1)) {
			MoveForwards (deltaY);
			ChangeHeight(-deltaY);
		} else if (Input.GetMouseButton (1)) {
			ChangeHeading (deltaX);
			ChangePitch (-deltaY);
		} else if (scroll != 0) {
			MoveForwards (scroll * 3 * sensitivityY);
		}
	}
	
	void MoveForwards(float aVal)
	{
		Vector3 fwd = transform.forward;
		fwd.y = 0;
		fwd.Normalize();
		transform.position += aVal * fwd;
	}
	
	void Strafe(float aVal)
	{
		transform.position += aVal * transform.right;
	}
	
	void ChangeHeight(float aVal)
	{
		transform.position += aVal * Vector3.up;
	}
	
	void ChangeHeading(float aVal)
	{
		mHdg += aVal;
		//mHdg =  transform.rotation.y + aVal;
		WrapAngle(ref mHdg);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}
	
	void ChangePitch(float aVal)
	{
		mPitch += aVal;
		//mPitch = transform.rotation.y + aVal;
		WrapAngle(ref mPitch);
		transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
	}
	
	public static void WrapAngle(ref float angle)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
	}
	
} 
