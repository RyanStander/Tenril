// Crest Ocean System

// Copyright 2021 Wave Harmonic Ltd

#include "../../OceanConstants.hlsl"
#include "../../OceanInputsDriven.hlsl"
#include "../../OceanGlobals.hlsl"
#include "../../OceanHelpersNew.hlsl"
#include "../../OceanShaderHelpers.hlsl"

#include "../../Helpers/WaterVolume.hlsl"

half4 _ScatterColourBase;
half3 _ScatterColourShadow;
float4 _ScatterColourShallow;
half _ScatterColourShallowDepthMax;
half _ScatterColourShallowDepthFalloff;

half _SSSIntensityBase;
half _SSSIntensitySun;
half4 _SSSTint;
half _SSSSunFalloff;

half3 _DepthFogDensity;

float _CausticsTextureScale;
float _CausticsTextureAverage;
float _CausticsStrength;
float _CausticsFocalDepth;
float _CausticsDepthOfField;
float _CausticsDistortionStrength;
float _CausticsDistortionScale;

half3 _CrestAmbientLighting;

TEXTURE2D_X(_CrestOceanMaskTexture);
TEXTURE2D_X(_CrestOceanMaskDepthTexture);
TEXTURE2D_X(_CrestCameraColorTexture);

TEXTURE2D(_CausticsTexture); SAMPLER(sampler_CausticsTexture); float4 _CausticsTexture_TexelSize;
TEXTURE2D(_CausticsDistortionTexture); SAMPLER(sampler_CausticsDistortionTexture); float4 _CausticsDistortionTexture_TexelSize;

#include "../../ShadergraphFramework/CrestNodeDrivenInputs.hlsl"
#include "../../ShadergraphFramework/CrestNodeLightWaterVolume.hlsl"
#include "../../ShadergraphFramework/CrestNodeApplyCaustics.hlsl"
#include "../../ShadergraphFramework/CrestNodeAmbientLight.hlsl"

#include "../UnderwaterEffectShared.hlsl"

float LinearToDeviceDepth(float linearDepth, float4 zBufferParam)
{
	//linear = 1.0 / (zBufferParam.z * device + zBufferParam.w);
	float device = (1.0 / linearDepth - zBufferParam.w) / zBufferParam.z;
	return device;
}

