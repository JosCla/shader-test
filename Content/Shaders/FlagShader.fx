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
uniform float rightSideOffset;

uniform float leftSideU;
uniform float rightSideU;

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

    float withinUVal = (input.TexCoord.x - leftSideU) / (rightSideU - leftSideU);
    float offsetY = withinUVal * rightSideOffset;
    float4 offsetPosition = input.Position + float4(0.0, offsetY, 0.0, 0.0);
    float4 tempPos = mul(mvpMatrix, offsetPosition);
    float4 ndcPos = tempPos + float4(-1.0, 1.0, 0.0, 0.0);
    output.Position = float4(ndcPos.xyz, 1.0);

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
