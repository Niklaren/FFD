using UnityEngine;
using System.Collections;

public class DragObject : MonoBehaviour {

	private Vector3 screenPoint;
	private Vector3 offset;
	
	void Start () {
	
	}

	void Update () {
	
	}

	void OnMouseDown()
	{
		screenPoint = Camera.main.WorldToScreenPoint (gameObject.transform.position);
		Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(mousePos);
	}

	void OnMouseDrag()
	{
		Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 Position = Camera.main.ScreenToWorldPoint(mousePos) + offset;
		transform.position = Position;
		
	}
	
}
