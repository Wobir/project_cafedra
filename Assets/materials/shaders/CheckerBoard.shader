Shader "Custom/LocalCheckerboard"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,1)
        _CheckerSize ("Checker Size (local units)", Float) = 1
        _Offset ("Offset", Vector) = (0,0,0,0)
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        fixed4 _ColorA;
        fixed4 _ColorB;
        float _CheckerSize;
        float4 _Offset;
        float _Smoothness;
        float _Metallic;

        struct Input
        {
            float3 localPos; // Локальные координаты объекта
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            // Используем локальные координаты модели
            o.localPos = v.vertex.xyz;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Применяем смещение и масштаб
            float3 p = (IN.localPos + _Offset.xyz) / max(0.0001, _CheckerSize);

            // Двумерная шахматная сетка в локальных координатах XZ
            float fx = floor(p.x);
            float fz = floor(p.z);
            float s = fmod(fx + fz, 2.0);
            if (s < 0) s += 2.0;

            float mask = step(1.0, s);
            float3 color = lerp(_ColorA.rgb, _ColorB.rgb, mask);

            o.Albedo = color;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = 1;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
