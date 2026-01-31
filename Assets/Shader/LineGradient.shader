Shader "Custom/LineWidthGradient"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        [Toggle] _Invert ("Invert Gradient (Inside/Outside)", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR; // 获取 LineRenderer 的颜色
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            fixed4 _Color;
            float _Invert;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // i.uv.y 是线条的宽度方向 (0 是内侧或外侧)
                float alpha = i.uv.y;
                if (_Invert > 0.5) alpha = 1.0 - alpha;
                
                fixed4 col = i.color;
                col.a *= alpha; // 将计算出的渐变应用到透明度
                return col;
            }
            ENDCG
        }
    }
}