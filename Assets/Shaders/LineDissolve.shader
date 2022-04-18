Shader "Unlit/LineDissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveStrength ("Dissolve Strength", Float) = 0
        _EdgeAmount ("Edge Amount", Float) = 0.1
        _EdgeColor ("Edge Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 dissolve_uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;

            float4 _EdgeColor;
            float _DissolveAmount;
            float _DissolveStrength;
            float _EdgeAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.dissolve_uv = TRANSFORM_TEX(v.uv, _DissolveTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                const float dissolve_samp = tex2D(_DissolveTex, float2(i.dissolve_uv.x, 0)).r;
                const float dissolve = pow(dissolve_samp + (_DissolveAmount * 2 - 1), _DissolveStrength);

                const float edge = 1.0f - abs(i.uv.y * 2 - 1);
                const float t = 1.0 - saturate(abs((edge - dissolve) / _EdgeAmount));
                clip(edge - dissolve);
                
                return float4(lerp(col, _EdgeColor, smoothstep(0.45, 0.55, t)));
            }
            ENDCG
        }
    }
}
