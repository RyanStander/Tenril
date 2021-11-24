using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField]private Light directionalLight;
    [SerializeField] private LightingPreset preset;
    [SerializeField,Range(0,1)] private float dayNightSpeed=1;
    [Header("References")]
    [SerializeField, Range(0,24)] private float timeOfDay;

    private void Update()
    {
        //if no preset was given, do no perform any actions
        if (preset == null)
            return;

        //if in play mode, advance time
        if (Application.isPlaying)
        {
            timeOfDay += Time.deltaTime*dayNightSpeed;
            timeOfDay %= 24;
            UpdateLighting(timeOfDay / 24f);
        }
        //otherwise let use manually adjust it
        else
        {
            UpdateLighting(timeOfDay / 24f);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.ambientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);

        if (directionalLight!=null)
        {
            directionalLight.color = preset.directionalColor.Evaluate(timePercent);
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent*360)-90,-170,0));
        }
    }
    private void OnValidate()
    {
        //If light was set, exit
        if (directionalLight!=null)
            return;
        //if the rendersettings has a sun set, assign to that
        if (RenderSettings.sun!=null)
            directionalLight = RenderSettings.sun;
        else
        {
            //otherwise find all instances of light and look for a directional light and set too the first one
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type==LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }
}
