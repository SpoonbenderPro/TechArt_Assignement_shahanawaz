Shader "Battlebuck/Curved Lit Simple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag

            // URP lighting variants
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS      : SV_POSITION;
                float2 uv               : TEXCOORD0;
                half3 normalWS          : TEXCOORD1;
                float3 positionWS       : TEXCOORD2;
                half4 fogFactorAndVertLight : TEXCOORD3; // x=fog, yzw=vertex additional light
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 4);
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half _Smoothness;
            CBUFFER_END

            // Global values from script only
            float _CurveStrength;
            float _CurveStart;

            float3 ApplyCurveToWorld(float3 worldPos, float cameraViewDepth)
            {
                // Bend only in the far horizon zone.
                // cameraViewDepth is positive as things go farther away.
                float bendDist = max(0.0, cameraViewDepth - _CurveStart);
                worldPos.y -= bendDist * bendDist * _CurveStrength;
                return worldPos;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);

                // Use camera-relative depth so the bend starts only far from camera.
                float3 viewPos = TransformWorldToView(worldPos);
                float cameraDepth = max(0.0, -viewPos.z);

                worldPos = ApplyCurveToWorld(worldPos, cameraDepth);

                OUT.positionWS = worldPos;
                OUT.normalWS = normalize(normalWS);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                OUTPUT_LIGHTMAP_UV(IN.uv, unity_LightmapST, OUT.lightmapUV);
                OUTPUT_SH(OUT.normalWS, OUT.vertexSH);

                half fogFactor = ComputeFogFactor(OUT.positionHCS.z);
                half3 vertexLight = VertexLighting(worldPos, OUT.normalWS);
                OUT.fogFactorAndVertLight = half4(fogFactor, vertexLight);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = IN.positionWS;
                lightingInput.normalWS = normalize(IN.normalWS);
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                lightingInput.fogCoord = IN.fogFactorAndVertLight.x;
                lightingInput.vertexLighting = IN.fogFactorAndVertLight.yzw;
                lightingInput.bakedGI = SAMPLE_GI(IN.lightmapUV, IN.vertexSH, lightingInput.normalWS);
                lightingInput.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionHCS.xy);
                lightingInput.shadowMask = half4(1,1,1,1);

                SurfaceData surface = (SurfaceData)0;
                surface.albedo = albedo.rgb;
                surface.alpha = albedo.a;
                surface.metallic = 0.0;
                surface.specular = half3(0.04, 0.04, 0.04);
                surface.smoothness = _Smoothness;
                surface.normalTS = half3(0,0,1);
                surface.occlusion = 1.0;
                surface.emission = 0.0;
                surface.clearCoatMask = 0.0;
                surface.clearCoatSmoothness = 0.0;

                half4 color = UniversalFragmentBlinnPhong(lightingInput, surface);
                color.rgb = MixFog(color.rgb, lightingInput.fogCoord);
                return color;
            }
            ENDHLSL
        }
    }

    FallBack Off
}