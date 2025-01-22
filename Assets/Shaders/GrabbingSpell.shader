Shader "Unlit/AttractionSpellShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _SpellColor ("Spell Color", Color) = (41, 255, 41, 255)
        _BarsTex  ("Bars Texture", 2D) = "mask" {}
        _ScrollSpeed ("Scroll Speed", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _BarsTex;
            float4 _SpellColor;

            float _ScrollSpeed;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 offset = float2(0, _Time.y * _ScrollSpeed);
                
                float2 offset_UV = i.uv  + offset;
                fixed4 mask_value = tex2D(_BarsTex, offset_UV);

                float alpha = mask_value.r;
                fixed4 col = fixed4(_SpellColor.rgb, alpha);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
