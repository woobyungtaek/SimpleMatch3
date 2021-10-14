Shader "Hidden/ImageEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off
		ZWrite Off
		ZTest Always
		Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				o.color = v.color;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

				
				if (col.a == 0) { return fixed4(0, 0, 0, 0); }
				if (col.r < 0.5 && col.g < 0.5 && col.b <0.5) { return fixed4(0, 0, 0, 0); }
                return col;
            }
            ENDCG
        }
    }
}
