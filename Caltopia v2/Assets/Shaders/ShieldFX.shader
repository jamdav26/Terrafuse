// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/ShieldFX"
{
	Properties
	{
		_MainColor("MainColor", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_Fresnel("Fresnel Intensity", Range(0,200)) = 3.0
		_FresnelWidth("Fresnel Width", Range(0,2)) = 3.0
		_Distort("Distort", Range(0, 100)) = 1.0
		_IntersectionThreshold("Highlight of intersection threshold", range(0,1)) = .1 //Max difference for intersections
		_ScrollSpeedU("Scroll U Speed",float) = 2
		_ScrollSpeedV("Scroll V Speed",float) = 0
        _SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Shininess", Range (0.01, 1)) = 0.078125
        _Gloss ("Gloss", Range (0.0, 2.0)) = 1
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _GlossMap ("Gloss Map", 2D) = "gloss" {}
        _OcclusionMap("Occlusion", 2D) = "white" {}
		//[ToggleOff]_CullOff("Cull Front Side Intersection",float) = 1
	}
	SubShader
	{ 
		Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		GrabPass{ "_GrabTexture" }
		Pass
		{
			Lighting Off ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				fixed4 vertex : POSITION;
				fixed4 normal: NORMAL;
				fixed3 uv : TEXCOORD0;
                float4 tangent : TANGENT;
			};

			struct v2f
			{
				fixed2 uv : TEXCOORD0;
				fixed4 vertex : SV_POSITION;
				fixed3 rimColor :TEXCOORD1;
				fixed4 screenPos: TEXCOORD2;
                half3 worldNormal : TEXCOORD3;
                half3 worldPos : TEXCOORD4;
                // these three vectors will hold a 3x3 rotation matrix
                // that transforms from tangent to world space
                half3 tspace0 : TEXCOORD5; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD6; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD7; // tangent.z, bitangent.z, normal.z
			};

			sampler2D _MainTex, _CameraDepthTexture, _GrabTexture, _BumpMap, _GlossMap, _OcclusionMap;
			fixed4 _MainTex_ST,_MainColor,_GrabTexture_ST, _GrabTexture_TexelSize;
			fixed _Fresnel, _FresnelWidth, _Distort, _IntersectionThreshold, _ScrollSpeedU, _ScrollSpeedV;
            half _Shininess, _Gloss;
 

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // compute world space position of the vertex
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
                
				//scroll uv
				o.uv.x += _Time * _ScrollSpeedU;
				o.uv.y += _Time * _ScrollSpeedV;

				//fresnel 
				fixed3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				fixed dotProduct = 1 - saturate(dot(v.normal, viewDir));
				o.rimColor = smoothstep(1 - _FresnelWidth, 1.0, dotProduct) * .5f;
				o.screenPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.screenPos.z);//eye space depth of the vertex 
				return o;
			}
			
			fixed4 frag (v2f i,fixed face : VFACE) : SV_Target
			{
				//intersection
				fixed intersect = saturate((abs(LinearEyeDepth(tex2Dproj(_CameraDepthTexture,i.screenPos).r) - i.screenPos.z)) / _IntersectionThreshold);

				fixed3 main = tex2D(_MainTex, i.uv);
				//distortion
				i.screenPos.xy += (main.rg * 2 - 1) * _Distort * _GrabTexture_TexelSize.xy;
				fixed3 distortColor = tex2Dproj(_GrabTexture, i.screenPos);
				distortColor *= _MainColor * _MainColor.a + 1;

				//intersect hightlight
				i.rimColor *= intersect * clamp(0,1,face);
				main *= _MainColor * pow(_Fresnel,i.rimColor) ;
				
				//lerp distort color & fresnel color
				main = lerp(distortColor, main, i.rimColor.r);

                

                // sample the normal map, and decode from the Unity encoding
                half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
                // transform normal from tangent to world space
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);

                // rest the same as in previous shader
                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                half3 worldRefl = reflect(-worldViewDir, worldNormal);
                half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
                half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);
                fixed4 c = 0;
                c.rgb = skyColor;

                // output it!
                main.rgb += skyColor;
                main += (1 - intersect) * (face > 0 ? .03:.3) * _MainColor * _Fresnel;
                fixed3 baseColor = tex2D(_MainTex, i.uv).rgb;
                fixed occlusion = tex2D(_OcclusionMap, i.uv).r;
                c.rgb *= baseColor;
                c.rgb *= occlusion;
				return fixed4(main,.9);
			}
			ENDCG
		}
	}
}
