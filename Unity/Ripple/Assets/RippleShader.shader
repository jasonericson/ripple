Shader "Unlit/RippleShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaterColor ("Water Color", Color) = (0.0, 0.25, 0.75, 1)
        _GradientTex ("Gradient Texture", 2D) = "white" {}
        _Heightfield ("Heightfield", 2D) = "white" {}
        _HeightMax ("Height Max", Float) = 2.0
        _HeightScale ("Height Scale", Float) = 10.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _WaterColor;
            sampler2D _GradientTex;
            sampler2D _Heightfield;
            float _HeightMax;
            float _HeightScale;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float height = clamp(tex2Dlod(_Heightfield, float4(v.uv, 0.0, 0.0)).r * _HeightScale, 0.0, _HeightMax);
                v.vertex.y += height;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvGrad = float2(i.uv.y, 1.0 - i.uv.x);
                float4 colorMain = tex2D(_MainTex, uvGrad);

                float heightNormalized = clamp(tex2D(_Heightfield, i.uv).r * _HeightScale, 0.0, _HeightMax) / _HeightMax;
                float2 uvHeight = float2(heightNormalized, heightNormalized);
                float4 colorHeight = tex2D(_GradientTex, uvHeight);

                float4 colorCombined = lerp(_WaterColor, colorMain, colorHeight.r);
                fixed4 colorFinal = lerp(float4(1.0, 1.0, 1.0, 1.0), colorCombined, colorHeight.a);

                return colorFinal;
            }
            ENDCG
        }
    }
}
