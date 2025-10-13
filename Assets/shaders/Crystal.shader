Shader "Custom/GemCrystal"
{
    Properties
    {
        _Color ("Gem Color", Color) = (0.3,0.7,1,0.5)
        _Transparency ("Transparency", Range(0,1)) = 0.5
        _RefractionStrength ("Refraction Strength", Range(0,0.5)) = 0.05
        _Glossiness ("Glossiness", Range(0,1)) = 0.9
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 300

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            float4 _Color;
            float _Transparency;
            float _RefractionStrength;
            float _Glossiness;
            float4 _SpecColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Fresnel для краев
                float fresnel = pow(1.0 - saturate(dot(i.viewDir, i.worldNormal)), 3.0);

                // Простейшая имитация refraction через сдвиг нормали
                float3 refractDir = refract(i.viewDir, normalize(i.worldNormal), 1.0/1.5);
                float2 offset = refractDir.xy * _RefractionStrength;

                // Основной цвет кристалла + Fresnel
                fixed4 color = _Color;
                color.rgb += _SpecColor.rgb * fresnel;
                color.a *= _Transparency;

                return color;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
