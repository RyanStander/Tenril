using UnityEngine;
using System.Collections;

namespace MagicalFX
{
	public class FX_LifeTime : MonoBehaviour
	{

		public float LifeTime = 3;
		public float FadeTime = 1;
		public GameObject SpawnAfterDead;
		private float timeTemp;
		public ParticleSystem[] Particles;
		public FX_FadeToGround[] Faders;
		public Renderer[] Renderers;
		public Light[] Lights;
		FX_Tentacle ten;

		void Awake ()
		{
			ten = this.GetComponent<FX_Tentacle> ();
			if (Particles.Length <= 0) {
				Particles = (ParticleSystem[])this.transform.GetComponentsInChildren <ParticleSystem> ();
			}

			if (Faders.Length <= 0) {
				Faders = (FX_FadeToGround[])this.transform.GetComponentsInChildren <FX_FadeToGround> ();
			}

			if (Lights.Length <= 0) {
				Lights = (Light[])this.transform.GetComponentsInChildren <Light> ();
			}

			for (int i = 0; i < Particles.Length; i++) {
				Particles [i].Pause ();
				Particles [i].Stop ();
				Particles [i].Clear ();
			}
			for (int i = 0; i < Particles.Length; i++) {
				var main = Particles [i].main;
				main.duration = LifeTime - FadeTime;
				main.loop = false;	
			}
		}

		MaterialPropertyBlock block;

		void Start ()
		{
			block = new MaterialPropertyBlock ();
			timeTemp = Time.time;

			if (SpawnAfterDead == null) {
				GameObject.Destroy (this.gameObject, LifeTime);
			}

			for (int i = 0; i < Particles.Length; i++) {
				Particles [i].Play ();
			}

		}

		void Update ()
		{
			float timeleft = (timeTemp + LifeTime) - Time.time;
			float delta = Mathf.Clamp (timeleft, 0, 1);

			for (int i = 0; i < Renderers.Length; i++) {
				Renderers [i].GetPropertyBlock (block);
				block.SetFloat ("_Alpha", delta);
				block.SetFloat ("_Cutoff", 1 - delta);
				Renderers [i].SetPropertyBlock (block);
			}

			for (int i = 0; i < Lights.Length; i++) {
				if (Lights [i])
					Lights [i].intensity *= delta;
			}
			if (Time.time > timeTemp + LifeTime - 1) {
				if (ten)
					ten.End (15);

				for (int i = 0; i < Faders.Length; i++) {
					Faders [i].OnDead ();
				}
			}
			if (Time.time > timeTemp + LifeTime) {

				if (SpawnAfterDead != null) {
					GameObject.Destroy (this.gameObject);
					GameObject.Instantiate (SpawnAfterDead, this.transform.position, SpawnAfterDead.transform.rotation);
				}
			}
		}
	}
}