// Crest Ocean System

// Copyright 2022 Wave Harmonic Ltd

Shader "Hidden/Crest/Examples/Mask Fill"
{
    SubShader
    {
        Pass
        {
            Name "Mask Fill"
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment

            // If we use a built-in RP shader, then the texture is not set correctly to an array if XR package is
            // installed.
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"

            TEXTURE2D_X(_CrestWaterVolumeFrontFaceTexture);

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vertex(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            float4 Fragment(Varyings input, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                if (LOAD_TEXTURE2D_X(_CrestWaterVolumeFrontFaceTexture, input.positionCS.xy).r < input.positionCS.z)
                {
                    discard;
                }

                return isFrontFace ? 0.0 : 1.0;
            }
            ENDHLSL
        }
    }
}
