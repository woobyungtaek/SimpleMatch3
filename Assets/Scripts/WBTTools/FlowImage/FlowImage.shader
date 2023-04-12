Shader "Unlit/FlowImage"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}

		_Color("Tint", Color) = (1,1,1,1)
		_SpeedX("SpeedX", float) = 0
		_SpeedY("SpeedY", float) = 0

		_FlowColor("FlowColor", Color) = (1,1,1,1)
		_TileX("TileX", float) = 1
		_TileY("TileY", float) = 1

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
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
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile_local _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"

            UNITY_INSTANCING_BUFFER_START(Props)

                UNITY_DEFINE_INSTANCED_PROP(fixed4, _FlowColor)
                UNITY_DEFINE_INSTANCED_PROP(float,  _TileX)
                UNITY_DEFINE_INSTANCED_PROP(float,  _TileY)

            UNITY_INSTANCING_BUFFER_END(Props)

				v2f Vert(appdata_t IN)
				{
				    v2f OUT;
				
				    UNITY_SETUP_INSTANCE_ID (IN);
				    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				
				    OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
				    OUT.vertex = UnityObjectToClipPos(OUT.vertex);
				    OUT.texcoord = IN.texcoord;
				    OUT.color = IN.color;
				
				    #ifdef PIXELSNAP_ON
				    OUT.vertex = UnityPixelSnap (OUT.vertex);
				    #endif
				
				    return OUT;
				}

				float _SpeedX;
				float _SpeedY;
				float _UnscaledTime;

				fixed4 Frag(v2f IN) : SV_Target
				{
					// Gpu 인스턴싱 된 값 가져오기
                    fixed4 flowColor  = UNITY_ACCESS_INSTANCED_PROP(Props, _FlowColor);
					float tx = UNITY_ACCESS_INSTANCED_PROP(Props, _TileX);
					float ty = UNITY_ACCESS_INSTANCED_PROP(Props, _TileY);

					// 타일링 된 만큼 uv값 조절 
					float2 uv = IN.texcoord;
					uv.x *= tx;
					uv.y *= ty;

					// 시간 * 속도만큼 uv값 조절 
					uv.x += _UnscaledTime * _SpeedX;
					uv.y += _UnscaledTime * _SpeedY;

					// 조절된 uv값에 해당하는 원본 색상 출력
					fixed4 c = tex2D(_MainTex, uv);
					c *= flowColor;
					c.rgb *= c.a;
					
					return c;
				}
			ENDCG
			}
		}
}
