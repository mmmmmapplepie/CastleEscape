// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/RoomClear"
{
		Properties {
			_MainTex("Texture", 2D) = "white"{}
			_MainTex2("Texture2", 2D) = "white"{}
			_Color("Color", Color) = (1,1,1,1)
			_Merge("Merge", Range(0,1)) = 0
		}
    SubShader
    {
			Tags {
				"Queue" = "Transparent"
			}
        Pass
        {
					// Blend SrcAlpha OneMinusSrcAlpha
					Blend One One

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;  
            sampler2D _MainTex2;  
float4 _Color;
float1 _Merge;

            v2f vert (appdata v)
            {
                v2f Out;
                Out.vertex = UnityObjectToClipPos(v.vertex);
                Out.uv = v.uv;
                return Out;
            }

            fixed4 frag (v2f Input) : SV_Target
            {
                // float4 texCol = tex2D(_MainTex, 2*Input.uv)*_Merge+tex2D(_MainTex2, 2*Input.uv)*(1-_Merge);
								// float1 lum = (texCol.r + texCol.g + texCol.b)/3;
								// float4 col = float4(lum, lum, lum, texCol.a);
								// col = col * _Color;
								float4 col = tex2D(_MainTex, Input.uv);
                return col;
            }
            ENDCG
        }
    }
}
