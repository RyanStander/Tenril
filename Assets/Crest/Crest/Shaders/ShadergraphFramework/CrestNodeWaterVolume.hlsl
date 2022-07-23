// Crest Ocean System

// Copyright 2021 Wave Harmonic Ltd

#include "OceanGraphConstants.hlsl"
#include "../OceanConstants.hlsl"
#include "../OceanShaderHelpers.hlsl"
#include "../Helpers/WaterVolume.hlsl"

SamplerState WaterVolume_point_clamp_sampler;

void CrestNodeWaterVolume_float
(
	in const float i_clip,
	in const float2 i_positionNDC,
	in const float i_rawPixelZ,
	out float o_clip
)
{
	o_clip = i_clip;

#if CREST_WATER_VOLUME
	float2 positionNDC = i_positionNDC;

#if CREST_HDRP
	// Support RTHandle scaling for HDRP.
	positionNDC *= _RTHandleScale.xy;
#endif

#if CREST_WATER_VOLUME_HAS_BACKFACE
	// Discard ocean after volume or when not on pixel.
	float rawBackFaceZ = SAMPLE_DEPTH_TEXTURE_X(_CrestWaterVolumeBackFaceTexture, WaterVolume_point_clamp_sampler, positionNDC.xy);
	if (rawBackFaceZ == 0.0 || rawBackFaceZ > i_rawPixelZ)
	{
		o_clip = -1.0;
		return;
	}
#endif // CREST_WATER_VOLUME_VOLUME

	// Discard ocean before volume.
	float rawFrontFaceZ = SAMPLE_DEPTH_TEXTURE_X(_CrestWaterVolumeFrontFaceTexture, WaterVolume_point_clamp_sampler, positionNDC.xy);
	if (rawFrontFaceZ > 0.0 && rawFrontFaceZ < i_rawPixelZ)
	{
		o_clip = -1.0;
		return;
	}

#if CREST_WATER_VOLUME_2D
	// Discard ocean when plane is not in view.
	if (rawFrontFaceZ == 0.0)
	{
		o_clip = -1.0;
		return;
	}
#endif // CREST_WATER_VOLUME_2D
#endif // CREST_WATER_VOLUME
}
