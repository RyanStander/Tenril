// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

#include "OceanGraphConstants.hlsl"
#include "../OceanGlobals.hlsl"
#include "../ShaderLibrary/Texture.hlsl"

half WhiteFoamTexture
(
	in const WaveHarmonic::Crest::TiledTexture i_texture,
	in const half i_foam,
	in const half i_feather,
	in const float2 i_worldXZ0,
	in const float2 i_worldXZ1,
	in const float2 i_texelOffset,
	in const float i_texelSize0,
	in const float i_texelSize1,
	in const half i_lodVal
)
{
	half ft = lerp
	(
		i_texture.Sample((1.25 * (i_worldXZ0 + i_texelOffset) + _CrestTime / 10.0) / (4.0 * i_texelSize0 * i_texture._scale)).r,
		i_texture.Sample((1.25 * (i_worldXZ1 + i_texelOffset) + _CrestTime / 10.0) / (4.0 * i_texelSize1 * i_texture._scale)).r,
		i_lodVal
	);

	// black point fade
	half result = saturate(1.0 - i_foam);
	return smoothstep(result, result + i_feather, ft);
}

void CrestNodeFoam
(
	in const Texture2D<float4> i_texture,
	in const half2 i_texelSize,
	in const half i_scale,
	in const half i_feather,
	in const half i_albedoIntensity,
	in const half i_emissiveIntensity,
	in const half i_foamSmoothness,
	in const half i_normalStrength,
	in const float4 i_oceanParams0,
	in const float4 i_oceanParams1,
	in const half i_foam,
	in const half i_lodVal,
	in const float2 i_worldXZUndisplaced,
	in const float i_pixelZ,
	in const half3 i_normalTS,
	in const half3 i_emission,
	in const float i_smoothness,
	in const float2 i_flow,
	out half3 o_albedo,
	out half3 o_n,
	out half3 o_emission,
	out float o_smoothness
)
{
#ifdef SHADERGRAPH_PREVIEW
	// Samplers are not defined in shader graph. Silence errors.
	SamplerState sampler_TextureFoam = LODData_linear_clamp_sampler;
	float4 _TextureFoam_TexelSize = float4(i_texelSize, 0.0, 0.0);
#endif

	const WaveHarmonic::Crest::TiledTexture foamTexture =
		WaveHarmonic::Crest::TiledTexture::Make(i_texture, sampler_TextureFoam, _TextureFoam_TexelSize, i_scale);

	float2 worldXZ0 = i_worldXZUndisplaced;
	float2 worldXZ1 = i_worldXZUndisplaced;

#if CREST_FLOATING_ORIGIN
	// Apply tiled floating origin offset. Only needed if:
	//  - _FoamScale is a non integer value
	//  - _FoamScale is over 48
	worldXZ0 -= foamTexture.FloatingOriginOffset(i_oceanParams0);
	worldXZ1 -= foamTexture.FloatingOriginOffset(i_oceanParams1);
#endif // CREST_FLOATING_ORIGIN

#if CREST_FLOW_ON
	// Apply flow offset.
	worldXZ0 -= i_flow;
	worldXZ1 -= i_flow;
#endif // _FLOW_ON

	half whiteFoam = WhiteFoamTexture(foamTexture, i_foam, i_feather, worldXZ0, worldXZ1, (float2)0.0, i_oceanParams0.x, i_oceanParams1.x, i_lodVal);

	o_albedo = saturate(whiteFoam * i_albedoIntensity);
	o_emission = lerp(i_emission, i_emissiveIntensity, whiteFoam);
	o_smoothness = lerp(i_smoothness, i_foamSmoothness, whiteFoam);

	//#if _FOAM3DLIGHTING_ON
	float2 dd = float2(0.25 * i_pixelZ * foamTexture._texel, 0.0);
	half whiteFoam_x = WhiteFoamTexture(foamTexture, i_foam, i_feather, worldXZ0, worldXZ1, dd.xy, i_oceanParams0.x, i_oceanParams1.x, i_lodVal);
	half whiteFoam_z = WhiteFoamTexture(foamTexture, i_foam, i_feather, worldXZ0, worldXZ1, dd.yx, i_oceanParams0.x, i_oceanParams1.x, i_lodVal);

	// Compute a foam normal - manually push in derivatives. If i used blend smooths all the normals towards straight up when there is no foam.
	o_n = i_normalTS;
	o_n.xy -= i_normalStrength * half2(whiteFoam_x - whiteFoam, whiteFoam_z - whiteFoam);
	o_n = normalize(o_n);
	//#endif // _FOAM3DLIGHTING_ON
}

void CrestNodeFoam_half
(
	in const Texture2D<float4> i_texture,
	in const half2 i_texelSize,
	in const half i_scale,
	in const half i_feather,
	in const half i_albedoIntensity,
	in const half i_emissiveIntensity,
	in const half i_foamSmoothness,
	in const half i_normalStrength,
	in const float4 i_oceanParams0,
	in const float4 i_oceanParams1,
	in const half i_foam,
	in const half i_lodVal,
	in const float2 i_worldXZUndisplaced,
	in const float i_pixelZ,
	in const half3 i_normalTS,
	in const half3 i_emission,
	in const float i_smoothness,
	in const float2 i_flow,
	out half3 o_albedo,
	out half3 o_n,
	out half3 o_emission,
	out float o_smoothness
)
{
#if CREST_FLOW_ON
	const float half_period = 1.0;
	const float period = half_period * 2.0;
	float sample1_offset = fmod(_CrestTime, period);
	float sample1_weight = sample1_offset / half_period;
	if (sample1_weight > 1.0) sample1_weight = 2.0 - sample1_weight;
	float sample2_offset = fmod(_CrestTime + half_period, period);
	float sample2_weight = 1.0 - sample1_weight;
#endif

	half3 albedo = (half3)0.0;
	half3 n = (half3)0.0;
	half3 emission = (half3)0.0;
	float smoothness = 0.0;

	CrestNodeFoam
	(
		i_texture,
		i_texelSize,
		i_scale,
		i_feather,
		i_albedoIntensity,
		i_emissiveIntensity,
		i_foamSmoothness,
		i_normalStrength,
		i_oceanParams0,
		i_oceanParams1,
		i_foam,
		i_lodVal,
		i_worldXZUndisplaced,
		i_pixelZ,
		i_normalTS,
		i_emission,
		i_smoothness,
		i_flow
#if CREST_FLOW_ON
		* sample1_offset
#endif
		,
		albedo,
		n,
		emission,
		smoothness
	);

	o_albedo = albedo;
	o_n = n;
	o_emission = emission;
	o_smoothness = smoothness;

#if CREST_FLOW_ON
	o_albedo *= sample1_weight;
	o_n *= sample1_weight;
	o_emission *= sample1_weight;
	o_smoothness *= sample1_weight;

	CrestNodeFoam
	(
		i_texture,
		i_texelSize,
		i_scale,
		i_feather,
		i_albedoIntensity,
		i_emissiveIntensity,
		i_foamSmoothness,
		i_normalStrength,
		i_oceanParams0,
		i_oceanParams1,
		i_foam,
		i_lodVal,
		i_worldXZUndisplaced,
		i_pixelZ,
		i_normalTS,
		i_emission,
		i_smoothness,
		i_flow * sample2_offset,
		albedo,
		n,
		emission,
		smoothness
	);

	o_albedo += albedo * sample2_weight;
	o_n += n * sample2_weight;
	o_emission += emission * sample2_weight;
	o_smoothness += smoothness * sample2_weight;
#endif
}
