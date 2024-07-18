#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// cubic paraboloid
const static float texCutoff = 0.064;
const static float borderCutoff = 0.091;

// paraboloid
// const static float texCutoff = 0.16;
// const static float borderCutoff = 0.2;

// cone
// const static float texCutoff = 0.4;
// const static float borderCutoff = 0.45;

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
    // getting underlying "circle" height
    float xDist = abs(input.TexCoord.x - 0.5);
    float yDist = abs(input.TexCoord.y - 0.5);
    // float height = sqrt(xDist * xDist + yDist * yDist); // cone
    // float height = xDist * xDist + yDist * yDist; // paraboloid
    float height = pow(xDist, 3) + pow(yDist, 3); // cubic paraboloid

    // TODO: add perlin noise height

    // doing cutoffs based on result height
    if (height < texCutoff) {
        return tex2D(SpriteTextureSampler, input.TexCoord) * input.Color;
    } else if (height < borderCutoff) {
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
