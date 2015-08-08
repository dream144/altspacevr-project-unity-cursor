Shader "Custom/CursorShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	SubShader {
	  Lighting Off
	  ZTest Always
	  
	  Tags { "Queue" = "Overlay" }
      
	  Pass {
		SetTexture [_MainTex] {
			constantColor [_Color]
			Combine texture * constant, texture * constant
		}
	  }
    }
}