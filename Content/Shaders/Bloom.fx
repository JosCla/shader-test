#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct MainPixelInput
{
    float4 Position: SV_Position0;
    float4 Color: COLOR0;
    float2 TexCoord: TEXCOORD0;
};

uniform float brightThreshold;

// credit to joey devries for brightness and gaussian coefficients!
// (and also for a great explanation of bloom) https://learnopengl.com/Advanced-Lighting/Bloom
float Brightness(float4 color)
{
    return dot(color.rgb, float3(0.2126, 0.7152, 0.0722));
}

float4 ExtractBrightPS(MainPixelInput input) : COLOR0
{
    float4 color = tex2D(SpriteTextureSampler, input.TexCoord);
    if (Brightness(color) < brightThreshold)
        return float4(0.0, 0.0, 0.0, 0.0);

    return color;
}

#define NUM_SAMPLES 5
const static float gaussCoeffs[NUM_SAMPLES] = {0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216};

uniform int texWidth;
uniform int texHeight;

uniform float movementMult;

float4 HorizBlurPS(MainPixelInput input) : COLOR0
{
    float movement = 1.0 / texWidth;
    float4 res = tex2D(SpriteTextureSampler, input.TexCoord) * gaussCoeffs[0];
    for (int i = 1; i < NUM_SAMPLES; i++)
    {
        res += tex2D(
            SpriteTextureSampler,
            input.TexCoord + float2(i * movement * movementMult, 0)
        ) * gaussCoeffs[i];
        res += tex2D(
            SpriteTextureSampler,
            input.TexCoord - float2(i * movement * movementMult, 0)
        ) * gaussCoeffs[i];
    }
    return res;
}

float4 VertBlurPS(MainPixelInput input) : COLOR0
{
    float movement = 1.0 / texHeight;
    float4 res = tex2D(SpriteTextureSampler, input.TexCoord) * gaussCoeffs[0];
    for (int i = 1; i < NUM_SAMPLES; i++)
    {
        res += tex2D(
            SpriteTextureSampler,
            input.TexCoord + float2(0, i * movement * movementMult)
        ) * gaussCoeffs[i];
        res += tex2D(
            SpriteTextureSampler,
            input.TexCoord - float2(0, i * movement * movementMult)
        ) * gaussCoeffs[i];
    }
    return res;
}

technique ExtractBright
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL ExtractBrightPS();
	}
}

technique HorizBlur
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL HorizBlurPS();
    }
}

technique VertBlur
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL VertBlurPS();
    }
}