#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define NUM_SAMPLES 3
static const int samplesAlongAxis = (2.0 * NUM_SAMPLES) + 1.0;
static const float sampleDist = 0.005;
static const float sampleDivFactor = 1.0 / (samplesAlongAxis * samplesAlongAxis);

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

float4 MainPS(MainPixelInput input) : COLOR0
{
    float4 colorRes = float4(0.0, 0.0, 0.0, 0.0);
    for (int row = -NUM_SAMPLES; row <= NUM_SAMPLES; row++) {
        for (int col = -NUM_SAMPLES; col <= NUM_SAMPLES; col++) {
            float2 texOffset = float2(row, col) * sampleDist;
            float4 color = tex2D(SpriteTextureSampler, input.TexCoord + texOffset);
            colorRes += color * sampleDivFactor;
        }
    }
    return colorRes;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}