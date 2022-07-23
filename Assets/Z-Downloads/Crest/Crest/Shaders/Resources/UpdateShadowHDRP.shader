// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

Shader "Hidden/Crest/Simulation/Update Shadow HDRP"
{
	SubShader
	{
		Pass
		{
			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			// #pragma enable_d3d11_debug_symbols

			// SHADOW_ULTRA_LOW uses Gather which is 4.5. HDRP minimum is 5.0 so this is fine.
			#pragma target 4.5

			// TODO: We might be able to expose this to give developers the option.
			// #pragma multi_compile SHADOW_ULTRA_LOW SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH

			// Ultra low uses Gather to filter which should be same cost as not filtering. See algorithms per keyword:
			// Runtime/Lighting/Shadow/HDShadowAlgorithms.hlsl
			#define SHADOW_ULTRA_LOW

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/HDShadow.hlsl"

			float4x4 _CrestViewProjectionMatrix;

			#include "../ShaderLibrary/UpdateShadow.hlsl"

			half CrestSampleShadows(const float4 i_positionWS)
			{
				// Get directional light data. By definition we only have one directional light casting shadow.
				DirectionalLightData light = _DirectionalLightDatas[_DirectionalShadowIndex];
				HDShadowContext context = InitShadowContext();

				// Zeros are for screen space position and world space normal which are for filtering and normal bias
				// respectively. They did not appear to have an impact.
				half shadows = GetDirectionalShadowAttenuation(context, 0, i_positionWS.xyz, 0, _DirectionalShadowIndex, -light.forward);
				// Apply shadow strength from main light.
				shadows = LerpWhiteTo(shadows, light.shadowDimmer);

				return shadows;
			}

			half CrestComputeShadowFade(const float4 i_positionWS)
			{
				// TODO: Work out shadow fade.
				return 0.0;
			}

			Varyings Vert(Attributes input)
			{
				Varyings output;

				// Use a custom matrix which is the value of unity_MatrixVP from the frame debugger.
				output.positionCS = mul(_CrestViewProjectionMatrix, float4(input.positionOS, 1.0));

				// world pos from [0,1] quad
				output.positionWS.xyz = float3(input.positionOS.x - 0.5, 0.0, input.positionOS.y - 0.5) * _Scale * 4.0 + _CenterPos;
				output.positionWS.y = _OceanCenterPosWorld.y;

				return output;
			}
			ENDHLSL
		}
	}

	Fallback Off
}
