Shader "Custom/URP_FlowerPetals"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)

        _PetalCount("Petal Count", Float) = 6
        _Radius("Radius", Float) = 0.4
        _PetalDepth("Petal Depth", Float) = 0.15
        _PetalSharpness("Petal Sharpness", Float) = 2.5

        _OpenClose("Open/Close", Range(0,1)) = 1
        _WaveSpeed("Wave Speed", Float) = 1.0
        _WaveAmount("Wave Amount", Float) = 0.05

        _BendUp("Bend Up", Float) = 0.15
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _PetalCount;
                float _Radius;
                float _PetalDepth;
                float _PetalSharpness;
                float _OpenClose;
                float _WaveSpeed;
                float _WaveAmount;
                float _BendUp;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 posOS = IN.positionOS.xyz;

                // Treat XZ as "around the stem" plane.
                float2 p = posOS.xz;
                float r = length(p);
                float ang = atan2(p.y, p.x);  // -pi..pi

                // Petal pattern: lobes around the center
                // Normalize to 0..1-ish, then sharpen.
                float petals = 0.5 + 0.5 * cos(ang * _PetalCount);
                petals = pow(saturate(petals), _PetalSharpness);

                // A soft falloff so the center doesn't explode
                // (assumes the mesh roughly fits within _Radius in XZ)
                float centerToEdge = saturate(r / max(_Radius, 1e-4));

                // Optional subtle animation rippling the petals
                float t = _Time.y;
                float wave = sin(t * _WaveSpeed + ang * _PetalCount) * _WaveAmount;

                // Radial push: stronger near the outer region, scaled by petal mask
                float push = (petals * _PetalDepth + wave) * _OpenClose * centerToEdge;

                // Push outward in XZ (radial direction)
                float2 dir = (r > 1e-5) ? (p / r) : float2(1, 0);
                posOS.xz += dir * push;

                // Bend slightly upward near the tips to feel more like petals
                posOS.y += push * _BendUp;

                OUT.positionHCS = TransformObjectToHClip(posOS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Simple normal visualization tinted by base color
                float3 n = normalize(IN.normalWS);
                float3 rgb = n * 0.5 + 0.5;
                return half4(rgb, 1) * _BaseColor;
            }
            ENDHLSL
        }
    }
}
