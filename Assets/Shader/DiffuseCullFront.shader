Shader "Custom/DiffuseCullFront" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull front
		
		Pass {
			Lighting Off
			SetTexture [_MainTex] { combine texture } 
		}
	} 
	FallBack "Diffuse"
}
