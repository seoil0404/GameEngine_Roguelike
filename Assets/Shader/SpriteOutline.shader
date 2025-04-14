Shader "CustomShader/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness", Float) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineThickness;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float alpha = tex2D(_MainTex, uv).a;
                float outline = 0.0;

                float thickness = _OutlineThickness * _MainTex_TexelSize.x;

                // 더 정밀한 외곽선: 16 방향으로 확인
                for (float angle = 0.0; angle < 6.2831853; angle += 0.392699) // 0 ~ 2π, 약 45도 간격
                {
                    float2 offset = float2(cos(angle), sin(angle)) * thickness;
                    outline = max(outline, tex2D(_MainTex, uv + offset).a);
                }

                fixed4 col = tex2D(_MainTex, uv);
                fixed4 result = col;

                if (alpha == 0 && outline > 0)
                {
                    result = _OutlineColor;
                    result.a = outline;
                }

                return result;
            }
            ENDCG
        }
    }
}
