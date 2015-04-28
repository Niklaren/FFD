using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFD2D : MonoBehaviour {

	public Mesh mesh;

	public float coefficients;
	public List<GameObject> controlPoints;
	//public List<GameObject[,] > controlPoints = new List<GameObject[,]>(); 
	public List<GameObject> connectors;
	public List<Vector3> meshCoordinates;
	//public List <LineRenderer> connectors;
	
	public Vector3 p0;
	public Vector3 pN;
	public float S,T,U;
	
	public GameObject CPNode;
	public GameObject connector;

	int CPs_s = 4;
	//float CPs_t = 1;
	int CPs_u = 4;

	//List <float> [] Bs;
	//List <float> [] Bt;
	//List <float> [] Bu;
	
	// Use this for initialization
	void Start () {
		InitMesh ();

		InitCPs ();

		InitConnectors ();
		//foreach (Vector3 vertex in mesh.vertices) {
		//	glarbnu(vertex);
		//}
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 [] verts = new Vector3[mesh.vertexCount]; 
		verts = mesh.vertices;

		for (int i = 0; i < mesh.vertexCount; i++){
			//verts[i] = glarbnu(mesh.vertices[i]);
			verts[i] = calculateSurface(i);
			//Debug.Log(mesh.vertices[i]+"|"+verts[i]);

			if (mesh.vertices[i] != verts[i]){
				//Debug.Log(mesh.vertices[i]+"|"+verts[i]);
				//mesh.vertices[i] = verts[i];
				//Debug.Log("unequal");
			}
		}

		if (mesh.vertices != verts) {
			mesh.vertices = verts;
			MoveConnectors();
		//	mesh.RecalculateBounds();
		//	mesh.RecalculateNormals();
		}

		//foreach (Vector3 vertex in mesh.vertices) {
		//	//Debug.Log(vertex);
		//}
	}

	void InitMesh(){
		mesh = GetComponent<MeshFilter>().mesh;
		
		pN = mesh.vertices [0];
		p0 = mesh.vertices [mesh.vertexCount - 1];
		
		S = mesh.bounds.size.x;
		T = mesh.bounds.size.y;
		U = mesh.bounds.size.z;

		for (int i = 0; i < mesh.vertexCount; i++){
			float s = ((mesh.vertices[i].x - p0.x) / (pN.x - p0.x));
			float t = ((mesh.vertices[i].y - p0.y) / (pN.y - p0.y));
			float u = ((mesh.vertices[i].z - p0.z) / (pN.z - p0.z));
			meshCoordinates.Add(new Vector3(s,t,u));
		}
	}

	void CreateBernsteinCoefficients(){

	}
	
	void InitCPs(){
		
		int i =0, j = 0;
		float s,/*t,*/u;

		//P = p0 + (i/l)S + (j/m)T + (k/n)U;

		for (u = 0.0f; u <= 1.0f; u += 1.0f/(CPs_u-1)) {
			for (s = 0.0f; s <= 1.0f; s += 1.0f/(CPs_s-1),i++) {
				GameObject Node = Instantiate(CPNode, transform.position, Quaternion.identity) as GameObject;
				Node.transform.parent = transform;
				Node.transform.localPosition = (p0 + new Vector3(s*S,0,u*U)); // 
				controlPoints.Add(Node);
				//controlPoints.Add(p0 + new Vector3(x*S,0,z*U));
			
			}
		}

		for (i = 0; i < 4; i++) {
			for (j = 0; j < 4; j++) {
				//Debug.Log( controlPoints[j + (i*4)]);
			}
		}

	}

	void InitConnectors(){
		int i, j;
		for (i = 0; i < CPs_u-1; i++) {
			for (j = 0; j < CPs_s; j++) {
				GameObject connection = Instantiate (connector, transform.position, Quaternion.identity) as GameObject;
				connection.transform.parent = transform;
				LineRenderer lr = connection.GetComponent<LineRenderer> ();
				lr.SetVertexCount (2);
				lr.SetWidth (0.01f, 0.01f);
				lr.SetPosition (0, controlPoints [j + (i * (CPs_u))].transform.position);
				lr.SetPosition (1, controlPoints [j + ((i+1) * (CPs_u))].transform.position);
				connectors.Add (connection);
			}
		}
		
		for (i = 0; i < CPs_u; i++) {
			for (j = 0; j < CPs_s-1; j++) {
				GameObject connection = Instantiate(connector, transform.position, Quaternion.identity) as GameObject;
				connection.transform.parent = transform;
				LineRenderer lr = connection.GetComponent<LineRenderer>();
				lr.SetVertexCount(2);
				lr.SetWidth(0.01f,0.01f);
				lr.SetPosition(0,controlPoints[j + (i*(CPs_u))].transform.position);
				lr.SetPosition(1,controlPoints[j+1 + ((i+0)*(CPs_u))].transform.position);
				connectors.Add(connection);
			}
		}
	}

	void MoveConnectors(){
		int i, j;

		for (i = 0; i < CPs_s-1; i++) {
			for (j = 0; j < CPs_u; j++) {
				//if(i<(CPs_u-1)){
					LineRenderer lr = connectors[j + (i*CPs_u)].GetComponent<LineRenderer>();
					lr.SetPosition(0,controlPoints[j + (i*(CPs_u))].transform.position);
					lr.SetPosition(1,controlPoints[j+0 + ((i+1)*(CPs_u))].transform.position);
				//}
			}
		}

		for (i = 0; i < CPs_s; i++) {
			for (j = 0; j < CPs_u-1; j++) {
				//if(j<(CPs_s-1)){
					LineRenderer lr = connectors[i + (j*CPs_u) + ((CPs_s-1)*(CPs_u))].GetComponent<LineRenderer>();
					lr.SetPosition(0,controlPoints[j + (i*(CPs_u))].transform.position);
					lr.SetPosition(1,controlPoints[j+1 + ((i)*(CPs_u))].transform.position);
				//}
			}
		}
	}

	Vector3 calculateSurface(int index){
		float s = meshCoordinates[index].x;
		float t = meshCoordinates[index].y;
		float u = meshCoordinates[index].z;
		
		//Debug.Log (s + " | " + u);
		
		float [] Bs = new float[4];
		float [] Bu = new float[4];
		
		Bs[0] = (1.0f - s) * (1.0f - s) * (1.0f - s);
		Bs[1] = 3.0f * s * (1 - s) * (1.0f - s);
		Bs[2] = 3.0f * s * s * (1.0f - s);
		Bs[3] = s * s * s;
		
		Bu[0] = (1.0f - u) * (1.0f - u) * (1.0f - u);
		Bu[1] = 3.0f * u * (1 - u) * (1.0f - u);
		Bu[2] = 3.0f * u * u * (1.0f - u);
		Bu[3] = u * u * u;
		
		//Debug.Log(Bs[0]+"|"+Bs[1]+"|"+Bs[2]+"|"+Bs[3]);
		//Debug.Log(Bu[0]+"|"+Bu[1]+"|"+Bu[2]+"|"+Bu[3]);
		
		Vector3 pos = new Vector3 (0,0,0);
		
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				pos += controlPoints[i + (j*4)].transform.localPosition * (Bs[i] * Bu[j]);
			}
		}
		//Debug.Log(gleeb);
		
		return pos;
	}
}
