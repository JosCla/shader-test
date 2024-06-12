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

#define LOOKUP_SIZE 140
uniform int lookup[LOOKUP_SIZE];

uniform int texWidth;
uniform int texHeight;

const static float time = 0.0;

const static float movementMult = 2.5;

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

// Returns the value for a given hash
float GetHashValue(int hash, float x, float y) {
    float xpart = (((float)lookup[hash] / 64.0) - 1.0) * x;
    float ypart = (((float)lookup[hash + 1] / 64.0) - 1.0) * y;
    return xpart + ypart;
}

float4 MainPS(MainPixelInput input) : COLOR0
{
    float x = input.TexCoord.x * texWidth;
    float y = input.TexCoord.y * texHeight;

    // Coordinates of box bounding x and y
    int X = (int)floor(x) % 128;
    int Y = (int)floor(y) % 128;
    // Fractional part of x and y
    x -= floor(x);
    y -= floor(y);
    // Fade of x and y's fraction (for smooth interpolation)
    float fx = SmoothFade(x);
    float fy = SmoothFade(y);

    // Finding hashes for the four corners around x and y
    int hx0y0 = lookup[lookup[X  ]+ Y  ];
    int hx1y0 = lookup[lookup[X+1]+ Y  ];
    int hx0y1 = lookup[lookup[X  ]+ Y+1];
    int hx1y1 = lookup[lookup[X+1]+ Y+1];

    // Then interpolating the random gradients at the corners to find our noise
    float x0 = lerp(fx, GetHashValue(hx0y0, x  , y  ), GetHashValue(hx1y0, x-1, y  ));
    float x1 = lerp(fx, GetHashValue(hx0y1, x  , y-1), GetHashValue(hx1y1, x-1, y-1));
    float offsetX = lerp(fy, x0, x1) * movementMult / texWidth;

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