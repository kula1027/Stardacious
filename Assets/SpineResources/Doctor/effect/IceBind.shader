Shader "Custom/IceBind" {
Properties {
         _Color ("Main Color", Color) = (1,1,1,1)
         _MainTex ("Base (RGB) Gloss (A)", 2D) = "black" {}
         _BumpMap ("Normalmap", 2D) = "bump" {}
         _ColorStrength ("Color Strength", Float) = 1
         _BumpAmt ("Distortion [0-100]", range (0, 100)) = 10
         _Distance ("Distance", Float) = 1
 }
 
 SubShader {
         Tags { "Queue"="Transparent+1" "RenderType"="Transperent" }
         GrabPass {}    
         LOD 200
         ZTest Less
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha
         Cull Back
 CGPROGRAM
 
 #pragma surface surf Lambert alpha vertex:vert
 
	sampler2D _MainTex;
	sampler2D _BumpMap;
	float _BumpAmt;
	float _ColorStrength;
	sampler2D _GrabTexture;
	float4 _GrabTexture_TexelSize;
	float _Distance;

	float4 _Color;
 
	struct Input {
    	float2 uv_MainTex;
    	float2 uv_BumpMap;
    	float4 proj : TEXCOORD0;
    	fixed4 color;
	};
 
	void vert (inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input,o);
		float4 oPos = mul(UNITY_MATRIX_MVP, v.vertex);
		#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
		#else
			float scale = 1.0;
		#endif
		o.proj.xy = (float2(oPos.x, oPos.y*scale) + oPos.w) * 0.5;
		o.proj.zw = oPos.zw;
		o.color = v.color;
 	}
  
 void surf (Input IN, inout SurfaceOutput o) {
         o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        
         half2 offset = o.Normal.rg * _BumpAmt * _GrabTexture_TexelSize.xy;
         //IN.proj.xy = offset * IN.proj.z + IN.proj.xy;
         IN.proj.xy = offset * _Distance + IN.proj.xy;
         half4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.proj));

         fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
         o.Emission = col.xyz + tex*_ColorStrength;
		 o.Alpha = IN.color.a * tex.a;

 		 //if (tex.r == 0){
 		 //	o.Alpha = 0;
 		 //}else{
 		 //	o.Alpha = IN.color.a;
 		 //}
 }


 ENDCG
 }
 
 FallBack "Diffuse"
}
