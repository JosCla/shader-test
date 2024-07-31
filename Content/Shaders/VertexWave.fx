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

uniform float4x4 mvpMatrix;
uniform float time;
uniform float intensity;

const static float timeFactor = 0.4;
const static float sinFactor = 4.0;

struct VertexInput
{
    float4 Position: SV_POSITION0;
    float4 Color: COLOR0;
    float2 TexCoord: TEXCOORD0;
};

struct PixelInput
{
    float4 Position: SV_POSITION0;
    float4 Color: COLOR0;
    float2 TexCoord: TEXCOORD0;
};

PixelInput MainVS(VertexInput input)
{
    PixelInput output = (PixelInput)0;
    output.Color = input.Color;
    output.TexCoord = input.TexCoord;

    float4 tempPos = mul(mvpMatrix, input.Position);
    float offsetY = sin((input.TexCoord.x * sinFactor) + (time * timeFactor)) * intensity;
    float4 offset = float4(0.0, offsetY, 0.0, 0.0);
    float4 tempPosOffset = tempPos + offset;
    output.Position = float4(tempPosOffset.xyz, 1.0);

    return output;
}

float4 MainPS(PixelInput input) : COLOR0
{
    return tex2D(SpriteTextureSampler, input.TexCoord) * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
