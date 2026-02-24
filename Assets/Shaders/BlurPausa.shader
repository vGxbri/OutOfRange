Shader "Custom/BlurPausa"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        // Pass 1: Blur horizontal
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texel = _MainTex_TexelSize.xy * _BlurSize;
                fixed4 col = tex2D(_MainTex, i.uv) * 0.227027;
                col += tex2D(_MainTex, i.uv + float2(texel.x, 0)) * 0.1945946;
                col += tex2D(_MainTex, i.uv - float2(texel.x, 0)) * 0.1945946;
                col += tex2D(_MainTex, i.uv + float2(texel.x * 2, 0)) * 0.1216216;
                col += tex2D(_MainTex, i.uv - float2(texel.x * 2, 0)) * 0.1216216;
                col += tex2D(_MainTex, i.uv + float2(texel.x * 3, 0)) * 0.0540541;
                col += tex2D(_MainTex, i.uv - float2(texel.x * 3, 0)) * 0.0540541;
                col += tex2D(_MainTex, i.uv + float2(texel.x * 4, 0)) * 0.0162162;
                col += tex2D(_MainTex, i.uv - float2(texel.x * 4, 0)) * 0.0162162;
                return col;
            }
            ENDCG
        }

        // Pass 2: Blur vertical
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texel = _MainTex_TexelSize.xy * _BlurSize;
                fixed4 col = tex2D(_MainTex, i.uv) * 0.227027;
                col += tex2D(_MainTex, i.uv + float2(0, texel.y)) * 0.1945946;
                col += tex2D(_MainTex, i.uv - float2(0, texel.y)) * 0.1945946;
                col += tex2D(_MainTex, i.uv + float2(0, texel.y * 2)) * 0.1216216;
                col += tex2D(_MainTex, i.uv - float2(0, texel.y * 2)) * 0.1216216;
                col += tex2D(_MainTex, i.uv + float2(0, texel.y * 3)) * 0.0540541;
                col += tex2D(_MainTex, i.uv - float2(0, texel.y * 3)) * 0.0540541;
                col += tex2D(_MainTex, i.uv + float2(0, texel.y * 4)) * 0.0162162;
                col += tex2D(_MainTex, i.uv - float2(0, texel.y * 4)) * 0.0162162;
                return col;
            }
            ENDCG
        }
    }
}
