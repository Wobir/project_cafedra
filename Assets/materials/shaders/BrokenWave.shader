Shader "Custom/AngularWavePBR_Hard"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _WaveColor("Wave Color", Color) = (1,0,0,1)
        _WaveBlend("Wave Blend", Range(0,1)) = 0.5
        _WaveFrequency("Wave Frequency", Float) = 5.0
        _WaveDistortion("Wave Distortion", Float) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Smoothness("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        struct Input
        {
            float3 localPos;
        };

        fixed4 _BaseColor;
        fixed4 _WaveColor;
        float _WaveBlend;
        float _WaveFrequency;
        float _WaveDistortion;
        float _Metallic;
        float _Smoothness;

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.localPos = v.vertex;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float distortion = sin(IN.localPos.x * _WaveDistortion * 10.0) * 0.5;

            // Угловатая ломаная волна (треугольная)
            float t = frac((IN.localPos.y + distortion) * _WaveFrequency);
            float wave = step(0.5, t); // резкий переход на 0 или 1

            fixed4 col = lerp(_BaseColor, _WaveColor, wave * _WaveBlend);

            o.Albedo = col.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Standard"
}
