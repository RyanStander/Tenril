// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

#include "OceanGraphConstants.hlsl"
#include "../OceanGlobals.hlsl"
#include "../ShaderLibrary/Texture.hlsl"

struct FoamWorldXZ
{
	float2 displaced1;
	float2 displaced0;
	float2 undisplaced0;
	float2 undisplaced1;
};

half FoamBubblesTexture
(
	in const WaveHarmonic::Crest::TiledTexture i_texture,
	in const FoamWorldXZ i_worldXZ,
	in const half i_parallax,
	in const half3 i_normalWS,
	in const half3 i_view,
	in const float i_texelSize0,
	in const float i_texelSize1,
	in const half i_lodVal
)
{
	float scale = 0.74;
#if CREST_FLOATING_ORIGIN
	// This value causes no pops.
	scale = 0.75;
#endif
	float2 windDir = float2(0.866, 0.5);
	float2 foamUVBubbles0 = scale * (lerp(i_worldXZ.undisplaced0, i_worldXZ.displaced0, 0.7) + 0.5 * _CrestTime * windDir) / i_texture._scale + 0.125 * i_normalWS.xz;
	float2 foamUVBubbles1 = scale * (lerp(i_worldXZ.undisplaced1, i_worldXZ.displaced1, 0.7) + 0.5 * _CrestTime * windDir) / i_texture._scale + 0.125 * i_normalWS.xz;
	float2 parallaxOffset = -i_parallax * i_view.xz / dot(i_normalWS, i_view);
	half ft = lerp
	(
		i_texture.SampleLevel((foamUVBubbles0 + parallaxOffset) / (4.0 * i_texelSize0), 3.0).r,
		i_texture.SampleLevel((foamUVBubbles1 + parallaxOffset) / (4.0 * i_texelSize1), 3.0).r,
		i_lodVal
	);

	return ft;
}

void CrestNodeFoamBubbles_half
(
	in const half3 i_color,
	in const half i_parallax,
	in const half i_coverage,
	in const Texture2D<float4> i_texture,
	in const half2 i_texelSize,
	in const half i_scale,
	in const half i_foamAmount,
	in const half3 i_normalTS,
	in const float4 i_oceanParams0,
	in const float4 i_oceanParams1,
	in const float2 i_worldXZ,
	in const float2 i_worldXZUndisplaced,
	in const half i_lodVal,
	in const half3 i_view,
	in const half3 i_ambientLight,
	in const half2 i_flow,
	out half3 o_color
)
{
#if CREST_FLOW_ON
	const float half_period = 1;
	const float period = half_period * 2;
	float sample1_offset = fmod(_CrestTime, period);
	float sample1_weight = sample1_offset / half_period;
	if (sample1_weight > 1.0) sample1_weight = 2.0 - sample1_weight;
	float sample2_offset = fmod(_CrestTime + half_period, period);
	float sample2_weight = 1.0 - sample1_weight;
#endif

#ifdef SHADERGRAPH_PREVIEW
	// Samplers are not defined in shader graph. Silence errors.
	SamplerState sampler_TextureFoam = LODData_linear_clamp_sampler;
	float4 _TextureFoam_TexelSize = float4(i_texelSize, 0.0, 0.0);
#endif

	// Convert from texture space.
	float3 normalWS = i_normalTS.xzy;

	const WaveHarmonic::Crest::TiledTexture foamTexture =
		WaveHarmonic::Crest::TiledTexture::Make(i_texture, sampler_TextureFoam, _TextureFoam_TexelSize, i_scale);

	FoamWorldXZ worldXZ;
	worldXZ.displaced0   = i_worldXZ;
	worldXZ.displaced1   = i_worldXZ;
	worldXZ.undisplaced0 = i_worldXZUndisplaced;
	worldXZ.undisplaced1 = i_worldXZUndisplaced;

#if CREST_FLOATING_ORIGIN
	// Apply tiled floating origin offset. Only needed if:
	//  - _FoamScale is a non integer value
	//  - _FoamScale is over 48
	worldXZ.displaced0   -= foamTexture.FloatingOriginOffset(i_oceanParams0);
	worldXZ.displaced1   -= foamTexture.FloatingOriginOffset(i_oceanParams1);
	worldXZ.undisplaced0 -= foamTexture.FloatingOriginOffset(i_oceanParams0);
	worldXZ.undisplaced1 -= foamTexture.FloatingOriginOffset(i_oceanParams1);
#endif // CREST_FLOATING_ORIGIN

#if CREST_FLOW_ON
	FoamWorldXZ worldXZ2 = worldXZ;
	// Apply flow offset.
	worldXZ.undisplaced0 -= i_flow * sample1_offset;
	worldXZ.undisplaced1 -= i_flow * sample1_offset;
	worldXZ2.undisplaced0 -= i_flow * sample2_offset;
	worldXZ2.undisplaced1 -= i_flow * sample2_offset;
#endif // _FLOW_ON

	// Additive underwater foam - use same foam texture but add mip bias to blur for free
	half3 bubbleFoamTexValue = (half3)FoamBubblesTexture
	(
		foamTexture,
		worldXZ,
		i_parallax,
		normalWS,
		i_view,
		i_oceanParams0.x,
		i_oceanParams1.x,
		i_lodVal
	);

	o_color = bubbleFoamTexValue;

#if CREST_FLOW_ON
	o_color *= sample1_weight;
	bubbleFoamTexValue = (half3)FoamBubblesTexture
	(
		foamTexture,
		worldXZ2,
		i_parallax,
		normalWS,
		i_view,
		i_oceanParams0.x,
		i_oceanParams1.x,
		i_lodVal
	);
	o_color += bubbleFoamTexValue * sample2_weight;
#endif

	// Finally, apply colour, coverage and lighting
	 o_color *= i_color.rgb * saturate(i_foamAmount * i_coverage) * i_ambientLight;
}
