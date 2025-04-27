Shader "Custom/LaserBeam"
{
    Properties
    {
        _Color ("Laser Color", Color) = (1,0,0,1)
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Float) = 1
        _Intensity ("Glow Intensity", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha One    // Para efecto de aditivo (brillo)
        ZWrite Off            // No escribir en el z-buffer (para que no tape otras cosas)

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _ScrollSpeed;
            float _Intensity;
           // float _Time;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Hacemos que las UVs se estiren bien con el rectángulo
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Scroll en el eje X para dar efecto de movimiento
                float2 uv = i.uv;
                uv.x += _Time * _ScrollSpeed;

                fixed4 tex = tex2D(_MainTex, uv);

                // Resultado
                return tex * _Color * _Intensity;
            }
            ENDCG
        }
    }
}
