#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Largely based on Ken Perlin's Improved Noise implementation
// Based on: https://adrianb.io/2014/08/09/perlinnoise.html

uniform int texWidth;
uniform int texHeight;

uniform float time;

uniform float movementMult;
uniform float gridWidth;

const static float timeMult = 0.5;

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct MainPixelInput
{
    float4 Position: SVlookuposition0;
    float4 Color: COLOR0;
    float2 TexCoord: TEXCOORD0;
};

// Smoothly fades n between 0 and 1
// Returns 6n^5 - 15n^4 + 10n^3
float SmoothFade(float n) {
    return n * n * n * (n * (n * 6.0 - 15.0) + 10.0);
}

// Rand function from
// https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83
float GetRand(float x, float y, float t)
{
    float resFull = (sin(dot(float3(x, y, t), float3(17.823, 91.123, 44.144))) * 11238.2);
    return resFull - floor(resFull);
}

float4 MainPS(MainPixelInput input) : COLOR0
{
    float x = input.TexCoord.x * texWidth / gridWidth;
    float y = input.TexCoord.y * texHeight / gridWidth;
    float t = time * timeMult;

    // Coordinates of bounding box
    int X = floor(x);
    int Y = floor(y);
    int T = floor(t);
    // Fractional parts of bounding box
    x -= (float)X;
    y -= (float)Y;
    t -= (float)T;
    // Smoothly fading fractional parts
    float fx = SmoothFade(x);
    float fy = SmoothFade(y);
    float ft = SmoothFade(t);

    // Finding rand values for the eight corners
    float rx0y0t0 = GetRand(X  , Y  , T  );
    float rx1y0t0 = GetRand(X+1, Y  , T  );
    float rx0y1t0 = GetRand(X  , Y+1, T  );
    float rx1y1t0 = GetRand(X+1, Y+1, T  );
    float rx0y0t1 = GetRand(X  , Y  , T+1);
    float rx1y0t1 = GetRand(X+1, Y  , T+1);
    float rx0y1t1 = GetRand(X  , Y+1, T+1);
    float rx1y1t1 = GetRand(X+1, Y+1, T+1);

    // Then interpolating the random gradients at the corners to find our noise
    float y0t0 = lerp(rx0y0t0, rx1y0t0, fx);
    float y1t0 = lerp(rx0y1t0, rx1y1t0, fx);
    float y0t1 = lerp(rx0y0t1, rx1y0t1, fx);
    float y1t1 = lerp(rx0y1t1, rx1y1t1, fx);
    float t0 = lerp(y0t0, y1t0, fy);
    float t1 = lerp(y0t1, y1t1, fy);
    float offsetX = (lerp(t0, t1, ft) * 2.0 - 1.0) * movementMult / texWidth;

    float2 actualTexCoord = input.TexCoord + float2(offsetX, 0);
    return tex2D(SpriteTextureSampler, actualTexCoord) * input.Color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}