Shader "Unlit/ComputeNormal"
{
    Properties
    {
        _Heightfield ("Height Field", 2D) = "white" {}
        _HeightScale ("Height Scale", Float) = 0.01
        _TexelSize ("Texel Size", Float) = 0.003906
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _Heightfield;
            float _HeightScale;
            float _TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv1 = i.uv + float2( 1.0,  0.0) * _TexelSize;
                float2 uv2 = i.uv + float2(-1.0,  0.0) * _TexelSize;
                float2 uv3 = i.uv + float2( 0.0,  1.0) * _TexelSize;
                float2 uv4 = i.uv + float2( 0.0, -1.0) * _TexelSize;

                float2 texelVal = (float2(tex2D(_Heightfield, uv1).r, tex2D(_Heightfield, uv3).r) - float2(tex2D(_Heightfield, uv2).r, tex2D(_Heightfield, uv4).r)) * _HeightScale;
                float3 col = normalize(cross(float3(_TexelSize * 2.0, 0.0, texelVal.r), float3(0.0, _TexelSize * 2.0, texelVal.g)));

                return fixed4(col, 1.0f);
            }
            ENDCG
        }
    }
}
