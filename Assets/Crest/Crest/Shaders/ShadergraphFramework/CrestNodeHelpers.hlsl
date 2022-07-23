// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

float4 _CameraDepthTexture_TexelSize;

#include "OceanGraphConstants.hlsl"
#include "../OceanGlobals.hlsl"
#include "../OceanShaderHelpers.hlsl"

void CrestNodeLinearEyeDepth_float
(
	in const float i_rawDepth,
	out float o_linearDepth
)
{
	o_linearDepth = CrestLinearEyeDepth(i_rawDepth);
}

void CrestNodeMultiSampleDepth_float
(
	in const float i_rawDepth,
	in const float2 i_positionNDC,
	out float o_rawDepth
)
{
	o_rawDepth = CREST_MULTISAMPLE_SCENE_DEPTH(i_positionNDC, i_rawDepth);
}
