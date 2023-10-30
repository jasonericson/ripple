Shader "Unlit/ForceSplat"
{
    Properties
    {
        _ForcePosition ("Force Position", Color) = (0.5, 0.5, 0.0, 0.0)
        _ForceSize ("Force Size", Float) = 0.02
        _ForceStrength ("Force Strength", Float) = 1.0
        _Heightfield ("Height Field", 2D) = "white" {}
        _Thing ("Thing", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Additive" }
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

            float4 _ForcePosition;
            float _ForceSize;
            float _ForceStrength;
            sampler2D _Heightfield;
            float _Thing;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = (clamp((1.0 - length(i.uv - pow(_ForcePosition, 0.454545).xy)) - (1.0 - _ForceSize), 0.0, 1.0) / _ForceSize) * _ForceStrength;
                return col + tex2D(_Heightfield, 1.0 - i.uv);
            }
            ENDCG
        }
    }
}
