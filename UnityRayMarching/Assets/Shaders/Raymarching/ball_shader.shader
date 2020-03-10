Shader "Custom/BALLSHADER"
{
	Properties
	{
	_Radius("Radius", Float) = 2
	_Radius1("Radius1", Float) = 3
	_Radius2("Radius2", Float) = 2.5
	_Radius3("Radius3", Float) = 12.5
	_Centre("Centre", Vector) = (5.0, 5.0, 5.0, 1.0)
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
		float _Radius1;
		float _Radius2;
		float _Radius3;
		Vector _Centre;

	//	float3 center_1 = float3(0.02, 0.02, 0.05);
	//	float3 center_2 = float3(-0.1, 0.05, 0.05);
	//	float3 center_3 = float3(0, 0.03, -0.05);
	//	float3 center_4 = float3(0.04, 0, 0.01);
	//float radius1 = 0.01;
	//float radius2 = 0.03;
	//float radius3 = 0.04;
	//float radius4 = 0.1;


	#define STEPS 64
	#define STEP_SIZE 0.1

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

	bool singleHit(float3 p, float3 center, float radius) {
		return distance(p, center) < radius;
	}

	float3 sphereHit(float3 p) {
		float3 total_hit = float3(0.0, 0.0, 0.0);
		float3 center = float3(_Centre.x, _Centre.y, _Centre.z);
		if (singleHit(p,  center, _Radius)) {
			total_hit = float3(0.8, 0, 0);
		}
		else if (singleHit(p,  center, _Radius1)) {
			total_hit = float3(0.7, 0.6, 1);
		}
		else if (singleHit(p,  center, _Radius2)) {
			total_hit = float3(0.7, 1, 0.7);
		}
		else if (singleHit(p,  center, _Radius3)) {
			total_hit = float3(0.3, 0.4, 1);
		}
		return total_hit;
	}



	float3 raymarchHit(float3 position, float3 direction) {
		float sum = 0.0;
		for (int i = 0; i < STEPS; i++) {
			//sum += 
			float3 temp = sphereHit(position);
			if (temp.x != 0.0) {
				return temp;
			}
			position += direction * STEP_SIZE;
		}
		return position;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float3 worldPosition = i.wPos;
		float3 viewDirection = normalize(i.wPos - _WorldSpaceCameraPos);
		//float res = raymarchHit(worldPosition, viewDirection);
		//if (res != 0) {
		//	return fixed4(0,res,res,1);
		//	//return fixed4(_Center, 1.0f);
		//}
		return fixed4(raymarchHit(worldPosition, viewDirection), 1.0);
		//return fixed4(0.3, 0.4, 1, 1);
  }
  ENDCG
  }
  }
}