// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

#include "OceanGraphConstants.hlsl"
#include "../OceanGlobals.hlsl"
#include "../OceanShaderHelpers.hlsl"

// We take the unrefracted scene colour (i_sceneColourUnrefracted) as input because having a Scene Colour node in the graph
// appears to be necessary to ensure the scene colours are bound?
void CrestNodeSceneColour_half
(
	in const half i_refractionStrength,
	in const half3 i_scatterCol,
	in const half3 i_normalTS,
	in const float4 i_screenPos,
	in const float i_pixelZ,
	in const half3 i_sceneColourUnrefracted,
	in const float i_sceneZ,
	in const float i_deviceSceneZ,
	in const bool i_underwater,
	out half3 o_sceneColour,
	out float o_sceneDistance,
	out float3 o_scenePositionWS
)
{
	//#if _TRANSPARENCY_ON

	// View ray intersects geometry surface either above or below ocean surface

	half2 refractOffset = i_refractionStrength * i_normalTS.xy;
	if (!i_underwater)
	{
		// We're above the water, so behind interface is depth fog
		refractOffset *= min(1.0, 0.5 * (i_sceneZ - i_pixelZ)) / i_sceneZ;
	}

#if CREST_HDRP
	// HDRP gets artefacts (dark patches) at the edges. And to quote a report, "Mainly on DX12 on Xbox Series X", color
	// artefacts appear underwater from refractions.
	const float4 screenPosRefract = clamp(i_screenPos + float4(refractOffset, 0.0, 0.0), 0.01, 0.99);
#else
	const float4 screenPosRefract = i_screenPos + float4(refractOffset, 0.0, 0.0);
#endif // CREST_HDRP
	const float sceneZRefractDevice = SHADERGRAPH_SAMPLE_SCENE_DEPTH(screenPosRefract.xy);

#if CREST_HDRP
	// Convert coordinates for Load.
	const float2 positionSS = i_screenPos.xy * _ScreenSize.xy;
	const float2 refractedPositionSS = screenPosRefract.xy * _ScreenSize.xy;
#endif // CREST_HDRP

	// Depth fog & caustics - only if view ray starts from above water
	if (!i_underwater)
	{
		float sceneZRefract = CrestLinearEyeDepth(sceneZRefractDevice);

		// Compute depth fog alpha based on refracted position if it landed on an underwater surface, or on unrefracted depth otherwise
		if (sceneZRefract > i_pixelZ)
		{
#if CREST_HDRP
			// NOTE: For HDRP, refractions produce an outline which requires multisampling with a two pixel offset to
			// cover. This is without MSAA. A deeper investigation is needed.
			float msDepth = CrestMultiLoadDepth
			(
				_CameraDepthTexture,
				refractedPositionSS,
				i_refractionStrength > 0 ? 2 : _CrestDepthTextureOffset,
				sceneZRefractDevice
			);
#else
			float msDepth = CrestMultiSampleDepth
			(
				_CameraDepthTexture,
				sampler_CameraDepthTexture,
				_CameraDepthTexture_TexelSize.xy,
				screenPosRefract.xy,
				i_refractionStrength > 0.0 ? 2 : _CrestDepthTextureOffset,
				sceneZRefractDevice
			);
#endif // CREST_HDRP
			o_sceneDistance = CrestLinearEyeDepth(msDepth) - i_pixelZ;

			o_sceneColour = SHADERGRAPH_SAMPLE_SCENE_COLOR(screenPosRefract.xy);

#if CREST_HDRP
			// HDRP needs a different way to unproject to world space. I tried to put this code into URP but it didnt work on 2019.3.0f1
			PositionInputs posInput = GetPositionInput(refractedPositionSS, _ScreenSize.zw, sceneZRefractDevice, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
			o_scenePositionWS = posInput.positionWS;
#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
			o_scenePositionWS += _WorldSpaceCameraPos;
#endif
#else
			o_scenePositionWS = ComputeWorldSpacePosition(screenPosRefract.xy, sceneZRefractDevice, UNITY_MATRIX_I_VP);
#endif // CREST_HDRP
		}
		else
		{
			// It seems that when MSAA is enabled this can sometimes be negative
			o_sceneDistance = max(CrestLinearEyeDepth(CREST_MULTILOAD_SCENE_DEPTH(positionSS, i_deviceSceneZ)) - i_pixelZ, 0.0);

			o_sceneColour = i_sceneColourUnrefracted;

#if CREST_HDRP
			// HDRP needs a different way to unproject to world space. I tried to put this code into URP but it didnt work on 2019.3.0f1
			PositionInputs posInput = GetPositionInput(positionSS, _ScreenSize.zw, i_deviceSceneZ, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
			o_scenePositionWS = posInput.positionWS;
#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
			o_scenePositionWS += _WorldSpaceCameraPos;
#endif
#else
			o_scenePositionWS = ComputeWorldSpacePosition(i_screenPos.xy, i_deviceSceneZ, UNITY_MATRIX_I_VP);
#endif // CREST_HDRP
		}
	}
	else
	{
		// Depth fog is handled by underwater shader
		o_sceneDistance = i_pixelZ;
		o_sceneColour = SHADERGRAPH_SAMPLE_SCENE_COLOR(screenPosRefract.xy);

#if CREST_HDRP
		// HDRP needs a different way to unproject to world space. I tried to put this code into URP but it didnt work on 2019.3.0f1
		PositionInputs posInput = GetPositionInput(refractedPositionSS, _ScreenSize.zw, sceneZRefractDevice, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
		o_scenePositionWS = posInput.positionWS;
#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
		o_scenePositionWS += _WorldSpaceCameraPos;
#endif
#else
		o_scenePositionWS = ComputeWorldSpacePosition(screenPosRefract.xy, sceneZRefractDevice, UNITY_MATRIX_I_VP);
#endif // CREST_HDRP
	}

	//#endif // _TRANSPARENCY_ON
}
