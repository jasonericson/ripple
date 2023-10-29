Shader "Unlit/HeightSim"
{
    Properties
    {
        _PreviousHeight1 ("Previous Height 1", 2D) = "white" {}
        _PreviousHeight2 ("Previous Height 2", 2D) = "white" {}
        _Dampening ("Dampening", Float) = 0.98
        _TexelSize ("Texel Size", Float) = 0.003906
        _TravelSpeed ("Travel Speed", Float) = 1.0
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _PreviousHeight1;
            sampler2D _PreviousHeight2;
            float _Dampening;
            float _TexelSize;
            float _TravelSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv1 = i.uv + float2(-1.0,  0.0) * _TexelSize;
                float2 uv2 = i.uv + float2( 1.0,  0.0) * _TexelSize;
                float2 uv3 = i.uv + float2( 0.0, -1.0) * _TexelSize;
                float2 uv4 = i.uv + float2( 0.0,  1.0) * _TexelSize;

                float texelVal = tex2D(_PreviousHeight1, uv1).r + tex2D(_PreviousHeight1, uv2).r + tex2D(_PreviousHeight1, uv3).r + tex2D(_PreviousHeight1, uv4).r;
                float3 baseHeight = tex2D(_PreviousHeight1, i.uv).rgb * 4.0;

                float3 thing = texelVal - baseHeight;
                float prevHeight2Val = tex2D(_PreviousHeight2, i.uv).r;
                float3 col = ((baseHeight + thing * _TravelSpeed) * 0.5 - prevHeight2Val) * _Dampening;

                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }
}
