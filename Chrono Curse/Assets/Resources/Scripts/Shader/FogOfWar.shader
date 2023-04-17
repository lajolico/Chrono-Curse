Shader "Custom/FogOfWar" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthMap ("Depth Map", 2D) = "white" {}
    }

    SubShader {
        Pass {
            Tags { "RenderType"="Opaque" }
            ZWrite On
            ColorMask RGB

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _DepthMap;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Sample the darkness texture
                fixed4 darkness = tex2D(_MainTex, i.uv);

                // Sample the depth map
                float depth = tex2D(_DepthMap, i.uv).r;

                // Apply the depth mask to the darkness texture
                fixed4 color = darkness;
                color.a *= smoothstep(0.5, 1.0, depth);

                return color;
            }
            ENDCG
        }
    }
}
