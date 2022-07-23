// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

#include "OceanGraphConstants.hlsl"
#include "../OceanConstants.hlsl"
#include "../OceanGlobals.hlsl"
#include "../OceanInputsDriven.hlsl"
#include "../OceanHelpersNew.hlsl"

void CrestNodeSampleClipSurfaceData_float
(
	in const float2 i_positionXZWS,
	in const float i_lodAlpha,
	in const float3 i_oceanPosScale0,
	in const float3 i_oceanPosScale1,
	in const float4 i_oceanParams0,
	in const float4 i_oceanParams1,
	in const float i_sliceIndex0,
	out float o_clipSurface
)
{
	o_clipSurface = 0.0;

	uint slice0; uint slice1; float alpha;
	PosToSliceIndices(i_positionXZWS, 0.0, _CrestCascadeData[0]._scale, slice0, slice1, alpha);

	const CascadeParams cascadeData0 = _CrestCascadeData[slice0];
	const CascadeParams cascadeData1 = _CrestCascadeData[slice1];
	const float weight0 = (1.0 - alpha) * cascadeData0._weight;
	const float weight1 = (1.0 - weight0) * cascadeData1._weight;

	if (weight0 > 0.001)
	{
		const float3 uv = WorldToUV(i_positionXZWS, cascadeData0, slice0);
		SampleClip(_LD_TexArray_ClipSurface, uv, weight0, o_clipSurface);
	}
	if (weight1 > 0.001)
	{
		const float3 uv = WorldToUV(i_positionXZWS, cascadeData1, slice1);
		SampleClip(_LD_TexArray_ClipSurface, uv, weight1, o_clipSurface);
	}

	o_clipSurface = lerp(_CrestClipByDefault, o_clipSurface, weight0 + weight1);

	// 0.5 mip bias for LOD blending and texel resolution correction. This will help to tighten and smooth clipped edges.
	// We set to 2 or 0 to work correctly with other alpha inputs like feathering.
	o_clipSurface = o_clipSurface > 0.5 ? 2.0 : 0.0;
}
