// Modified version of some code provided on Unity Forums https://forum.unity.com/threads/simple-optimized-blur-shader.185327/ by User "Cician" https://forum.unity.com/members/cician.164885/

Shader "GO2023/SimpleGrabPassBlur" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Tint Color (RGB)", 2D) = "white" {}
        _BlurStrength ("Blur Strength", Range(0, 10)) = 1
    }
   
    Category {
   
        // We must be transparent, so other objects are drawn before this one.
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
   
   
        SubShader {
       
            // Horizontal blur
            GrabPass {                     
                Tags { "LightMode" = "Always" }
            }
            Pass {
                Tags { "LightMode" = "Always" }
               
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
               
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
               
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                };
               
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    return o;
                }
               
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _BlurStrength
    ;
               
                half4 frag( v2f i ) : COLOR {
                   
                    half4 colour = half4(0,0,0,0);
 
                    #define GRABPIXEL(weight, offsetX) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * offsetX * _BlurStrength, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
 
                    colour += GRABPIXEL(0.05, -4.0);
                    colour += GRABPIXEL(0.09, -3.0);
                    colour += GRABPIXEL(0.12, -2.0);
                    colour += GRABPIXEL(0.15, -1.0);
                    colour += GRABPIXEL(0.18,  0.0);
                    colour += GRABPIXEL(0.15, +1.0);
                    colour += GRABPIXEL(0.12, +2.0);
                    colour += GRABPIXEL(0.09, +3.0);
                    colour += GRABPIXEL(0.05, +4.0);
                   
                    return colour;
                }
                ENDCG
            }
 
            // Vertical blur
            GrabPass {                         
                Tags { "LightMode" = "Always" }
            }
            Pass {
                Tags { "LightMode" = "Always" }
               
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
               
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
               
                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                };
               
                v2f vert (appdata_t v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    return o;
                }
               
                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _BlurStrength
    ;
                float4 _Color;
               
                half4 frag( v2f i ) : COLOR {
                   
                    half4 colour = half4(0, 0, 0, 0);
 
                    #define GRABPIXEL(weight, offsetY) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * offsetY * _BlurStrength, i.uvgrab.z, i.uvgrab.w))) * weight
                   
                    colour += GRABPIXEL(0.05, -4.0);
                    colour += GRABPIXEL(0.09, -3.0);
                    colour += GRABPIXEL(0.12, -2.0);
                    colour += GRABPIXEL(0.15, -1.0);
                    colour += GRABPIXEL(0.18,  0.0);
                    colour += GRABPIXEL(0.15, +1.0);
                    colour += GRABPIXEL(0.12, +2.0);
                    colour += GRABPIXEL(0.09, +3.0);
                    colour += GRABPIXEL(0.05, +4.0);
                   
                    return colour * _Color;
                }
                ENDCG
            }
        }
    }
}