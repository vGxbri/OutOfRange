Shader "UI/Blur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Size ("Blur Size", Range(0, 20)) = 5
        _Color ("Overlay Tint", Color) = (1,1,1,1)

        [Header(Stencil)]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        ColorMask [_ColorMask]
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass { "_BackgroundTexture" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 grabPos : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _BackgroundTexture;
            float4 _BackgroundTexture_TexelSize;
            float _Size;
            float4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                // Preserve the exact alpha and tint of the UI Image component
                o.color = v.color * _Color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 sum = half4(0,0,0,0);

                #define BLUR_SAMPLE(offset_x, offset_y, weight) \
                    sum += tex2Dproj(_BackgroundTexture, i.grabPos + float4(offset_x * _BackgroundTexture_TexelSize.x * _Size, offset_y * _BackgroundTexture_TexelSize.y * _Size, 0, 0)) * weight;

                // 5x5 Gaussian Blur Aproximado
                BLUR_SAMPLE(-2, -2, 0.023)
                BLUR_SAMPLE(-1, -2, 0.043)
                BLUR_SAMPLE( 0, -2, 0.054)
                BLUR_SAMPLE( 1, -2, 0.043)
                BLUR_SAMPLE( 2, -2, 0.023)

                BLUR_SAMPLE(-2, -1, 0.043)
                BLUR_SAMPLE(-1, -1, 0.080)
                BLUR_SAMPLE( 0, -1, 0.098)
                BLUR_SAMPLE( 1, -1, 0.080)
                BLUR_SAMPLE( 2, -1, 0.043)

                BLUR_SAMPLE(-2,  0, 0.054)
                BLUR_SAMPLE(-1,  0, 0.098)
                BLUR_SAMPLE( 0,  0, 0.122)
                BLUR_SAMPLE( 1,  0, 0.098)
                BLUR_SAMPLE( 2,  0, 0.054)

                BLUR_SAMPLE(-2,  1, 0.043)
                BLUR_SAMPLE(-1,  1, 0.080)
                BLUR_SAMPLE( 0,  1, 0.098)
                BLUR_SAMPLE( 1,  1, 0.080)
                BLUR_SAMPLE( 2,  1, 0.043)

                BLUR_SAMPLE(-2,  2, 0.023)
                BLUR_SAMPLE(-1,  2, 0.043)
                BLUR_SAMPLE( 0,  2, 0.054)
                BLUR_SAMPLE( 1,  2, 0.043)
                BLUR_SAMPLE( 2,  2, 0.023)

                // Multiplicamos por el color del Image de Unity (para poder controlar la opacidad desde ahí)
                return sum * i.color;
            }
            ENDCG
        }
    }
}
