// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

#ifndef CREST_OCEAN_NORMAL_MAPPING_INCLUDED
#define CREST_OCEAN_NORMAL_MAPPING_INCLUDED

#include "../OceanGlobals.hlsl"
#include "../ShaderLibrary/Texture.hlsl"

half2 SampleNormalMaps
(
	in const WaveHarmonic::Crest::TiledTexture i_texture,
	in const float2 i_worldXZUndisplaced,
	in const float2 i_offset,
	in const half i_normalsStrength,
	in const float i_lodAlpha,
	in const CascadeParams i_cascadeData,
	in const PerCascadeInstanceData i_instanceData
)
{
	const float lodDataGridSize = i_cascadeData._texelWidth;
	float2 normalScrollSpeeds = i_instanceData._normalScrollSpeeds;

	float2 worldXZUndisplaced = i_worldXZUndisplaced;

#if CREST_FLOATING_ORIGIN
	// Apply tiled floating origin offset. Always needed.
	worldXZUndisplaced -= i_texture.FloatingOriginOffset(i_cascadeData);
#endif

	worldXZUndisplaced -= i_offset;

	const float2 v0 = float2(0.94, 0.34), v1 = float2(-0.85, -0.53);
	float nstretch = i_texture._scale * lodDataGridSize; // normals scaled with geometry
	const float spdmulL = normalScrollSpeeds[0];
	half2 norm =
		UnpackNormal(i_texture.Sample((v0 * _CrestTime * spdmulL + worldXZUndisplaced) / nstretch)).xy +
		UnpackNormal(i_texture.Sample((v1 * _CrestTime * spdmulL + worldXZUndisplaced) / nstretch)).xy;

	// blend in next higher scale of normals to obtain continuity
	const float farNormalsWeight = i_instanceData._farNormalsWeight;
	const half nblend = i_lodAlpha * farNormalsWeight;
	if (nblend > 0.001)
	{
		// next lod level
		nstretch *= 2.0;
		const float spdmulH = normalScrollSpeeds[1];
		norm = lerp(norm,
			UnpackNormal(i_texture.Sample((v0 * _CrestTime * spdmulH + worldXZUndisplaced) / nstretch)).xy +
			UnpackNormal(i_texture.Sample((v1 * _CrestTime * spdmulH + worldXZUndisplaced) / nstretch)).xy,
			nblend);
	}

	// approximate combine of normals. would be better if normals applied in local frame.
	return i_normalsStrength * norm;
}

void ApplyNormalMapsWithFlow
(
	in const half2 i_flow,
	in const WaveHarmonic::Crest::TiledTexture i_texture,
	in const float2 i_worldXZUndisplaced,
	in const half i_normalsStrength,
	in const float i_lodAlpha,
	in const CascadeParams i_cascadeData,
	in const PerCascadeInstanceData i_instanceData,
	inout float3 io_n
)
{
	// When converting to Shader Graph, this code is already in the CrestFlow subgraph
	const float half_period = 1;
	const float period = half_period * 2;
	float sample1_offset = fmod(_CrestTime, period);
	float sample1_weight = sample1_offset / half_period;
	if (sample1_weight > 1.0) sample1_weight = 2.0 - sample1_weight;
	float sample2_offset = fmod(_CrestTime + half_period, period);
	float sample2_weight = 1.0 - sample1_weight;
	sample1_offset -= 0.5 * period;
	sample2_offset -= 0.5 * period;

	// In order to prevent flow from distorting the UVs too much,
	// we fade between two samples of normal maps so that for each
	// sample the UVs can be reset
	half2 io_n_1 = SampleNormalMaps(i_texture, i_worldXZUndisplaced, i_flow * sample1_offset, i_normalsStrength, i_lodAlpha, i_cascadeData, i_instanceData);
	half2 io_n_2 = SampleNormalMaps(i_texture, i_worldXZUndisplaced, i_flow * sample2_offset, i_normalsStrength, i_lodAlpha, i_cascadeData, i_instanceData);
	io_n.xz += sample1_weight * io_n_1;
	io_n.xz += sample2_weight * io_n_2;
}

#endif // CREST_OCEAN_NORMAL_MAPPING_INCLUDED
