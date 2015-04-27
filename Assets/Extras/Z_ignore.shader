Shader "Custom/Z_ignore" {
	 Properties 
 {
     _Color ("Main Color", Color) = (1,0,0,1)
     _MainTex ("Base (RGB) Gloss (A)", 2D) = "red" {}
 }
 
 Category 
 {
     SubShader 
     { 
         Tags { "Queue"="Overlay+1" }
 
         Pass
         {
             ZWrite Off
             ZTest Greater
             Lighting Off
             Color [_Color]
         }
 
         Pass 
         {
             ZTest Less          
             SetTexture [_MainTex] {combine texture}
         }
     }
 }
 
 FallBack "Specular", 1
}
