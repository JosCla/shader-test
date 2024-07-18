#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// paraboloid
// const static float texCutoff = 0.16;
// const static float borderCutoff = 0.2;

// cone
const static float texCutoff = 0.4;
const static float borderCutoff = 0.45;

const static float4 borderColor = float4(0.9, 0.9, 0.9, 1.0);

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
    float xDist = abs(input.TexCoord.x - 0.5);
    float yDist = abs(input.TexCoord.y - 0.5);
    float dist = sqrt(xDist * xDist + yDist * yDist);

    if (dist < texCutoff) {
        return tex2D(SpriteTextureSampler, input.TexCoord) * input.Color;
    } else if (dist < borderCutoff) {
        return borderColor;
    } else {
        return float4(0.0, 0.0, 0.0, 0.0);
    }
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
