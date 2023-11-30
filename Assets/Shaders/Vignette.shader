// Code originally from https://lindenreidblog.com/2018/02/05/camera-shaders-unity/
// Changes then made by the Developer

Shader "GO2023/VignetteSprite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Tint("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		_Color("Color", Color) = (1, 1, 1, 1)
		_VRadius("Vignette Radius", Range(0.0, 1.0)) = 0.8
		_VSoft("Vignette Softness", Range(0.0, 1.0)) = 0.5
	}
	SubShader
	{
		Blend OneMinusSrcAlpha SrcAlpha

		Tags{ "Queue" = "Transparent"}
		GrabPass{}
		
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

			sampler2D _GrabTexture;
			sampler2D _MainTex;
			float4 _MainTex_ST;
            float4 _Tint;
			
			v2f vert (appdata v)
			{
				v2f o;

				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif

				o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex *= scale;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_GrabTexture, i.uv);
				return col;
			}
			ENDCG
		}
		

		Pass
		{


			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
            #include "UnityCG.cginc"
			
			// Properties
			sampler2D _MainTex;
			float4 _Color;
			float4 _GlitchColor;
			float _VRadius;
			float _VSoft;

			float4 frag(v2f_img input) : COLOR
			{
				float4 base = tex2D(_MainTex, input.uv);

				// add vignette
				float radius = 1.0 - _VRadius;
				float distFromCenter = distance(input.uv.xy, float2(0.5, 0.5));
				float vignette = smoothstep(radius, radius - _VSoft, distFromCenter);
				base = saturate(base * vignette);
				base.rgb = float3(0.0, 0.0, 0.0);
				base.a = base.a < 0.1 ? 0 : base.a;

				return base;
			}

			ENDCG
		}

	}
}
