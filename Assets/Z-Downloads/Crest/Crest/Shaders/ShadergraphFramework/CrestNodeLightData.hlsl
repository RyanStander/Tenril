// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

#include "../OceanGlobals.hlsl"

// Based on tutorial: https://connect.unity.com/p/adding-your-own-hlsl-code-to-shader-graph-the-custom-function-node

#include "OceanGraphConstants.hlsl"

#if CREST_URP
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

void CrestNodeLightData_half
(
	out half3 o_direction,
	out half3 o_colour
)
{
#ifdef SHADERGRAPH_PREVIEW
	// Hardcoded data, used for the preview shader inside the graph where light functions are not available.
	o_direction = -normalize(float3(-0.5, 0.5, -0.5));
	o_colour = float3(1.0, 1.0, 1.0);
#else
#if CREST_HDRP
	// Manually drive these in HDRP as I don't think there is a nicer way to do this yet.
	o_direction = _PrimaryLightDirection;
	o_colour = _PrimaryLightIntensity;
#else
	// Actual light data from the pipeline.
	Light light = GetMainLight();
	o_direction = light.direction;
	o_colour = light.color;
#endif // CREST_HDRP
#endif // SHADERGRAPH_PREVIEW
}
