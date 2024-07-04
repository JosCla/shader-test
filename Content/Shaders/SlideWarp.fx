#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

uniform float blendFactor;
uniform float intensity;
const static float2 redOffset = float2(0.00, 0.01);
const static float2 greenOffset = float2(-0.006, -0.006);
const static float2 blueOffset = float2(0.006, -0.006);

Texture2D FirstTexture;
sampler2D FirstTextureSampler = sampler_state
{
	Texture = <FirstTexture>;
};

Texture2D SecondTexture;
sampler2D SecondTextureSampler = sampler_state
{
	Texture = <SecondTexture>;
};

struct MainPixelInput
{
    float4 Position: SV_Position0;
    float4 Color: COLOR0;
    float2 TexCoord: TEXCOORD0;
};

float4 GetOffsetted(sampler2D texSampler, float2 initTexCoord, float blend)
{
    float4 colorInit = tex2D(texSampler, initTexCoord);
    float2 factors = colorInit.r * redOffset + colorInit.g * greenOffset + colorInit.b * blueOffset;
    float2 offset = factors * intensity * blend;
    return tex2D(texSampler, initTexCoord + offset);
}

float4 MainPS(MainPixelInput input) : COLOR0
{
    float firstBlend = blendFactor;
    float4 firstColor = GetOffsetted(FirstTextureSampler, input.TexCoord, firstBlend);

    float secondBlend = 1.0 - blendFactor;
    float4 secondColor = GetOffsetted(SecondTextureSampler, input.TexCoord, secondBlend);

    return (firstColor * firstBlend) + (secondColor * secondBlend);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}