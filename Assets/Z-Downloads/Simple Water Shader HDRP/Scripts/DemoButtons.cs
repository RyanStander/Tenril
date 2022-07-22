using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoButtons : MonoBehaviour
{

	public GameObject CameraA;
	public GameObject CameraB;
	public GameObject CameraC;
	public GameObject CameraD;

	void Start()
	{
		CameraA.SetActive(false);
		CameraB.SetActive(false);
		CameraC.SetActive(true);
		CameraD.SetActive(false);
	}

	void Update()
	{
		
	}

	public void XenCamA(){
		CameraA.SetActive(true);
		CameraB.SetActive(false);
		CameraC.SetActive(false);
		CameraD.SetActive(false);
	}

	public void XenCamB(){
		CameraB.SetActive(true);
		CameraA.SetActive(false);
		CameraC.SetActive(false);
		CameraD.SetActive(false);
	}

	public void XenCamC(){
		CameraC.SetActive(true);
		CameraB.SetActive(false);
		CameraA.SetActive(false);
		CameraD.SetActive(false);
	}

	public void XenCamD(){
		CameraD.SetActive(true);
		CameraC.SetActive(false);
		CameraB.SetActive(false);
		CameraA.SetActive(false);
	}
}
