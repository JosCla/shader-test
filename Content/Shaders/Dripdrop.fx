#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler s0;

float time;

float texOffsetMult;

float sharpness;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct PixelInput
{
    float4 Position: SV_Position0;
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
    float trueDistance = sqrt(input.Position.x * input.Position.x + input.Position.y * input.Position.y);
    float targetDistance = time * 20.0;
    float relativeDistance = trueDistance - targetDistance;
    float offsetFactor = OffsetFactor(relativeDistance);
    float2 offsetDir = float2(input.Position.x, input.Position.y);
    float2 offsetVec = normalize(offsetDir) * offsetFactor * texOffsetMult;
	float4 color = tex2D(SpriteTextureSampler, input.TexCoord + offsetVec) * input.Color;
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