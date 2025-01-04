Shader "Custom/Outline"
{
    Properties
    {
        // 인스펙터에 표시할 속성 추가
        _OutlineWidth("Outline Width", Range(0.0, 1.0)) = 0.1
        _OutlineColor("Outline Color", Color) = (1, 0, 0, 1)  // 외곽선 색상
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass {
            Name"OUTLINE"
            Tags { "LightMode"="Always" }

            ZWrite On
            Cull Front
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : POSITION;
            };

            // Inspector에서 설정할 수 있도록 변수 정의
            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.pos.xy += v.normal.xy * _OutlineWidth; // 외곽선 두께 설정
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                return _OutlineColor;  // 외곽선 색상 반환
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}