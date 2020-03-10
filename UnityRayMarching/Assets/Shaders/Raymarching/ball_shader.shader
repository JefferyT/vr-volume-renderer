Shader "Custom/BALLSHADER"
{
Properties
{
_Radius("Radius", Float) = 1
_Centre("Centre", Vector) = (0.0,0.0,0.0)
}
SubShader
{

	Pass
	{
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag

	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float _Radius;
	float _Centre;

	#define STEPS 64
	#define STEP_SIZE 0.5

	struct appdata {
	float4 vertex : POSITION;
	};

	struct v2f {
	float4 vertex : SV_POSITION;
	float3 wPos : TEXCOORD1; // World Position
	};

	v2f vert(appdata v) {
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		return o;
	}

	float sphereHit(float3 p) {
		return distance(p, _Centre) < _Radius ? 0.00000000001 : 0;
	}

	bool raymarchHit(float3 position, float3 direction) {
		float sum = 0.0;
		for (int i = 0; i < STEPS; i++) {
			sum += sphereHit(position);
			position += direction * STEP_SIZE;
		}
		return sum;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float3 worldPosition = i.wPos;
		float3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos);
		float res = raymarchHit(worldPosition, viewDirection);
		if (res != 0) {
			return fixed4(0,res,res,1);
		}
		 else {
		 return fixed4(1,1,1,1);
	}
  }
  ENDCG
  }
  }
}