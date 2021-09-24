using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicalFX
{
	public class FX_FadeToGround : MonoBehaviour
	{

		public Vector3 Speed = -Vector3.up;
		public bool Down;

		public float ScaleSpeed = 0;
		public Vector3 Scale = Vector3.zero;

		void Start ()
		{
		}

		public void OnDead ()
		{
			Down = true;
		}

		void Update ()
		{
			if (Down) {
				this.transform.position += Speed * Time.deltaTime;

				this.transform.localScale = Vector3.Lerp (this.transform.localScale, Scale, ScaleSpeed * Time.deltaTime);
			}
		}
	}
}