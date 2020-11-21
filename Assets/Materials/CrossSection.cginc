#include <HLSLSupport.cginc>
#include <Lighting.cginc>

bool checkVisibility(fixed3 worldPos, fixed3 planeNormal, fixed3 planePosition) {
    float dotProd1 = dot(worldPos - planePosition, planeNormal);
    return dotProd1 > 0;
}

fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
    return fixed4(s.Albedo, s.Alpha);
}
