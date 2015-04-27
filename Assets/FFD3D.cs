using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FFD3D : MonoBehaviour {
	public Mesh mesh;
	public List<Vector3> meshCoordinates;
	public List<GameObject> connectors;
	public List<GameObject> controlPoints;
	
	public Vector3 p0;
	public Vector3 pN;
	public float S,T,U;

	int CPs_s = 3;
	int CPs_t = 4;
	int CPs_u = 4;

	private float [] Bs = new float[3];
	private float [] Bt = new float[4];
	private float [] Bu = new float[4];
	
	public GameObject CPNode;
	public GameObject connector;

	void Start () {
		InitMesh ();
		
		InitCPs ();

		InitConnectors ();
	}

	void Update () {
		Vector3 [] verts = new Vector3[mesh.vertexCount]; 
		verts = mesh.vertices;
		
		for (int i = 0; i < mesh.vertexCount; i++){
			verts[i] = EvalArea(i);
		}
		
		if (mesh.vertices != verts) {
			mesh.vertices = verts;
			MoveConnectors();
			//	mesh.RecalculateBounds();
			//	mesh.RecalculateNormals();
		}
	}

	void CreateBernsteinCoefficients(int index){
		float s = meshCoordinates[index].x;
		float t = meshCoordinates[index].y;
		float u = meshCoordinates[index].z;
		
		//float [] Bs = new float[3];
		//float [] Bt = new float[4];
		//float [] Bu = new float[4];
		
		Bs[0] = (1.0f - s) * (1.0f - s);
		Bs[1] = 2.0f * s * (1.0f - s);
		Bs[2] = s * s;
		
		Bt[0] = (1.0f - t) * (1.0f - t) * (1.0f - t);
		Bt[1] = 3.0f * t * (1.0f - t) * (1.0f - t);
		Bt[2] = 3.0f * t * t * (1.0f - t);
		Bt[3] = t * t * t;
		
		Bu[0] = (1.0f - u) * (1.0f - u) * (1.0f - u);
		Bu[1] = 3.0f * u * (1.0f - u) * (1.0f - u);
		Bu[2] = 3.0f * u * u * (1.0f - u);
		Bu[3] = u * u * u;
	}

	void InitMesh(){
		mesh = GetComponent<MeshFilter>().mesh;
		
		S = mesh.bounds.size.x;
		T = mesh.bounds.size.y;
		U = mesh.bounds.size.z;

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

		for (x = 0.0f; x < 1.0f; x += 1.0f/CPs_s) {
			for (y = 0.0f; y < 1.0f; y += 1.0f/CPs_t) {
				for (z = 0.0f; z < 1.0f; z += 1.0f/CPs_u,i++) {
					GameObject Node = Instantiate(CPNode, transform.position, Quaternion.identity) as GameObject;
					Node.transform.parent = transform;
					Node.transform.localPosition = (p0 + new Vector3(x*S,y*T,z*U)); // 
					controlPoints.Add(Node);
				}
			}
		}
	}

	void InitConnectors(){
		int i, j, k;
		for (i = 0; i < CPs_s-1; i++) {
			for (j = 0; j < CPs_t; j++) {
				for (k = 0; k < CPs_u; k++) {
					GameObject connection = Instantiate (connector, transform.position, Quaternion.identity) as GameObject;
					connection.transform.parent = transform;
					LineRenderer lr = connection.GetComponent<LineRenderer> ();
					lr.SetVertexCount (2);
					lr.SetWidth (0.01f, 0.01f);
					lr.SetPosition (0, controlPoints [k + (j*4) + (i*(CPs_t*CPs_u)) ].transform.position);
					lr.SetPosition (1, controlPoints [k + (j*4) + ((i+1)*(CPs_t*CPs_u)) ].transform.position);
					connectors.Add (connection);
				}
			}
		}
		
		for (i = 0; i < CPs_s; i++) {
			for (j = 0; j < CPs_t-1; j++) {
				for (k = 0; k < CPs_u; k++) {
					GameObject connection = Instantiate (connector, transform.position, Quaternion.identity) as GameObject;
					connection.transform.parent = transform;
					LineRenderer lr = connection.GetComponent<LineRenderer> ();
					lr.SetVertexCount (2);
					lr.SetWidth (0.01f, 0.01f);
					lr.SetPosition (0, controlPoints [k + (j*4) + (i*(CPs_t*CPs_u)) ].transform.position);
					lr.SetPosition (1, controlPoints [k + ((j+1)*4) + (i*(CPs_t*CPs_u)) ].transform.position);
					connectors.Add (connection);
				}
			}
		}

		for (i = 0; i < CPs_s; i++) {
			for (j = 0; j < CPs_t; j++) {
				for (k = 0; k < CPs_u-1; k++) {
					GameObject connection = Instantiate (connector, transform.position, Quaternion.identity) as GameObject;
					connection.transform.parent = transform;
					LineRenderer lr = connection.GetComponent<LineRenderer> ();
					lr.SetVertexCount (2);
					lr.SetWidth (0.01f, 0.01f);
					lr.SetPosition (0, controlPoints [k + (j*4) + (i*(CPs_t*CPs_u)) ].transform.position);
					lr.SetPosition (1, controlPoints [(k+1) + (j*4) + (i*(CPs_t*CPs_u)) ].transform.position);
					connectors.Add (connection);
				}
			}
		}
	}
	
	void MoveConnectors(){
		int i, j, k;
		for (i = 0; i < CPs_s-1; i++) {
			for (j = 0; j < CPs_t; j++) {
				for (k = 0; k < CPs_u; k++) {
					LineRenderer lr = connectors[k + (j*4) + (i*(CPs_t*CPs_u))].GetComponent<LineRenderer>();
					lr.SetPosition(0,controlPoints[k + (j*4) + (i*(CPs_t*CPs_u))].transform.position);
					lr.SetPosition(1,controlPoints[k + (j*4) + ((i+1)*(CPs_t*CPs_u))].transform.position);
				}
			}
		}
		
		for (i = 0; i < CPs_s; i++) {
			for (j = 0; j < CPs_t-1; j++) {
				for (k = 0; k < CPs_u; k++) {
					LineRenderer lr = connectors [k + (j*4) + (i*(CPs_s*CPs_u)) + ((CPs_s-1)*CPs_t*CPs_u)].GetComponent<LineRenderer> ();
					lr.SetPosition (0, controlPoints [k + (j*4) + (i*(CPs_t*CPs_u))].transform.position);
					lr.SetPosition (1, controlPoints [k + ((j+1)*4) + (i*(CPs_t*CPs_u))].transform.position);
				}
			}
		}

		for (i = 0; i < CPs_s; i++) {
			for (j = 0; j < CPs_t; j++) {
				for (k = 0; k < CPs_u-1; k++) {
					//(k + (j* inner loop) + (i * outer loop) * (previousdistance)
					LineRenderer lr = connectors [(k + (j*(CPs_u-1)) + (i*(CPs_s*CPs_t)) + ((CPs_s-1)*CPs_t*CPs_u) + (CPs_s*(CPs_t-1)*CPs_u))].GetComponent<LineRenderer> ();
					lr.SetPosition (0, controlPoints [k + (j*4) + (i*(CPs_t*CPs_u))].transform.position);
					lr.SetPosition (1, controlPoints [(k+1) + (j*4) + (i*(CPs_t*CPs_u))].transform.position);
				}
			}
		}
	}

	Vector3 EvalArea(int index){
		CreateBernsteinCoefficients (index);

		Vector3 point = new Vector3 (0,0,0);
		
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 4; j++) {
				for (int k = 0; k < 4; k++) {
					point += controlPoints[k+(j*4)+ (i*4*4)].transform.localPosition * (Bs[i]*Bt[j]*Bu[k]);
				}
			}
		}
		
		return point;
	}
}