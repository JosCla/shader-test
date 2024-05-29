#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float3 redOffset;
float3 greenOffset;
float3 blueOffset;

Texture2D SpriteTexture;
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
    float4 startColor = tex2D(SpriteTextureSampler, input.TexCoord);
    float3 offset = startColor.r * redOffset + startColor.g * greenOffset + startColor.b * blueOffset;
    float4 trueColor = tex2D(SpriteTextureSampler, input.TexCoord + offset) * input.Color;
	return trueColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}