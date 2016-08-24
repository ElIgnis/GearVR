Shader "Custom/Billboard" {
	Properties {
      	_MainTex ("Base (RGB)", 2D) = "white" {}
      	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
      	_Scale("Scale", float) = 1.0      	
	}
	
	Category	
	{	
	    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }	
	    Blend SrcAlpha OneMinusSrcAlpha	
	    AlphaTest Greater .01	
	    Cull Off
	    ZWrite off
           
		SubShader {
			
		    Pass	
		    {
		
		        CGPROGRAM
		        #pragma vertex vert
		        #pragma fragment frag
		        #include "UnityCG.cginc"
		
		        sampler2D _MainTex;
		        fixed4 _TintColor;
		        float _Scale;
		
		        struct appdata_t
		        {
		            float4 vertex : POSITION;
		            fixed4 color : COLOR;
		            float2 texcoord : TEXCOORD0;
		        };
		
		
		
		        struct v2f
		        {
		            float4 vertex : POSITION;
		            fixed4 color : COLOR;
		            float2 texcoord : TEXCOORD0;
		        };
		
		
		
		        v2f vert (appdata_t v)
		        {
		            v2f o;
		            
		            o.vertex = mul(UNITY_MATRIX_P,
		                 mul(UNITY_MATRIX_MV, float4(0.0, 0.0, 0.0, 1.0))
		                + float4(v.vertex.x, v.vertex.y, 0.0, 0.0)*_Scale);
		
		
		
		            o.color = v.color;
		            o.texcoord = v.texcoord;
		            return o;
		
		        }	
		
		        fixed4 frag (v2f i) : COLOR
		        {
		            return 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
		
		        }
		
		        ENDCG
		
		    }
		}
	}

}
