Shader "Custom/SpriteTextureOverride"
{
    Properties
    {
        // SpriteRenderer will automatically assign a texture here at runtime
        // We declare it but won't use it
        _MainTex ("Default Sprite", 2D) = "white" {}
        
        // This is our actual texture that we want to display
        // Could be a RenderTexture or any other texture
        _OverrideTexture ("Override Texture", 2D) = "white" {}
        
        // Required for sprite renderer sorting
        [HideInInspector] _Cutoff ("Cutoff", Float) = 0.5
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
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
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            }; 
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _OverrideTexture;
            float4 _OverrideTexture_ST;
            fixed4 _Color;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_OverrideTexture, i.uv);
                color.rgb *= color.a;
                return color * i.color;
            }
            ENDCG
        }
    }
}