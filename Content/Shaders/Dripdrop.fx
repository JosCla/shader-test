#if OPENGL
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define MAX_DROPS 5

Texture2D SpriteTexture;
sampler s0;

float time;

float texOffsetMult;

float sharpness;

int numDrops;
float2 dropCenters[MAX_DROPS];

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct PixelInput
{
    float4 Position: SV_POSITION;
    float4 Color: COLOR0;
    float2 TexCoord: TEXCOORD0;
};

// effectively, the derivative of a bell curve centered at distance = 0 with a given sharpness
float OffsetFactor(float distance)
{
    return -2.0 * distance * sharpness * exp(-sharpness * (distance * distance));
}

float4 MainPS(PixelInput input) : COLOR
{
    float2 cumulativeOffset = float2(0.0, 0.0);
    for (int i = 0; i < MAX_DROPS; i++) {
        if (i < numDrops) {
            float xDist = input.Position.x - dropCenters[i].x;
            float yDist = input.Position.y - dropCenters[i].y;
            float trueDistance = sqrt(xDist * xDist + yDist * yDist);
            float targetDistance = time * 20.0;
            float relativeDistance = trueDistance - targetDistance;
            float offsetFactor = OffsetFactor(relativeDistance);
            float2 offsetDir = float2(input.Position.x, input.Position.y);
            float2 offsetVec = normalize(offsetDir) * offsetFactor * texOffsetMult;
            cumulativeOffset = cumulativeOffset + offsetVec;
        }
    }
	float4 color = tex2D(SpriteTextureSampler, input.TexCoord + cumulativeOffset) * input.Color;
    color.a = 0.1f;
	return color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}