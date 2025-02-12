//
// Created :    Spring 2023
// Author :     SeungGeon Kim (keithrek@hanmail.net)
// Project :    FogWar
// Filename :   FogPlane.shader (cg shader)
// 
// All Content (C) 2022 Unlimited Fischl Works, all rights reserved.
//

// This shader is based on an implementation of fog shader by rito15
// https://rito15.github.io/posts/fog-of-war/
// The main difference is that the lerping of fragment shader happens in csFogWar (CPU), not in an additional shader pass

Shader "FogWar/FogPlane"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _BlurOffset("BlurOffset", Range(0, 10)) = 1
    }

    CGINCLUDE

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

    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;
    float4 _Color;
    float _BlurOffset;

    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        // This normalizes the offset to the uv-coordinates scale, having range of [0, 1]
        float offset = _BlurOffset * _MainTex_TexelSize;

        // 3x3 gaussian kernel
        // https://homepages.inf.ed.ac.uk/rbf/HIPR2/gsmooth.htm
        // Above link may be a good reference of what is going on
        half GaussianKernel[9] =
        {
            1,2,1,
            2,4,2,
            1,2,1
        };

        // Color accumulator
        fixed4 col = fixed4(0,0,0,0);

        // UV index slightly going out of range is fine, texture wrap mode (clamp) will deal with that
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                col +=
                tex2D(_MainTex, i.uv + fixed2(x - 1, y - 1) * offset) *
                GaussianKernel[x * 1 + y * 3];
            }
        }

        // Adding up all elements in the 3x3 kernel results in 16
        col /= 16;

        // Add a color to the pixel
        return col * _Color;
    }

    ENDCG

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        CULL BACK
        ZWrite OFF
        ZTest Always
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}