half3 ApplyUnderwaterEffect(
	half3 sceneColour,
	const float rawDepth,
	const float sceneZ,
	const float fogDistance,
	const half3 view,
	const float2 screenPos,
	const bool hasCaustics
) {
	const bool isUnderwater = true;

	half3 volumeLight = 0.0;
	float3 displacement = 0.0;
	half seaLevelOffset = 0.0;
	{
		// Offset slice so that we dont get high freq detail. But never use last lod as this has crossfading.
		int sliceIndex = clamp(_CrestDataSliceOffset, 0, _SliceCount - 2);
		float3 uv_slice = WorldToUV(_WorldSpaceCameraPos.xz, _CrestCascadeData[sliceIndex], sliceIndex);
		SampleDisplacements(_LD_TexArray_AnimatedWaves, uv_slice, 1.0, displacement);

		half depth = CREST_OCEAN_DEPTH_BASELINE;
		half2 shadow = 0.0;
		{
			SampleSingleSeaDepth(_LD_TexArray_SeaFloorDepth, uv_slice, depth, seaLevelOffset);
// #if CREST_SHADOWS_ON
			SampleShadow(_LD_TexArray_Shadow, uv_slice, 1.0, shadow);
// #endif
		}

		half3 ambientLighting = _CrestAmbientLighting;
		ApplyIndirectLightingMultiplier(ambientLighting);

		CrestNodeLightWaterVolume_half
		(
			_ScatterColourBase.xyz,
			_ScatterColourShadow.xyz,
			_ScatterColourShallow.xyz,
			_ScatterColourShallowDepthMax,
			_ScatterColourShallowDepthFalloff,
			_SSSIntensityBase,
			_SSSIntensitySun,
			_SSSTint.xyz,
			_SSSSunFalloff,
			depth,
			shadow,
			1.0, // Skip SSS pinch calculation due to performance concerns.
			view,
			_WorldSpaceCameraPos,
			ambientLighting,
			_PrimaryLightDirection,
			_PrimaryLightIntensity,
			volumeLight
		);
	}

#if CREST_CAUSTICS_ON
	float3 worldPos;
	{

		// HDRP needs a different way to unproject to world space. I tried to put this code into URP but it didnt work on 2019.3.0f1
		float deviceZ = LinearToDeviceDepth(sceneZ, _ZBufferParams);
		PositionInputs posInput = GetPositionInput(screenPos * _ScreenSize.xy, _ScreenSize.zw, deviceZ, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
		worldPos = posInput.positionWS;
#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
		worldPos += _WorldSpaceCameraPos;
#endif
	}

	if (hasCaustics)
	{
		CrestNodeApplyCaustics_float
		(
			sceneColour,
			worldPos,
			displacement.y + _OceanCenterPosWorld.y + seaLevelOffset,
			_DepthFogDensity,
			_PrimaryLightIntensity,
			_PrimaryLightDirection,
			sceneZ,
			_CausticsTexture,
			_CausticsTextureScale,
			_CausticsTextureAverage,
			_CausticsStrength,
			_CausticsFocalDepth,
			_CausticsDepthOfField,
			_CausticsDistortionTexture,
			_CausticsDistortionStrength,
			_CausticsDistortionScale,
			isUnderwater,
			sceneColour
		);
	}
#endif // CREST_CAUSTICS_ON

	return lerp(sceneColour, volumeLight * GetCurrentExposureMultiplier(), saturate(1.0 - exp(-_DepthFogDensity.xyz * fogDistance)));
}

struct Attributes
{
#if CREST_WATER_VOLUME
	float3 positionOS : POSITION;
#else
	uint id : SV_VertexID;
#endif
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
	float4 positionCS : SV_POSITION;
#if CREST_WATER_VOLUME
	float4 screenPosition : TEXCOORD0;
#else
	float2 uv : TEXCOORD0;
#endif
	UNITY_VERTEX_OUTPUT_STEREO
};

// #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

Varyings Vert (Attributes input)
{
	Varyings output;
	ZERO_INITIALIZE(Varyings, output);
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#if CREST_WATER_VOLUME
	// Use actual geometry instead of full screen triangle.
	output.positionCS = TransformObjectToHClip(input.positionOS);
#else
	output.positionCS = GetFullScreenTriangleVertexPosition(input.id, UNITY_RAW_FAR_CLIP_VALUE);
#endif

	return output;
}

float4 Frag(Varyings input) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

	float rawDepth = LoadCameraDepth(input.positionCS.xy);

	PositionInputs posInput = GetPositionInput(input.positionCS.xy, _ScreenSize.zw, rawDepth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
	const half3 view = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
	float2 uv = posInput.positionNDC.xy;

	uint2 positionSS = input.positionCS.xy;
	half3 sceneColour = LOAD_TEXTURE2D_X(_CrestCameraColorTexture, positionSS).rgb;
	float mask = LOAD_TEXTURE2D_X(_CrestOceanMaskTexture, positionSS).x;
	const float rawOceanDepth = LOAD_TEXTURE2D_X(_CrestOceanMaskDepthTexture, positionSS).x;

#if _DEBUG_VIEW_STENCIL
	return DebugRenderStencil(sceneColour);
#endif

	bool isOceanSurface; bool isUnderwater; bool hasCaustics; float sceneZ;
	GetOceanSurfaceAndUnderwaterData(input.positionCS, positionSS, rawOceanDepth, mask, rawDepth, isOceanSurface, isUnderwater, hasCaustics, sceneZ, OCEAN_DEPTH_TOLERANCE);

	float fogDistance = sceneZ;
	float meniscusDepth = 0.0;
#if CREST_WATER_VOLUME
	ApplyWaterVolumeToUnderwaterFogAndMeniscus(input.positionCS, fogDistance, meniscusDepth);
#endif

#if _DEBUG_VIEW_OCEAN_MASK
	return DebugRenderOceanMask(isOceanSurface, isUnderwater, mask, sceneColour);
#endif

	if (isUnderwater)
	{
		sceneColour = ApplyUnderwaterEffect(sceneColour, rawDepth, sceneZ, fogDistance, view, uv, hasCaustics);
	}

	const float wt = ComputeMeniscusWeight(positionSS, mask, _HorizonNormal, sceneZ);

	return half4(wt * sceneColour, 1.0);
}
