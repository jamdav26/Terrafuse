// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PortalAdditive"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _IntensityAndScrolling("Intensity (XY), Scrolling (ZW)", Vector) = (0.1,0.1,0.1,0.1)
    }
    SubShader
    {
        Tags { 
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend One One
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _NoiseTex;
            uniform float4 _MainTex_ST;
            uniform float4 _NoiseTex_ST;
            uniform float4 _IntensityAndScrolling;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 texcoord0 : TEXCOORD1;
                float2 texcoord1 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
                float4 vertexColor : COLOR;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.uv0 = TRANSFORM_TEX(v.texcoord0, _MainTex);
                o.uv1 = TRANSFORM_TEX(v.texcoord1, _NoiseTex);
                o.uv1 += _Time.yy * _IntensityAndScrolling.zw;
                o.vertexColor = v.vertexColor;
                o.vertex += UnityObjectToClipPos(v.vertex);
 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {


                float4 noiseTex = tex2D(_NoiseTex, i.uv1);
                float2 offset = (noiseTex.rg * 2 - 1) * _IntensityAndScrolling.rg;
                float2 uvNoise = i.uv0 + offset;
                float4 mainTex = tex2D(_MainTex, uvNoise);
                float3 emissive = (mainTex.rgb * i.vertexColor.rgb) * (mainTex.a * i.vertexColor.a);
 
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog

                return fixed4(emissive, 1);

            }
            ENDCG
        }
    }
}
