// Crest Ocean System

// Copyright 2021 Wave Harmonic Ltd

Shader "Hidden/Crest/Underwater/Underwater Effect HDRP"
{
	HLSLINCLUDE
	#pragma target 4.5
	#pragma vertex Vert
	#pragma fragment Frag

	// #pragma enable_d3d11_debug_symbols

	// Use multi_compile because these keywords are copied over from the ocean material. With shader_feature,
	// the keywords would be stripped from builds. Unused shader variants are stripped using a build processor.
	#pragma multi_compile_local __ CREST_CAUSTICS_ON
	#pragma multi_compile_local __ CREST_FLOW_ON
	#pragma multi_compile_local __ CREST_FOAM_ON

	#pragma multi_compile_local __ CREST_MENISCUS
	// #pragma multi_compile_local __ _FULL_SCREEN_EFFECT
	#pragma multi_compile_local __ _DEBUG_VIEW_OCEAN_MASK
	#pragma multi_compile_local __ _DEBUG_VIEW_STENCIL

	#pragma multi_compile _ CREST_FLOATING_ORIGIN

	// Low appears good enough as it has filtering which is necessary when close to a shadow.
	#define SHADOW_LOW

	// In shared SG code we target the forward pass to avoid shader compilation errors.
	#define SHADERPASS SHADERPASS_FORWARD

	#define OCEAN_DEPTH_TOLERANCE 0.0

	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
	ENDHLSL

	SubShader
	{
		ZWrite Off Blend Off

		Pass
		{
			Name "Full Screen"
			Cull Off
			ZTest Always

			HLSLPROGRAM
			// Both "__" and "_FULL_SCREEN_EFFECT" are fullscreen triangles. The latter only denotes an optimisation of
			// whether to skip the horizon calculation.
			#pragma multi_compile_local __ _FULL_SCREEN_EFFECT
			#include "../UnderwaterEffectHDRP.hlsl"
			ENDHLSL
		}

		Pass
		{
			// Only adds fog to the front face and in effect anything behind it.
			Name "Volume: Front Face (2D)"
			Cull Back
			ZTest LEqual

			HLSLPROGRAM
			#define CREST_WATER_VOLUME 1
			#define CREST_WATER_VOLUME_FRONT_FACE 1
			#include "../UnderwaterEffectHDRP.hlsl"
			ENDHLSL
		}

		Pass
		{
			// Only adds fog to the front face and in effect anything behind it.
			Name "Volume: Front Face (3D)"
			Cull Back
			ZTest LEqual

			HLSLPROGRAM
			#define CREST_WATER_VOLUME 1
			#define CREST_WATER_VOLUME_HAS_BACKFACE 1
			#define CREST_WATER_VOLUME_FRONT_FACE 1
			#include "../UnderwaterEffectHDRP.hlsl"
			ENDHLSL
		}

		Pass
		{
			// Only adds fog to the front face and in effect anything behind it.
			Name "Volume: Front Face (Fly-Through)"
			Cull Back
			ZTest LEqual

			Stencil
			{
				// Must match k_StencilValueVolume in:
				// Scripts/Underwater/UnderwaterRenderer.Mask.cs
				Ref 5
				Comp Always
				Pass Replace
				ZFail IncrSat
			}

			HLSLPROGRAM
			#define CREST_WATER_VOLUME 1
			#define CREST_WATER_VOLUME_HAS_BACKFACE 1
			#define CREST_WATER_VOLUME_FRONT_FACE 1
			#include "../UnderwaterEffectHDRP.hlsl"
			ENDHLSL
		}

		Pass
		{
			// Back face will only render if view is within the volume and there is no scene in front. It will only add
			// fog to the back face (and in effect anything behind it). No caustics.
			Name "Volume: Back Face"
			Cull Front
			ZTest LEqual

			Stencil
			{
				// Must match k_StencilValueVolume in:
				// Scripts/Underwater/UnderwaterRenderer.Mask.cs
				Ref 5
				Comp NotEqual
				Pass Replace
				ZFail IncrSat
			}

			HLSLPROGRAM
			#define CREST_WATER_VOLUME 1
			#define CREST_WATER_VOLUME_BACK_FACE 1
			#include "../UnderwaterEffectHDRP.hlsl"
			ENDHLSL
		}

		Pass
		{
			// When inside a volume, this pass will render to the scene within the volume.
			Name "Volume: Scene (Full Screen)"
			Cull Back
			ZTest Always
			Stencil
			{
				// We want to render over the scene that's inside the volume, but not over already fogged areas. It will
				// handle all of the scene within the geometry once the camera is within the volume.
				// 0 = Outside of geometry as neither face passes have touched it.
				// 1 = Only back face z failed which means scene is in front of back face but not front face.
				// 2 = Both front and back face z failed which means outside geometry.
				Ref 1
				Comp Equal
				Pass Replace
			}

			HLSLPROGRAM
			#define CREST_WATER_VOLUME_FULL_SCREEN 1
			#include "../UnderwaterEffectHDRP.hlsl"
			ENDHLSL
		}

		Pass
		{
			Name "Underwater Post Process"

			Cull Off

			HLSLPROGRAM
			// Both "__" and "_FULL_SCREEN_EFFECT" are fullscreen triangles. The latter only denotes an optimisation of
			// whether to skip the horizon calculation.
			#pragma multi_compile_local __ _FULL_SCREEN_EFFECT

			// NOTE: In HDRP PP we get given a depth buffer which contains the depths of rendered transparencies
			// (such as the ocean). We would preferably only have opaque objects in the depth buffer, so that we can
			// more easily tell whether the current pixel is rendering the ocean surface or not.
			// (We would do this by checking if the ocean mask pixel is in front of the scene pixel.)
			//
			// To workaround this. we provide a depth tolerance, to avoid ocean-surface z-fighting. (We assume that
			// anything in the depth buffer which has a depth within the ocean tolerance to the ocean mask itself is
			// the ocean).
			//
			// FUTURE NOTE: This issue is easily avoided with a small modification to HDRenderPipeline.cs
			// Look at the RenderPostProcess() method.
			// We get given m_SharedRTManager.GetDepthStencilBuffer(), having access
			// m_SharedRTManager.GetDepthTexture() would immediately resolve this issue.
			// - Tom Read Cutting - 2020-01-03
			//
			// FUTURE NOTE: The depth texture was accessible through reflection. But could no longer be accessed
			// with future HDRP versions.
			// - Dale Eidd - 2020-11-09
			#undef OCEAN_DEPTH_TOLERANCE
			#define OCEAN_DEPTH_TOLERANCE 0.0001

			#include "../UnderwaterEffectHDRP.hlsl"
			ENDHLSL
		}
	}
	Fallback Off
}
