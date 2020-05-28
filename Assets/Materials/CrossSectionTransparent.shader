Shader "Bodylogical/XRayCS" {
	Properties {
		_Color("Color", Color) = (1, 1, 1, 1)
		_CrossColor("Cross Section Color", Color) = (1, 1, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}
		_PlaneNormal("PlaneNormal",Vector) = (0, 1, 0, 0)
		_PlanePosition("PlanePosition",Vector) = (0, 0, 0, 1)
        _AlphaScale ("Alpha Scale", Range(0, 1)) = 1
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }

        Pass {
			Tags { "LightMode" = "ForwardBase" }
            Cull Back
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _AlphaScale;

            fixed3 _PlaneNormal;
            fixed3 _PlanePosition;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float3 worldNormal: TEXCOORD1;
                float3 worldPos: TEXCOORD2;
                float4 vertex : SV_POSITION; 
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            bool checkVisability(fixed3 worldPos) {
                float dotProd1 = dot(worldPos - _PlanePosition, _PlaneNormal);
                return dotProd1 > 0;
            }

            fixed4 frag (v2f i) : SV_Target {
                if (checkVisability(i.worldPos)) {
                    discard;
                }

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed3 albedo = texColor.rgb * _Color.rgb;
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
                fixed3 diffuse  =_LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));
                return fixed4(ambient + diffuse, texColor.a * _AlphaScale);
            }
            ENDCG
		}

        Pass {
            ZWrite On
            ColorMask 0
        }

        Pass {
			Tags { "LightMode" = "ForwardBase" }
            Cull Front
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            fixed _AlphaScale;
            fixed4 _CrossColor;
            fixed3 _PlaneNormal;
            fixed3 _PlanePosition;

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float3 worldPos: TEXCOORD0;
                float4 vertex : SV_POSITION; 
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            bool checkVisability(fixed3 worldPos) {
                float dotProd1 = dot(worldPos - _PlanePosition, _PlaneNormal);
                return dotProd1 > 0;
            }

            fixed4 frag (v2f i) : SV_Target {
                if (checkVisability(i.worldPos)) {
                    discard;
                }

                fixed3 ambient = _CrossColor.rgb;
                return fixed4(ambient, 0.3f);
            }
            ENDCG
		}
	}
	FallBack "Transparent/VertexLit"
}
