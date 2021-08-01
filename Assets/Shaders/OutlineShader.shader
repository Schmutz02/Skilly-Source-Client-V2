Shader "Unlit/OutlineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("OutlineColor", Color) = (0,0,0,1)
        _GradientColor ("GradientColor", Color) = (0,0,0,1)
        _BlurAmount ("Blur", Range(1,10)) = 0
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

            float testForColor(v2f pix)
            {
                for (uint i = 0; i < 5; i++)
                {
                    // check left
                    half4 pixColor = tex2D(_MainTex, pix.uv + float2((i + 1) * _MainTex_TexelSize.x, 0));
                    if (pixColor.a != 0)
                    {
                        return 1.0 - sqrt(i / 5.0);
                    }

                    // check right
                    pixColor = tex2D(_MainTex, pix.uv - float2((i + 1) * _MainTex_TexelSize.x, 0));
                    if (pixColor.a != 0)
                    {
                        return 1.0 - sqrt(i / 5.0);
                    }

                    // check up
                    pixColor = tex2D(_MainTex, pix.uv - float2(0, (i + 1) * _MainTex_TexelSize.y));
                    if (pixColor.a != 0)
                    {
                        return 1.0 - sqrt(i / 5.0);
                    }

                    // check down
                    pixColor = tex2D(_MainTex, pix.uv + float2(0, (i + 1) * _MainTex_TexelSize.y));
                    if (pixColor.a != 0)
                    {
                        return 1.0 - sqrt(i / 5.0);
                    }

                    // check left down
                    pixColor = tex2D(_MainTex, pix.uv + float2((i + 1) * _MainTex_TexelSize.x, (i + 1) * _MainTex_TexelSize.y));
                    if (pixColor.a != 0)
                    {
                        return 1.0 - sqrt(i / 5.0);
                    }

                    // check left up
                    pixColor = tex2D(_MainTex, pix.uv + float2((i + 1) * _MainTex_TexelSize.x, -(i + 1) * _MainTex_TexelSize.y));
                    if (pixColor.a != 0)
                    {
                        return 1.0 - sqrt(i / 5.0);
                    }

                    // check right up
                    pixColor = tex2D(_MainTex, pix.uv + float2(-(i + 1) * _MainTex_TexelSize.x, -(i + 1) * _MainTex_TexelSize.y));
                    if (pixColor.a != 0)
                    {
                        return 1.0 - sqrt(i / 5.0);
                    }

                    // check right down
                    pixColor = tex2D(_MainTex, pix.uv + float2(-(i + 1) * _MainTex_TexelSize.x, (i + 1) * _MainTex_TexelSize.y));
                    if (pixColor.a != 0)
                    {
                        return 1.0 - sqrt(i / 5.0);
                    }
                }
                return 0;
            }
            
            half4 frag (v2f IN) : SV_Target
            {
                const half4 texColor = tex2D(_MainTex, IN.uv);
                half4 col = lerp(texColor * _GradientColor, texColor, IN.uv.y);
                
                if (col.a == 0)
                {
                    // float shouldColor = testForColor(IN);
                    //
                    // return lerp(col, lerp(_Color * shouldColor, _Color, shouldColor), ceil(shouldColor));
                    for (int i = -1; i <= 1; i += 2)
                    {
                        for (int j = -1; j <= 1; j += 2)
                        {
                            half4 pixColor = tex2D(_MainTex, IN.uv + float2(j * _MainTex_TexelSize.x, i * _MainTex_TexelSize.y));

                            if (pixColor.a != 0)
                            {
                                return _OutlineColor;
                            }
                        }
                    }
                }
                
                return col;
            }
            ENDCG
        }
    }
}
