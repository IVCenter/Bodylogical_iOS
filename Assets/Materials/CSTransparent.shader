Shader "Bodylogical/CSTransparent" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_CrossColor("Cross Section Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_PlaneNormal("PlaneNormal",Vector) = (0,1,0,0)
		_PlanePosition("PlanePosition",Vector) = (0,0,0,1)
		_StencilMask("Stencil Mask", Range(0, 255)) = 255
		_AlphaScale ("Alpha Scale", Range(0, 1)) = 1
		[Toggle] _RenderBack("Render Back", Int) = 0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
		//LOD 200
		Stencil {
			Ref [_StencilMask]
			CompBack Always
			PassBack Replace

			CompFront Always
			PassFront Zero
		}

		Pass {
			ZWrite On
			ColorMask 0
        }

		// First render back side
		Cull Front
		CGPROGRAM
		#pragma surface surf NoLighting noambient

		struct Input {
			float3 worldPos;
		};

		fixed4 _CrossColor;
		fixed3 _PlaneNormal;
		fixed3 _PlanePosition;
		int _RenderBack;

		bool checkVisibility(fixed3 worldPos) {
			float dotProd1 = dot(worldPos - _PlanePosition, _PlaneNormal);
			return !_RenderBack || dotProd1 > 0;
		}

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			if (checkVisibility(IN.worldPos)) {
				discard;
			}
			o.Albedo = _CrossColor;
			o.Alpha = 1;
		}
		ENDCG

		// Then render front side
		Cull Back
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NormalMap;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _CrossColor;
		fixed3 _PlaneNormal;
		fixed3 _PlanePosition;
		fixed _AlphaScale;
		fixed _RenderBack;

		bool checkVisibility(fixed3 worldPos) {
			float dotProd1 = dot(worldPos - _PlanePosition, _PlaneNormal);
			return _RenderBack && dotProd1 > 0;
		}

		void surf(Input IN, inout SurfaceOutputStandard o) {
			if (checkVisibility(IN.worldPos)) {
				discard;
			}
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _RenderBack ? 1 : _AlphaScale;
		}
		ENDCG
		
	}
	//FallBack "Standard"
}
