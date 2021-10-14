// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CustomWBT/PixelDraw"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Value("Value", Range(0,1)) = 0
	}
		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					fixed4 color : COLOR;
					half2 texcoord  : TEXCOORD0;
				};

				fixed4 _Color;
				float _Value;

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.vertex = UnityObjectToClipPos(IN.vertex);
					OUT.texcoord = IN.texcoord;
					OUT.color = IN.color * _Color;
					return OUT;
				}
				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
					if (c.a == 0) { return fixed4(0, 0, 0, 0); }

					float gV = (c.r + c.g + c.b) / 3;

					if (gV > _Value)
					{
						return fixed4(0, 0, 0, 0);
					}
					return c;
				}
					ENDCG
			}
		}
}