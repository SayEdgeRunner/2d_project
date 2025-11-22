Shader "Game/Telegraph/Circle"
{
    Properties
    {
        _Color ("Fill Color", Color) = (1, 0, 0, 0.3)
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        _FillAmount ("Fill Amount", Range(0, 1)) = 0
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
            float _RadiusX;
            float _RadiusY;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // 타원 내부 판정 (1 이하면 내부)
            float ellipseDistance(float2 uv, float2 center, float2 radius)
            {
                float2 diff = uv - center;
                return (diff.x * diff.x) / (radius.x * radius.x) +
                       (diff.y * diff.y) / (radius.y * radius.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 radius = float2(0.5, 0.5); // UV 공간에서 0.5가 최대 반지름

                // 타원까지의 정규화된 거리 (0 = 중심, 1 = 경계)
                float distSq = ellipseDistance(i.uv, center, radius);

                // 타원 외부는 투명
                if (distSq > 1.0)
                    return fixed4(0, 0, 0, 0);

                // 선형화된 거리 (0~1)
                float dist = sqrt(distSq);

                // 외곽선 판정
                float outlineThreshold = 1.0 - _OutlineWidth * 2;
                bool isOutline = dist > outlineThreshold;

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
                    // 채워지지 않은 영역은 더 투명하게
                    return fixed4(_Color.rgb, _Color.a * 0.2);
                }
            }
            ENDCG
        }
    }
}
