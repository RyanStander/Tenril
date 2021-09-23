using UnityEngine;
using System.Collections;

namespace MagicalFX
{
	public class FX_ElectroLine : MonoBehaviour
	{
		public LineRenderer LineRender;
		public bool RayCast;
		public float Length = 300;
		public Transform StartObject, EndObject;
		public Vector3 EndPosition;
		public Vector3 StartPosition;
		public float DistancePerSegment = 0.5f;
		public float Noise = 0.5f;
		public float NoiseInterval = 0.05f;
		public bool Blending = true;
		private Vector3[] vertexTemps, vertexTempsTarget, vertexTempsCurrent;
		private int vertexCount = 0;
		private float noiseIntervalTemp;
		public bool FixRotation = false;
		public bool Normal;
		public bool ParentFXstart = true;
		public bool ParentFXend = true;
		public GameObject FXStart, FXEnd;
		private GameObject fxStart, fxEnd;
		public bool KeepConnect = false;
		
		void Start ()
		{
			if (StartObject) {
				StartPosition = StartObject.transform.position;
			}
			if (EndObject) {
				EndPosition = EndObject.transform.position;	
			}
			
			if (RayCast) {
				StartPosition = this.transform.position;
				Ray ray = new Ray (this.transform.position, this.transform.forward);	
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, Length)) {
					EndPosition = hit.point;
					vertexCount = (int)(hit.distance / DistancePerSegment);
				}
			} else {
				vertexCount = (int)(Vector3.Distance (StartPosition, EndPosition) / DistancePerSegment);	
			}
			
			if (LineRender == null) {
				LineRender = this.GetComponent<LineRenderer> ();	
				LineRender.positionCount = (vertexCount);
				vertexTemps = new Vector3[vertexCount];
				vertexTempsTarget = new Vector3[vertexCount];
				vertexTempsCurrent = new Vector3[vertexCount];
				for (int i=0; i<vertexCount; i++) {
					vertexTemps [i] = StartPosition + ((this.transform.forward * DistancePerSegment) * i);
					
					if (i == 0) {
						if (StartObject) {
							
							vertexTemps [i] = StartPosition;
						}
					}
					if (i == vertexCount - 1) {
						if (EndObject) {
						
							vertexTemps [i] = EndPosition;
						}
					}
					
					
					vertexTempsTarget [i] = vertexTemps [i];
					vertexTempsCurrent [i] = vertexTemps [i];
					LineRender.SetPosition (i, vertexTemps [i]);
					if (!EndObject) {
						if (i == vertexCount - 1)
							EndPosition = vertexTemps [i];
					}
				}
			}
			
			

			if (FXStart != null) {
				Quaternion rotate = this.transform.rotation;
				if (!FixRotation)
					rotate = FXStart.transform.rotation;
			
				fxStart = (GameObject)GameObject.Instantiate (FXStart, StartPosition, rotate);
				if (Normal)
					fxStart.transform.forward = this.transform.forward;
				
				if(ParentFXstart)
				fxStart.transform.parent = this.transform;

			}
			
			if (FXEnd != null) {
				Quaternion rotate = this.transform.rotation;
				if (!FixRotation)
					rotate = FXEnd.transform.rotation;
			
				fxEnd = (GameObject)GameObject.Instantiate (FXEnd, EndPosition, rotate);
				if (Normal)
					fxEnd.transform.forward = this.transform.forward;
				
				if(ParentFXend)
				fxEnd.transform.parent = this.transform;
				
			}
		}
		
		void UpdatePosition ()
		{
			this.transform.forward = (EndPosition - StartPosition).normalized;
			for (int i=0; i<vertexCount; i++) {
				vertexTemps [i] = StartPosition + ((this.transform.forward * DistancePerSegment) * i);
			}

			if (fxStart)
				fxStart.transform.position = StartPosition;
			if (fxEnd)
				fxEnd.transform.position = EndPosition;
		}
	
		void Update ()
		{
			if (StartObject) {
				StartPosition = StartObject.transform.position;
			}
			if (EndObject) {
				EndPosition = EndObject.transform.position;	
			}
			if (KeepConnect) {
				UpdatePosition ();	
			}
			
			if (LineRender == null)
				return;
			
			if (Time.time > noiseIntervalTemp + NoiseInterval) {
				noiseIntervalTemp = Time.time;
				if (Noise > 0) {
					for (int i=0; i<vertexCount; i++) {
						Vector3 up = new Vector3 (Random.Range (-100, 100) * Noise * this.transform.up.x, Random.Range (-100, 100) * Noise * this.transform.up.y, Random.Range (-100, 100) * Noise * this.transform.up.z);
						Vector3 right = new Vector3 (Random.Range (-100, 100) * Noise * this.transform.right.x, Random.Range (-100, 100) * Noise * this.transform.right.y, Random.Range (-100, 100) * Noise * this.transform.right.z);
						vertexTempsTarget [i] = vertexTemps [i] + right + up;
						if (!Blending) {
							LineRender.SetPosition (i, vertexTemps [i] + right + up);
						}
					}	
				}
			}
			if (Blending) {
				for (int i=0; i<vertexCount; i++) {
					
					if (i == 0 || i == vertexCount - 1)
						continue;
					
					vertexTempsCurrent [i] = Vector3.Lerp (vertexTempsCurrent [i], vertexTempsTarget [i], 0.5f);
					LineRender.SetPosition (i, vertexTempsCurrent [i]);
				}
			}
		}
	}
}
