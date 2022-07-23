// Crest Ocean System

// Copyright 2022 Wave Harmonic Ltd

// NOTE: It is important that everything has a Crest prefix to avoid possible conflicts.

#ifndef SHADERGRAPH_PREVIEW

#include "../ShadergraphFramework/OceanGraphConstants.hlsl"

TEXTURE2D_X(_CrestOceanMaskTexture); SAMPLER(sampler_CrestOceanMaskTexture);
TEXTURE2D_X(_CrestWaterVolumeFrontFaceTexture); SAMPLER(sampler_CrestWaterVolumeFrontFaceTexture);
TEXTURE2D_X(_CrestWaterVolumeBackFaceTexture); SAMPLER(sampler_CrestWaterVolumeBackFaceTexture);

half3 _CrestDiffuse;
half3 _CrestDiffuseGrazing;

#if CREST_HDRP
#define CREST_SHADOWS_ON 1
#define CREST_SUBSURFACESCATTERING_ON 1
#endif

#if CREST_SHADOWS_ON
half3 _CrestDiffuseShadow;
#endif

#if CREST_SUBSURFACESCATTERING_ON
half3 _CrestSubSurfaceColour;
half _CrestSubSurfaceBase;
half _CrestSubSurfaceSun;
half _CrestSubSurfaceSunFallOff;
#endif

half3 _CrestAmbientLighting;
half4 _CrestDepthFogDensity;

#include "../OceanConstants.hlsl"
#include "../OceanGlobals.hlsl"
#include "../OceanInputsDriven.hlsl"
#include "../OceanShaderHelpers.hlsl"

// HLSL function.
half3 CrestScatterColour
(
	const half3 i_ambientLighting,
	const half3 i_lightCol,
	const half3 i_lightDir,
	const half3 i_view,
	const float i_shadow
)
{
	// Base colour.
	float v = abs(i_view.y);
	half3 col = lerp(_CrestDiffuse, _CrestDiffuseGrazing, 1. - pow(v, 1.0));

#if CREST_SHADOWS_ON
	col = lerp(_CrestDiffuseShadow, col, i_shadow);
#endif

#if CREST_SUBSURFACESCATTERING_ON
	{
		col *= i_ambientLighting + i_lightCol;

		// Approximate subsurface scattering - add light when surface faces viewer. Use geometry normal - don't need high freqs.
		half towardsSun = pow(max(0., dot(i_lightDir, -i_view)), _CrestSubSurfaceSunFallOff);
		half3 subsurface = (_CrestSubSurfaceBase + _CrestSubSurfaceSun * towardsSun) * _CrestSubSurfaceColour.rgb * i_lightCol * i_shadow;
		col += subsurface;
	}
#endif // CREST_SUBSURFACESCATTERING_ON

	return col;
}

// SG function.
half3 CrestNodeLightWaterVolume
(
	const half3 i_scatterColourBase,
	const half3 i_scatterColourShadow,
	const half i_sssIntensityBase,
	const half i_sssIntensitySun,
	const half3 i_sssTint,
	const half i_sssSunFalloff,
	const half i_shadow,
	const half i_sss,
	const half3 i_viewNorm,
	const half3 i_ambientLighting,
	const half3 i_primaryLightDirection,
	const half3 i_primaryLightIntensity
)
{
	// base colour
	half3 volumeLight = i_scatterColourBase;

// #if CREST_SHADOWS_ON
	volumeLight = lerp(i_scatterColourShadow, volumeLight, i_shadow);
// #endif // CREST_SHADOWS_ON

	// Light the base colour. Use the constant term (0th order) of SH stuff - this is the average. Use the primary light integrated over the
	// hemisphere (divide by pi).
	volumeLight *= i_ambientLighting + i_shadow * i_primaryLightIntensity / 3.14159;

	// Approximate subsurface scattering - add light when surface faces viewer. Use geometry normal - don't need high freqs.
	half towardsSun = pow(max(0.0, dot(i_primaryLightDirection, -i_viewNorm)), i_sssSunFalloff);

	half sss = i_sss;

	// Extents need the default SSS to avoid popping and not being noticeably different.
	if (_LD_SliceIndex == ((uint)_SliceCount - 1))
	{
		sss = CREST_SSS_MAXIMUM - CREST_SSS_RANGE;
	}

	float v = abs(i_viewNorm.y);
	half3 subsurface = (i_sssIntensityBase + i_sssIntensitySun * towardsSun) * i_sssTint * i_primaryLightIntensity * i_shadow;
	subsurface *= (1.0 - v * v) * sss;
	volumeLight += subsurface;

	return volumeLight;
}

// Taken from: OceanHelpersNew.hlsl
float3 CrestWorldToUV(in float2 i_samplePos, in CascadeParams i_cascadeParams, in float i_sliceIndex)
{
	float2 uv = (i_samplePos - i_cascadeParams._posSnapped) / (i_cascadeParams._texelWidth * i_cascadeParams._textureRes) + 0.5;
	return float3(uv, i_sliceIndex);
}

