// Crest Ocean System

// Copyright 2022 Wave Harmonic Ltd

Shader "Hidden/Crest/SphereGradientBackground"
{
	Properties
	{
		_ColorTowardsSun("_ColorTowardsSun", Color) = (1, 1, 1)
		_ColorAwayFromSun("_ColorAwayFromSun", Color) = (1, 1, 1)
		_Exponent("_Exponent", Float) = 1.0
	}
	SubShader
	{
		// Did not like being Opaque in HDRP.
		Tags { "Queue"="Transparent-10" }
		LOD 100

		Pass
		{
			Cull Front
			Blend Off

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

			float3 _PrimaryLightDirection;
			float3 _PrimaryLightIntensity;

			struct Attributes
			{
				float3 vertex : POSITION;
			};

			struct Varyings
			{
				float4 vertex : SV_POSITION;
				float3 positionWS : TEXCOORD0;
			};

			Varyings vert (Attributes v)
			{
				Varyings o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.positionWS = TransformObjectToWorld(v.vertex);
				return o;
			}

			float3 _ColorTowardsSun;
			float3 _ColorAwayFromSun;
			float _Exponent;

			float4 frag (Varyings i) : SV_Target
			{
				float3 worldPosition = i.positionWS;
				float3 viewDirection = normalize(i.positionWS - _WorldSpaceCameraPos);

				const real3 lightDir = _PrimaryLightDirection;
				const real3 lightCol = _PrimaryLightIntensity * GetCurrentExposureMultiplier();

				float alpha = saturate(0.5 * dot(viewDirection, lightDir) + 0.5);
				alpha = pow(alpha, _Exponent);

				float3 col = lerp(_ColorAwayFromSun, _ColorTowardsSun, alpha);
				return float4(col * saturate(max(lightCol.r, max(lightCol.g, lightCol.b))), 1.0);
			}
			ENDHLSL
		}
	}
}
