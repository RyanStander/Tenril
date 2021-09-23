using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicalFX
{
	public class FX_Tentacle : MonoBehaviour
	{

		public TrailRenderer Trail;
		public int SubNumber = 5;
		public int Number = 10;
		public float Speed = 10;
		public float Damping = 10;
		public Vector3 Direction;
		public Vector3 SpreadMin;
		public Vector3 SpreadMax;
		public Vector3 SpreadSpawn;
		public Vector3 SubSpread;
		public float Duration = 10;
		public float SubDuration = 10;
		public float SubRate = 10;
		public float GravityMult = 0;
		public bool ToGround;



		private TentacleInstance[] trails;
		public List<TentacleInstance> subTrails;


		float timeTmp;

		void Start ()
		{
			subTrails = new List<TentacleInstance> ();
			timeTmp = Time.time;

			trails = new TentacleInstance[Number];
			for (int i = 0; i < trails.Length; i++) {

				Vector3 spread = new Vector3 (Random.Range (-SpreadSpawn.x, SpreadSpawn.x), Random.Range (-SpreadSpawn.y, SpreadSpawn.y), Random.Range (-SpreadSpawn.z, SpreadSpawn.z)) * 0.01f;

				trails [i] = new TentacleInstance ();
				trails [i].Trail = (TrailRenderer)GameObject.Instantiate (Trail, this.transform.position + spread, Quaternion.identity);
				trails [i].Trail.transform.forward = Vector3.up;
				trails [i].TimeTmp = Time.time;
				trails [i].TimeSubTmp = Time.time;
				trails [i].SubNumber = SubNumber;
				trails [i].SpawnRate = SubRate;
				trails [i].Trail.transform.parent = this.transform;
			}
		}

		void AddSubtrail (Vector3 position)
		{
			TentacleInstance tr = new TentacleInstance ();
			tr.Trail = (TrailRenderer)GameObject.Instantiate (Trail, position, Quaternion.identity);
			tr.Trail.endWidth *= 0.5f;
			tr.Trail.startWidth *= 0.5f;

			tr.Trail.transform.parent = this.transform;
			tr.Trail.transform.forward = Vector3.up;
			tr.TimeTmp = Time.time;
			tr.Duration = Random.Range (0, SubDuration);
			subTrails.Add (tr);
		}

		public void End (float speed)
		{
			for (int i = 0; i < trails.Length; i++) {
				Color startcolor = trails [i].Trail.startColor;
				Color endColor = trails [i].Trail.endColor;
				startcolor.a = Mathf.Lerp (startcolor.a, 0, speed * Time.deltaTime);
				endColor.a = startcolor.a;
				trails [i].Trail.startColor = startcolor;
				trails [i].Trail.endColor = endColor;
			}
			foreach (TentacleInstance tr in subTrails) {
				Color startcolor = tr.Trail.startColor;
				Color endColor = tr.Trail.endColor;
				startcolor.a = Mathf.Lerp (startcolor.a, 0, speed * Time.deltaTime);
				endColor.a = startcolor.a;

				tr.Trail.startColor = startcolor;
				tr.Trail.endColor = endColor;
			}
		}

		void Update ()
		{
			if (Time.time < timeTmp + Duration) {

				for (int i = 0; i < trails.Length; i++) {
					if (trails [i] != null) {
						Vector3 noise = new Vector3 (Random.Range (SpreadMin.x, SpreadMax.x), Random.Range (SpreadMin.y, SpreadMax.y), Random.Range (SpreadMin.z, SpreadMax.z));

						Quaternion rotationToTarget = Quaternion.LookRotation (((this.transform.position + Direction + noise) - trails [i].Trail.transform.position).normalized);
						trails [i].Trail.transform.rotation = Quaternion.Lerp (trails [i].Trail.transform.rotation, rotationToTarget, Damping * Time.deltaTime);
						trails [i].Trail.transform.position += trails [i].Trail.transform.forward * Speed * Time.deltaTime;
						trails [i].Trail.transform.position += Vector3.up * GravityMult * Time.deltaTime;
						if (subTrails.Count < trails [i].SubNumber) {
							if (Time.time > trails [i].TimeSubTmp + trails [i].SpawnRate) {
								trails [i].TimeSubTmp = Time.time;
								AddSubtrail (trails [i].Trail.transform.position);
							}
						}
					}
				}
				foreach (TentacleInstance tr in subTrails) {
					if (Time.time < tr.TimeTmp + tr.Duration) {
						Vector3 noise = new Vector3 (Random.Range (-SubSpread.x, SubSpread.x), Random.Range (-SubSpread.y, SubSpread.y), Random.Range (-SubSpread.z, SubSpread.z));
						Quaternion rotationToTarget = Quaternion.LookRotation (((this.transform.position + Direction + noise) - tr.Trail.transform.position).normalized);
						tr.Trail.transform.rotation = Quaternion.Lerp (tr.Trail.transform.rotation, rotationToTarget, Damping * Time.deltaTime);
						tr.Trail.transform.position += tr.Trail.transform.forward * Speed * 0.5f * Time.deltaTime;
						tr.Trail.transform.position += Vector3.up * GravityMult * Time.deltaTime;
					}
				}
			} else {
				if (ToGround) {
					for (int i = 0; i < trails.Length; i++) {
						if (trails [i] != null) {
							trails [i].Trail.transform.position += trails [i].Trail.transform.forward * Speed * 0.3f * Time.deltaTime;
							trails [i].Trail.transform.position += -Vector3.up * Speed * 0.9f * Time.deltaTime;
						}
					}
					foreach (TentacleInstance tr in subTrails) {
						tr.Trail.transform.position += tr.Trail.transform.forward * Speed * 0.3f * Time.deltaTime;
						tr.Trail.transform.position += -Vector3.up * Speed * 0.9f * Time.deltaTime;
					}
				}
			}
		}
	}

	public class TentacleInstance
	{
		public TrailRenderer Trail;
		public float TimeTmp;
		public float Duration;
		public float SpawnRate;
		public float TimeSubTmp;
		public float SubNumber;
	}
}