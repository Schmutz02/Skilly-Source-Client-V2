Shader "Unlit/Outline Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("OutlineColor", Color) = (0,0,0,1)
        _GradientColor ("GradientColor", Color) = (0,0,0,1)
        _BlurAmount ("Blur", Range(1,10)) = 0
        _YOffset ("YOffset", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

        ZWrite Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                return o;
            }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            half4 _OutlineColor;
            half4 _GradientColor;
            uint _BlurAmount;
            fixed _YOffset;
            
            half4 frag (v2f IN) : SV_Target
            {
                const half4 tex_color = tex2D(_MainTex, IN.uv + float2(0, _YOffset));
                float4 col = lerp(tex_color * _GradientColor, tex_color, IN.uv.y);

                if (IN.uv.y > 1 - _YOffset)
                {
                    col.a = 0;
                    return col;
                }
                
                if (col.a == 0)
                {
                    half4 pix_color = tex2D(_MainTex, IN.uv + float2(-1 * _MainTex_TexelSize.x, -1 * _MainTex_TexelSize.y) + float2(0, _YOffset));
                    if (pix_color.a != 0)
                        return _OutlineColor;

                    pix_color = tex2D(_MainTex, IN.uv + float2(1 * _MainTex_TexelSize.x, -1 * _MainTex_TexelSize.y) + float2(0, _YOffset));
                    if (pix_color.a != 0)
                        return _OutlineColor;

                    pix_color = tex2D(_MainTex, IN.uv + float2(-1 * _MainTex_TexelSize.x, 1 * _MainTex_TexelSize.y) + float2(0, _YOffset));
                    if (pix_color.a != 0)
                        return _OutlineColor;

                    pix_color = tex2D(_MainTex, IN.uv + float2(1 * _MainTex_TexelSize.x, 1 * _MainTex_TexelSize.y) + float2(0, _YOffset));
                    if (pix_color.a != 0)
                        return _OutlineColor;
                }
                
                return col;
            }
            ENDCG
        }
    }
}
