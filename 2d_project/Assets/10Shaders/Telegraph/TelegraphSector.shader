Shader "Game/Telegraph/Sector"
{
    Properties
    {
        _Color ("Fill Color", Color) = (1, 0, 0, 0.3)
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        _FillAmount ("Fill Amount", Range(0, 1)) = 0
        _Angle ("Sector Angle", Range(1, 360)) = 90
        _RadiusX ("Radius X", Float) = 1
        _RadiusY ("Radius Y", Float) = 1
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

            #define PI 3.14159265359
            #define DEG2RAD (PI / 180.0)

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
            float _Angle;
            float _RadiusX;
            float _RadiusY;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 타원 내부 판정
            float ellipseDistance(float2 uv, float2 center, float2 radius)
            {
                float2 diff = uv - center;
                return (diff.x * diff.x) / (radius.x * radius.x) +
                       (diff.y * diff.y) / (radius.y * radius.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 radius = float2(0.5, 0.5);

                // 중심으로부터의 방향
                float2 dir = i.uv - center;

                // 타원 거리 (제곱)
                float distSq = ellipseDistance(i.uv, center, radius);

                // 타원 외부는 투명
                if (distSq > 1.0)
                    return fixed4(0, 0, 0, 0);

                // 선형화된 거리 (0~1)
                float dist = sqrt(distSq);

                // 각도 계산 (오른쪽이 0도, 반시계 방향)
                float angle = atan2(dir.y, dir.x);
                float angleDeg = angle / DEG2RAD;

                // 부채꼴 각도 판정 (전방 = 오른쪽 방향 기준)
                float halfAngle = _Angle * 0.5;
                bool inAngle = abs(angleDeg) <= halfAngle;

                // 부채꼴 외부는 투명
                if (!inAngle)
                    return fixed4(0, 0, 0, 0);

                // 외곽선 판정 (호 부분)
                float outlineThreshold = 1.0 - _OutlineWidth * 2;
                bool isOutlineRadius = dist > outlineThreshold;

                // 부채꼴 가장자리 (각도 경계) 외곽선
                float angleFromEdge = halfAngle - abs(angleDeg);
                float edgeThreshold = _OutlineWidth * 50; // 각도 단위로 변환
                bool isOutlineEdge = angleFromEdge < edgeThreshold;

                bool isOutline = isOutlineRadius || isOutlineEdge;

                // Fill 영역 판정 (중심에서 바깥으로: dist <= fillAmount)
                bool isFilled = dist <= _FillAmount && _FillAmount > 0;

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
