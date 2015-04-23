using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFD : MonoBehaviour {

	public Mesh mesh;
	public List<Vector3> meshCoordinates;
	public List<GameObject> controlPoints;
	
	public Vector3 p0;
	public Vector3 pN;
	public float S,T,U;
	
	public GameObject CPNode;
	
	// Use this for initialization
	void Start () {
		InitMesh ();
		
		InitCPs ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 [] verts = new Vector3[mesh.vertexCount]; 
		verts = mesh.vertices;
		
		for (int i = 0; i < mesh.vertexCount; i++){
			verts[i] = EvalArea(i);
		}
		
		if (mesh.vertices != verts) {
			mesh.vertices = verts;
			//	mesh.RecalculateBounds();
			//	mesh.RecalculateNormals();
		}
	}

	void CreateBernsteinCoefficients(){
		/*
		float [] Bs = new float[4];
		float [] Bt = new float[4];
		float [] Bu = new float[4];

		Bs[0] = (1.0f - s) * (1.0f - s) * (1.0f - s);
		Bs[1] = 3.0f * s * (1 - s) * (1.0f - s);
		Bs[2] = 3.0f * s * s * (1.0f - s);
		Bs[3] = s * s * s;

		Bt[0] = (1.0f - t) * (1.0f - t) * (1.0f - t);
		Bt[1] = 3.0f * t * (1 - t) * (1.0f - t);
		Bt[2] = 3.0f * t * t * (1.0f - t);
		Bt[3] = t * t * t;
		
		Bu[0] = (1.0f - u) * (1.0f - u) * (1.0f - u);
		Bu[1] = 3.0f * u * (1 - u) * (1.0f - u);
		Bu[2] = 3.0f * u * u * (1.0f - u);
		Bu[3] = u * u * u;
		*/
	}

	void InitMesh(){
		mesh = GetComponent<MeshFilter>().mesh;
		
		S = mesh.bounds.size.x;
		T = mesh.bounds.size.y;
		U = mesh.bounds.size.z;

		//p0 = mesh.vertices [mesh.vertexCount - 1];
		//pN = mesh.vertices [0];
		p0 = - new Vector3 (S/2, T/2, U/2);
		pN = new Vector3 (S/2, T/2, U/2);

		for (int i = 0; i < mesh.vertexCount; i++){
			float s = ((mesh.vertices[i].x - p0.x) / (pN.x - p0.x));
			float t = ((mesh.vertices[i].y - p0.y) / (pN.y - p0.y));
			float u = ((mesh.vertices[i].z - p0.z) / (pN.z - p0.z));
			meshCoordinates.Add(new Vector3(s,t,u));
		}
	}
	
	void InitCPs(){
		
		int i = 0;
		float x,y,z;
		
		//P = p0 + (i/l)S + (j/m)T + (k/n)U;

		for (x = 0.0f; x <= 1.0f; x += 1.0f/2.0f) {
			for (y = 0.0f; y <= 1.0f; y += 1.0f/3.0f) {
				for (z = 0.0f; z <= 1.0f; z += 1.0f/3.0f,i++) {
					GameObject Node = Instantiate(CPNode, transform.position, Quaternion.identity) as GameObject;
					Node.transform.parent = transform;
					Node.transform.localPosition = (p0 + new Vector3(x*S,y*T,z*U)); // 
					controlPoints.Add(Node);
				}
			}
		}
		
		for (i = 0; i < 3; i++) {
			for (int j = 0; j < 4; j++) {
				for (int k = 0; k < 4; k++) {
					//Debug.Log( k + (j*4) + (i*4*4));
					//Debug.Log( controlPoints[k + (j*4) + (i*4*4) ].transform.position);
				}
			}
		}
	}

	Vector3 EvalArea(int index){
		float s = meshCoordinates[index].x;
		float t = meshCoordinates[index].y;
		float u = meshCoordinates[index].z;

		float [] Bs = new float[3];
		float [] Bt = new float[4];
		float [] Bu = new float[4];
		
		Bs[0] = (1.0f - s) * (1.0f - s);
		Bs[1] = 2.0f * s * (1.0f - s);
		Bs[2] = s * s;
		
		Bt[0] = (1.0f - t) * (1.0f - t) * (1.0f - t);
		Bt[1] = 3.0f * t * (1 - t) * (1.0f - t);
		Bt[2] = 3.0f * t * t * (1.0f - t);
		Bt[3] = t * t * t;
		
		Bu[0] = (1.0f - u) * (1.0f - u) * (1.0f - u);
		Bu[1] = 3.0f * u * (1 - u) * (1.0f - u);
		Bu[2] = 3.0f * u * u * (1.0f - u);
		Bu[3] = u * u * u;

		Vector3 point = new Vector3 (0,0,0);
		
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 4; j++) {
				for (int k = 0; k < 4; k++) {
					point += controlPoints[k+(j*4)+ (i*4*4)].transform.localPosition * (Bs[i]*Bt[j]*Bu[k]);
				}
			}
		}
		//Debug.Log(gleeb);
		
		return point;
	}

}
