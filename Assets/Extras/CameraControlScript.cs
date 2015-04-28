using UnityEngine;
using System.Collections;

public class CameraControlScript : MonoBehaviour {

	public float Yaw = 0F;
	public float Pitch = 45F;

	void Start()
	{

	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();

		float scroll = Input.GetAxis ("Mouse ScrollWheel");

		if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) && (scroll == 0))
			return;
		
		float deltaX = Input.GetAxis("Mouse X");
		float deltaY = Input.GetAxis ("Mouse Y");

		if (Input.GetMouseButton (1)) {
			ChangeYaw (deltaX);
			ChangePitch (-deltaY);
		} else if (scroll != 0) {
			MoveForwards (scroll * 4);
		}
		else if (Input.GetMouseButton (2)) {
			Strafe (-deltaX * 0.5f);
			ChangeHeight (-deltaY);
		}
	}
	
	void MoveForwards(float f)
	{
		transform.position += (f * transform.forward);
	}
	
	void Strafe(float s)
	{
		transform.position += (s * transform.right);
	}
	
	void ChangeHeight(float h)
	{
		transform.position += (h * Vector3.up);
	}
	
	void ChangeYaw(float y)
	{
		Yaw += y;
		WrapAngle(ref Yaw);
		transform.localEulerAngles = new Vector3(Pitch, Yaw, 0);
	}
	
	void ChangePitch(float p)
	{
		Pitch += p;
		WrapAngle(ref Pitch);
		transform.localEulerAngles = new Vector3(Pitch, Yaw, 0);
	}
	
	public static void WrapAngle(ref float angle)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
	}
	
} 
