Shader "Custom/TextureBlendByVertexColor"
{
    Properties
    {
        _Texture1 ("Texture 1", 2D) = "white" {}
        _Texture2 ("Texture 2", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            Cull Off
            ZWrite On
            ZTest LEqual

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
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _Texture1;
            sampler2D _Texture2;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex1 = tex2D(_Texture1, i.uv);
                fixed4 tex2 = tex2D(_Texture2, i.uv);
                
                float blendFactor = i.color.r;
                return lerp(tex1, tex2, blendFactor);
            }
            ENDCG
        }
    }
    FallBack "Unlit"
}