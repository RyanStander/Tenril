// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

#include "OceanGraphConstants.hlsl"
#include "../OceanGlobals.hlsl"
#include "../ShaderLibrary/Texture.hlsl"

#ifndef SHADERGRAPH_PREVIEW
#if CREST_HDRP_FORWARD_PASS
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/HDShadow.hlsl"
#endif // CREST_HDRP_FORWARD_PASS
#endif // SHADERGRAPH_PREVIEW

void CrestNodeApplyCaustics_float
(
	in const half3 i_sceneColour,
	in const float3 i_scenePos,
	in const float i_waterSurfaceY,
	in const half3 i_depthFogDensity,
	in const half3 i_lightCol,
	in const half3 i_lightDir,
	in const float i_sceneZ,
	in const Texture2D<float4> i_texture,
	in const half i_textureScale,
	in const half i_textureAverage,
	in const half i_strength,
	in const half i_focalDepth,
	in const half i_depthOfField,
	in const Texture2D<float4> i_distortion,
	in const half i_distortionStrength,
	in const half i_distortionScale,
	in const bool i_underwater,
	out half3 o_sceneColour
)
{
	o_sceneColour = i_sceneColour;

#ifdef SHADERGRAPH_PREVIEW
	// Samplers are not defined in shader graph. Silence errors.
	SamplerState sampler_CausticsTexture = LODData_linear_clamp_sampler;
	SamplerState sampler_CausticsDistortionTexture = LODData_linear_clamp_sampler;
	float4 _CausticsTexture_TexelSize = (float4)0.0;
	float4 _CausticsDistortionTexture_TexelSize = (float4)0.0;
#endif

	const WaveHarmonic::Crest::TiledTexture causticsTexture =
		WaveHarmonic::Crest::TiledTexture::Make(i_texture, sampler_CausticsTexture, _CausticsTexture_TexelSize, i_textureScale);
	const WaveHarmonic::Crest::TiledTexture distortionTexture =
		WaveHarmonic::Crest::TiledTexture::Make(i_distortion, sampler_CausticsDistortionTexture, _CausticsDistortionTexture_TexelSize, i_distortionScale);

	// @HACK: When used by the underwater effect, either scene position or surface height is out of sync leading to
	// caustics rendering short of the surface. CREST_GENERATED_SHADER_ON limits this to the ocean shader.
#if CREST_GENERATED_SHADER_ON
	// We don't want caustics showing above the surface until we can implement it for both cases when the view is either
	// above or below the surface. We can only do the latter scenario at the moment.
	if (i_scenePos.y > i_waterSurfaceY) return;
#endif

	half sceneDepth = i_waterSurfaceY - i_scenePos.y;

	// Compute mip index manually, with bias based on sea floor depth. We compute it manually because if it is computed automatically it produces ugly patches
	// where samples are stretched/dilated. The bias is to give a focusing effect to caustics - they are sharpest at a particular depth. This doesn't work amazingly
	// well and could be replaced.
	float mipLod = log2(i_sceneZ) + abs(sceneDepth - i_focalDepth) / i_depthOfField;

	// Project along light dir, but multiply by a fudge factor reduce the angle bit - compensates for fact that in real life
	// caustics come from many directions and don't exhibit such a strong directonality
	// Removing the fudge factor (4.0) will cause the caustics to move around more with the waves. But this will also
	// result in stretched/dilated caustics in certain areas. This is especially noticeable on angled surfaces.
	float2 lightProjection = i_lightDir.xz * sceneDepth / (4.0 * i_lightDir.y);

	float3 cuv1 = 0.0; float3 cuv2 = 0.0;
	{
		float2 surfacePosXZ = i_scenePos.xz;
		float surfacePosScale = 1.37;

#if CREST_FLOATING_ORIGIN
		// Apply tiled floating origin offset. Always needed.
		surfacePosXZ -= causticsTexture.FloatingOriginOffset();
		// Scale was causing popping.
		surfacePosScale = 1.0;
#endif

		surfacePosXZ += lightProjection;

		cuv1 = float3
		(
			surfacePosXZ / causticsTexture._scale + float2(0.044 * _CrestTime + 17.16, -0.169 * _CrestTime),
			mipLod
		);
		cuv2 = float3
		(
			surfacePosScale * surfacePosXZ / causticsTexture._scale + float2(0.248 * _CrestTime, 0.117 * _CrestTime),
			mipLod
		);
	}

	if (i_underwater)
	{
		float2 surfacePosXZ = i_scenePos.xz;

#if CREST_FLOATING_ORIGIN
		// Apply tiled floating origin offset. Always needed.
		surfacePosXZ -= distortionTexture.FloatingOriginOffset();
#endif

		surfacePosXZ += lightProjection;

		half2 causticN = i_distortionStrength * UnpackNormal(distortionTexture.Sample(surfacePosXZ / distortionTexture._scale)).xy;
		cuv1.xy += 1.30 * causticN;
		cuv2.xy += 1.77 * causticN;
	}

	half causticsStrength = i_strength;

// #if CREST_SHADOWS_ON
	{
#if CREST_URP
		// We could skip GetMainLight but this is recommended approach which is likely more robust to API changes.
		float4 shadowCoord = TransformWorldToShadowCoord(i_scenePos);
		Light mainLight = GetMainLight(TransformWorldToShadowCoord(i_scenePos));
		causticsStrength *= mainLight.shadowAttenuation;
#endif // CREST_URP

#ifndef SHADERGRAPH_PREVIEW
#if CREST_HDRP_FORWARD_PASS
		DirectionalLightData light = _DirectionalLightDatas[_DirectionalShadowIndex];
		HDShadowContext context = InitShadowContext();
		context.directionalShadowData = _HDDirectionalShadowData[_DirectionalShadowIndex];

		float3 positionWS = GetCameraRelativePositionWS(i_scenePos);
		// From Unity:
		// > With XR single-pass and camera-relative: offset position to do lighting computations from the combined center view (original camera matrix).
		// > This is required because there is only one list of lights generated on the CPU. Shadows are also generated once and shared between the instanced views.
		ApplyCameraRelativeXR(positionWS);

		// TODO: Pass in screen space position and scene normal.
		half shadows = GetDirectionalShadowAttenuation
		(
			context,
			0, // positionSS
			positionWS,
			0, // normalWS
			light.shadowIndex,
			-light.forward
		);

		// Apply shadow strength from main light.
		shadows = LerpWhiteTo(shadows, light.shadowDimmer);

		causticsStrength *= shadows;
#endif // CREST_HDRP_FORWARD_PASS
#endif // SHADERGRAPH_PREVIEW
	}
// #endif // CREST_SHADOWS_ON

	o_sceneColour.xyz *= 1.0 + causticsStrength * (
		0.5 * causticsTexture.SampleLevel(cuv1.xy, cuv1.z).xyz +
		0.5 * causticsTexture.SampleLevel(cuv2.xy, cuv2.z).xyz
		- i_textureAverage);
}
