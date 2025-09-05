Shader "Custom/ColorIDBlendTiled_URP"
{
    Properties
    {
        _TexA("Texture A", 2D) = "white" {}
        _TexB("Texture B", 2D) = "white" {}
        _TexC("Texture C", 2D) = "white" {}
        _ColorID("Color ID Map", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_TexA); SAMPLER(sampler_TexA);
            TEXTURE2D(_TexB); SAMPLER(sampler_TexB);
            TEXTURE2D(_TexC); SAMPLER(sampler_TexC);
            TEXTURE2D(_ColorID); SAMPLER(sampler_ColorID);

            float4 _TexA_ST;
            float4 _TexB_ST;
            float4 _TexC_ST;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uvA = TRANSFORM_TEX(IN.uv, _TexA);
                float2 uvB = TRANSFORM_TEX(IN.uv, _TexB);
                float2 uvC = TRANSFORM_TEX(IN.uv, _TexC);
                float2 uvMask = IN.uv;

                float3 mask = SAMPLE_TEXTURE2D(_ColorID, sampler_ColorID, uvMask).rgb;

                float3 texA = SAMPLE_TEXTURE2D(_TexA, sampler_TexA, uvA).rgb;
                float3 texB = SAMPLE_TEXTURE2D(_TexB, sampler_TexB, uvB).rgb;
                float3 texC = SAMPLE_TEXTURE2D(_TexC, sampler_TexC, uvC).rgb;

                float3 finalColor = texA * mask.r + texB * mask.g + texC * mask.b;

                return half4(finalColor, 1.0);
            }

            ENDHLSL
        }
    }
}
