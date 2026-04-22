Shader "Custom/skydomeblend"
{
    Properties
    {
        _DayTex ("Day Texture", 2D) = "white" {}
        _NightTex ("Night Texture", 2D) = "black" {}
        _Blend ("Blend", Range(0, 1)) = 0
        _TopColor ("Top Color", Color) = (0.30, 0.60, 1.00, 1)
        _BottomColor ("Bottom Color", Color) = (0.85, 0.92, 1.00, 1)
        _CloudStrength ("Cloud Strength", Range(0, 2)) = 1
        _TextureBrightness ("Texture Brightness", Range(0, 2)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "Universal Forward"
            Tags { "LightMode"="UniversalForward" }

            Cull Off
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_DayTex);
            SAMPLER(sampler_DayTex);

            TEXTURE2D(_NightTex);
            SAMPLER(sampler_NightTex);

            float4 _TopColor;
            float4 _BottomColor;
            float _Blend;
            float _CloudStrength;
            float _TextureBrightness;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float screenY : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.screenY = OUT.positionHCS.y;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float y = IN.screenY / _ScaledScreenParams.y;
                y = saturate(y);

                float3 gradient = lerp(_BottomColor.rgb, _TopColor.rgb, y);

                float3 dayCol = SAMPLE_TEXTURE2D(_DayTex, sampler_DayTex, IN.uv).rgb;
                float3 nightCol = SAMPLE_TEXTURE2D(_NightTex, sampler_NightTex, IN.uv).rgb;

                float3 texCol = lerp(dayCol, nightCol, _Blend);
                texCol *= _CloudStrength * _TextureBrightness;

                float3 finalCol = saturate(gradient + texCol * 0.5);

                return half4(finalCol, 1);
            }
            ENDHLSL
        }
    }
}