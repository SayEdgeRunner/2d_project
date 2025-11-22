Shader "Game/Telegraph/Capsule"
{
    Properties
    {
        _Color ("Fill Color", Color) = (1, 0, 0, 0.3)
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        _FillAmount ("Fill Amount", Range(0, 1)) = 0
        _Length ("Length", Float) = 1
        _Radius ("Radius", Float) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
            };

            float4 _Color;
            float4 _OutlineColor;
            float _OutlineWidth;
            float _FillAmount;
            float _Length;
            float _Radius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 캡슐 SDF (Signed Distance Function)
            float capsuleSDF(float2 p, float2 a, float2 b, float r)
            {
                float2 pa = p - a;
                float2 ba = b - a;
                float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
                return length(pa - ba * h) - r;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 localUV = i.uv - center;

                // 캡슐 파라미터 (UV 공간)
                // 가로로 긴 캡슐: 양쪽 끝점이 좌우에 위치
                float halfLength = 0.3; // 캡슐 중심선의 절반 길이
                float radius = 0.2;     // 캡슐 반지름

                float2 pointA = float2(-halfLength, 0);
                float2 pointB = float2(halfLength, 0);

                // 캡슐 SDF (음수 = 내부, 0 = 경계, 양수 = 외부)
                float sdf = capsuleSDF(localUV, pointA, pointB, radius);

                // 캡슐 외부는 투명
                if (sdf > 0)
                    return fixed4(0, 0, 0, 0);

                // 중심에서 가장자리까지의 정규화된 거리 (0~1)
                // sdf가 -radius일 때 0 (중심), 0일 때 1 (가장자리)
                float normalizedDist = 1.0 + (sdf / radius);

                // 외곽선 판정
                float outlineThreshold = 1.0 - _OutlineWidth * 5;
                bool isOutline = normalizedDist > outlineThreshold;

                // Fill 영역 판정 (중심에서 바깥으로: normalizedDist <= fillAmount)
                bool isFilled = normalizedDist <= _FillAmount && _FillAmount > 0;

                // 색상 결정
                if (isOutline)
                {
                    return _OutlineColor;
                }
                else if (isFilled)
                {
                    return _Color;
                }
                else
                {
                    return fixed4(_Color.rgb, _Color.a * 0.2);
                }
            }
            ENDCG
        }
    }
}
