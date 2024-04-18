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

float period;
float time;

float texOffsetMult;

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

float4 MainPS(PixelInput input) : COLOR
{
    // float radius = sqrt(input.Position.x * input.Position.x + input.Position.y * input.Position.y);
    float radius = (input.Position.y - 0.7f * input.Position.x) * 0.5f;
    float sinFactor = (radius + (time * 20.0)) * 2.0 * 3.14159 / period;
    float offsetFactor = (sin(sinFactor) + 1.0) / 2.0;
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