#endif // SHADERGRAPH_PREVIEW

void CrestNodeApplyUnderwaterFog_half
(
	in const float2 i_positionNDC,
	in const float3 i_positionWS,
	in const float i_deviceDepth,
	in const half i_multiplier,
	in const half3 i_color,
	out half3 o_color,
	out half3 o_scatterColor,
	out float o_alpha,
	out bool o_isFogged
)
{
#ifdef SHADERGRAPH_PREVIEW

	o_color = i_color;
	o_scatterColor = 0.0;
	o_isFogged = false;
	o_alpha = 0.0;
	return;
#else

	o_isFogged = false;
	o_color = i_color;
	o_alpha = 0.0;
	o_scatterColor = 0.0;

#if CREST_WATER_VOLUME
	// No fog before volume.
	float rawFrontFaceZ = SAMPLE_TEXTURE2D_X(_CrestWaterVolumeFrontFaceTexture, sampler_CrestWaterVolumeFrontFaceTexture, i_positionNDC).r;
	if (rawFrontFaceZ > 0.0 && rawFrontFaceZ < i_deviceDepth)
	{
		return;
	}

#if CREST_WATER_VOLUME_2D
	// No fog if plane is not in view. If we wanted to be consistent with the underwater shader, we should also check
	// this for non fly-through volumes too, but being inside a non fly-through volume is undefined behaviour so we can
	// save a variant.
	if (rawFrontFaceZ == 0.0)
	{
		return;
	}
#endif // CREST_WATER_VOLUME_2D
#endif // CREST_WATER_VOLUME

	half mask = SAMPLE_TEXTURE2D_X(_CrestOceanMaskTexture, sampler_CrestOceanMaskTexture, i_positionNDC).r;
	if (mask >= CREST_MASK_NO_FOG)
	{
		return;
	}

#if CREST_URP
	const Light light = GetMainLight();
	const half3 lightDirection = light.direction;
	const half3 lightColor = light.color * light.distanceAttenuation;
#elif CREST_HDRP
	const half3 lightDirection = _PrimaryLightDirection;
	const half3 lightColor = _PrimaryLightIntensity;
#else
	const half3 lightDirection = -normalize(float3(-0.5, 0.5, -0.5));;
	const half3 lightColor = (float3)1.0;
#endif

	half3 view =  GetWorldSpaceNormalizeViewDir(i_positionWS);
	// half3 view =  normalize(_WorldSpaceCameraPos - i_positionWS);

	// Get the largest distance.
	float rawFogDistance = i_deviceDepth;
#if CREST_WATER_VOLUME_HAS_BACKFACE
	// Use the closest of the two.
	float rawBackFaceZ = SAMPLE_TEXTURE2D_X(_CrestWaterVolumeBackFaceTexture, sampler_CrestWaterVolumeBackFaceTexture, i_positionNDC).r;
	rawFogDistance = max(rawFogDistance, rawBackFaceZ);
#endif

	float fogDistance = CrestLinearEyeDepth(rawFogDistance);

#if CREST_WATER_VOLUME
#if CREST_WATER_VOLUME_HAS_BACKFACE
	if (rawFrontFaceZ > 0.0)
#endif
	{
		fogDistance -= CrestLinearEyeDepth(rawFrontFaceZ);
	}
#endif

	half shadow = 1.0;
#if CREST_SHADOWS_ON
	{
		// Offset slice so that we do not get high frequency detail. But never use last lod as this has crossfading.
		int sliceIndex = clamp(_CrestDataSliceOffset, 0, _SliceCount - 2);
		const float3 uv = CrestWorldToUV(_WorldSpaceCameraPos.xz, _CrestCascadeData[sliceIndex], sliceIndex);
		// Camera should be at center of LOD system so no need for blending (alpha, weights, etc).
		shadow = _LD_TexArray_Shadow.SampleLevel(LODData_linear_clamp_sampler, uv, 0.0).x;
		shadow = saturate(1.0 - shadow);
	}
#endif // CREST_SHADOWS_ON

#if CREST_URP
	o_scatterColor = CrestScatterColour
	(
		_CrestAmbientLighting,
		lightColor,
		lightDirection,
		view,
		shadow
	);
#endif

#if CREST_HDRP
	o_scatterColor = CrestNodeLightWaterVolume
	(
		_CrestDiffuse,
		_CrestDiffuseShadow,
		_CrestSubSurfaceBase,
		_CrestSubSurfaceSun,
		_CrestSubSurfaceColour,
		_CrestSubSurfaceSunFallOff,
		shadow,
		1.0,
		view,
		_CrestAmbientLighting,
		lightDirection,
		lightColor
	);
#endif

	float3 fogFactor = saturate((1.0 - exp(-_CrestDepthFogDensity.xyz * fogDistance * i_multiplier)));
	o_color = lerp(i_color, o_scatterColor, fogFactor);
	o_alpha = min(fogFactor.r, min(fogFactor.g, fogFactor.b));
	o_isFogged = true;
#endif
}
