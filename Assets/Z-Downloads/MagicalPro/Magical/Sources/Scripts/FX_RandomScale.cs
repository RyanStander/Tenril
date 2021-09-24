using UnityEngine;
using System.Collections;

namespace MagicalFX
{
	public class FX_RandomScale : MonoBehaviour
	{
		public bool Blend = false;
		public float BlendSpeed = 0.5f;
		public float ScaleMin = 0;
		public float ScaleMax = 1;
		private Vector3 scaleTarget;
		
		void Start ()
		{
			scaleTarget = this.transform.localScale * Random.Range (ScaleMin, ScaleMax);
			if(!Blend){
				this.transform.localScale = scaleTarget;
			}else{
				this.transform.localScale = scaleTarget * 0.2f;	
			}
		}
	
		// Update is called once per frame
		void Update ()
		{
			if(Blend){
				this.transform.localScale = Vector3.Lerp(this.transform.localScale,scaleTarget,0.5f);
			}
		}
	}
}