Shader "Game/Telegraph/Box"
{
    Properties
    {
        _Color ("Fill Color", Color) = (1, 0, 0, 0.3)
        _OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        _FillAmount ("Fill Amount", Range(0, 1)) = 0
        _Width ("Width", Float) = 1
        _Height ("Height", Float) = 1
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
            float _Width;
            float _Height;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 localUV = i.uv - center;

                // 박스 내부 판정
                float2 halfSize = float2(0.5, 0.5);
                float2 d = abs(localUV) - halfSize;
                bool inside = max(d.x, d.y) <= 0;

                if (!inside)
                    return fixed4(0, 0, 0, 0);

                // 중심에서 가장자리까지의 정규화된 거리 (0~1)
                // 각 축에서 가장자리까지의 비율 중 큰 값 사용
                float2 normalizedPos = abs(localUV) / halfSize;
                float dist = max(normalizedPos.x, normalizedPos.y);

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
                    return fixed4(_Color.rgb, _Color.a * 0.2);
                }
            }
            ENDCG
        }
    }
}
