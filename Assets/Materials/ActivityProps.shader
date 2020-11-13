Shader "Bodylogical/ActivityProps" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _CrossColor("Cross Section Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Plane1Normal("Plane 1 Normal",Vector) = (0,1,0,0)
        _Plane1Position("Plane 1 Position",Vector) = (0,0,0,1)
        _Plane2Normal("Plane2 Normal",Vector) = (0,1,0,0)
        _Plane2Position("Plane 2 Position",Vector) = (0,0,0,1)
        _StencilMask("Stencil Mask", Range(0, 255)) = 255
    }
    
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        //LOD 200
        Stencil {
            Ref [_StencilMask]
            CompBack Always
            PassBack Replace

            CompFront Always
            PassFront Zero
        }
        
        // Render front side
        Cull Back
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float3 worldPos;
        };
        
        sampler2D _MainTex;
        sampler2D _NormalMap;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed3 _Plane1Normal;
        fixed3 _Plane1Position;
        fixed3 _Plane2Normal;
        fixed3 _Plane2Position;

        bool checkVisibility(fixed3 worldPos) {
            float dotProd1 = dot(worldPos - _Plane1Position, _Plane1Normal);
            float dotProd2 = dot(worldPos - _Plane2Position, _Plane2Normal);
            return dotProd1 > 0 || dotProd2 > 0;
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
            o.Alpha = c.a;
        }
        ENDCG

        // Render back side
        Cull Front
        CGPROGRAM
        #pragma surface surf NoLighting noambient

        struct Input {
            float3 worldPos;
        };

        fixed4 _CrossColor;
        fixed3 _Plane1Normal;
        fixed3 _Plane1Position;
        fixed3 _Plane2Normal;
        fixed3 _Plane2Position;

        bool checkVisibility(fixed3 worldPos) {
            float dotProd1 = dot(worldPos - _Plane1Position, _Plane1Normal);
            float dotProd2 = dot(worldPos - _Plane2Position, _Plane2Normal);
            return dotProd1 > 0 || dotProd2 > 0;
        }

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            return fixed4(s.Albedo, s.Alpha);
        }

        void surf(Input IN, inout SurfaceOutput o) {
            if (checkVisibility(IN.worldPos)) {
                discard;
            }
            o.Albedo = _CrossColor;
        }
        ENDCG

    }
    
    FallBack "Standard"
}