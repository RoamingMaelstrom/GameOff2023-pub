Shader "GO2023/PlayerShader"
{
	Properties
    {
		_MainTex ("Texture", 2D) = "white" {}
        _RotationSpeed("Rotation Speed", Range(0, 10)) = 2
	}

	SubShader
    {

		Tags{"RenderType"="Transparent"  "Queue"="Transparent"}

		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		Cull Off // Needed to enable sprite flipping

		Pass
        {
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;
            float _RotationSpeed;

			struct appdata
            {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
            {
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			v2f vert(appdata v){
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}

            // Modified version of Shadertoy code https://www.shadertoy.com/view/Xscyzn
            //---  Swirl distortion effect --- laserdog  February 2018

            #define PI 3.14159
            #define EFFECT_ANGLE 1.25 * PI
            #define EFFECT_RADIUS 0.45

			fixed4 frag(v2f i) : SV_TARGET
            {
                float2 uv_0d = i.uv - float2(0.5, 0.5); // Adjustment needed for math to work.

                float len = length(uv_0d);
                float angle = atan(uv_0d.y / uv_0d.x) + (EFFECT_ANGLE * smoothstep(EFFECT_RADIUS, 0.0, len));
                angle = uv_0d.x > 0 ? angle + (PI) : angle;
                float radius = length(uv_0d);
                float time = _Time.x * _RotationSpeed;

                fixed4 col = tex2D(_MainTex, float2(cos(angle + time) * radius, sin(angle + time) * radius) + float2(0.5, 0.5));

                col.a = max(col.b, col.g) * 2.0;
                col.a = length(uv_0d) < 0.1 ? 0 : col.a;
                col = length(uv_0d) > EFFECT_RADIUS ? float4(0.0, 0.0, 0.0, 0.0) : col;

                col *= i.color;
				return col;
			}
            
			ENDCG
		}
	}
}