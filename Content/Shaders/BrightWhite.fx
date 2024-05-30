#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float brightThreshold;

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

struct MainPixelOutput
{
    float4 Color: COLOR0;
    float4 Bright: COLOR1;
};

float Brightness(float4 color)
{
    // credit to joey devries (https://learnopengl.com/Advanced-Lighting/Bloom) for brightness calculation
    return dot(color.rgb, float3(0.2126, 0.7152, 0.0722));
}

MainPixelOutput MainPS(MainPixelInput input)
{
    MainPixelOutput output;
    float4 color = tex2D(SpriteTextureSampler, input.TexCoord);
    float4 brightColor = color;
    if (Brightness(color) < brightThreshold)
        brightColor = float4(0.0, 0.0, 0.0, 1.0);

    output.Color = color;
    output.Bright = brightColor;
    return output;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}