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

// smoothly fades n between 0 and 1
// returns 6n^5 - 15n^4 + 10n^3
float SmoothFade(float n) {
    return n * n * n * (n * (n * 6.0 - 15.0) + 10.0);
}

// based on rand function from
// https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83
float GetRand(float3 input)
{
    return frac(sin(dot(input, float3(17.823, 91.123, 44.144))) * 11238.2);
}

float3 GetRand3D(float3 input)
{
    return float3(
        GetRand(input),
        GetRand(float3(input.z, input.x, input.y)),
        GetRand(float3(input.y, input.z, input.x))
    );
}

// gets the noise value (between -1 and 1) at a certain point
float GetNoiseAt(float2 pos, float time)
{
    float3 floatPos = float3(pos.x, pos.y, time);
    float3 fullPos = float3(floor(pos.x), floor(pos.y), floor(time));
    float fadeX = SmoothFade(frac(floatPos.x));
    float fadeY = SmoothFade(frac(floatPos.y));
    float fadeZ = SmoothFade(frac(floatPos.z));

    float zRes[2];
    float yRes[2];
    float xRes[2];
    for (int z = 0; z <= 1; z++) {
        for (int y = 0; y <= 1; y++) {
            for (int x = 0; x <= 1; x++) {
                float3 currPos = fullPos + float3(x, y, z);

                float3 currGradient = GetRand3D(currPos);
                float3 currCenterVec = floatPos - currPos;

                xRes[x] = dot(currGradient, currCenterVec);
            }
            yRes[y] = lerp(xRes[0], xRes[1], fadeX);
        }
        zRes[z] = lerp(yRes[0], yRes[1], fadeY);
    }

    return lerp(zRes[0], zRes[1], fadeZ);
}

// cubic paraboloid
// const static float texCutoff = 0.064;
// const static float borderCutoff = 0.091;

// paraboloid
// const static float texCutoff = 0.16;
// const static float borderCutoff = 0.2;

// cone
const static float texCutoff = 0.4;
const static float borderCutoff = 0.45;

const static float4 borderColor = float4(0.9, 0.9, 0.9, 1.0);

uniform float perlinSharpness;
uniform float perlinMagnitude;

uniform float time;
const static float timeFactor = 0.4;

float4 MainPS(MainPixelInput input) : COLOR0
{
    // getting underlying "circle" height
    float xDist = abs(input.TexCoord.x - 0.5);
    float yDist = abs(input.TexCoord.y - 0.5);
    // float height = sqrt(xDist * xDist + yDist * yDist); // cone
    // float height = xDist * xDist + yDist * yDist; // paraboloid
    // float height = pow(xDist, 3) + pow(yDist, 3); // cubic paraboloid
    float height = pow(xDist * xDist * xDist + yDist * yDist * yDist, 1.0 / 3.0); // cubic cone

    // adding perlin noise height
    float noise = GetNoiseAt(input.TexCoord * perlinSharpness, time * timeFactor) * perlinMagnitude;
    height += noise;

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
