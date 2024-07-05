#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define MAX_DROPS 20

const static float2 redOffset = float2(0.00, 0.01);
const static float2 greenOffset = float2(-0.006, -0.006);
const static float2 blueOffset = float2(0.006, -0.006);

float time;
int numDrops;
float4 drops[MAX_DROPS];

const static float timeScaleFactor = 120.0;
const static float sigmoidSharpness = 0.1;
const static float sigmoidOffset = 0.0;

Texture2D FirstTexture;
sampler2D FirstTextureSampler = sampler_state
{
	Texture = <FirstTexture>;
};

Texture2D SecondTexture;
sampler2D SecondTextureSampler = sampler_state
{
	Texture = <SecondTexture>;
};

struct MainPixelInput
{
    float4 Position: SV_Position0;
    float4 Color: COLOR0;
    float2 TexCoord: TEXCOORD0;
};

float GetIntensity(float blendFactor)
{
    return exp(27.0 * blendFactor * (1.0 - blendFactor)) - 1.0;
}

float4 GetOffsetted(sampler2D texSampler, float2 initTexCoord, float blend)
{
    float intensity = GetIntensity(blend);
    float4 colorInit = tex2D(texSampler, initTexCoord);
    float2 factors = colorInit.r * redOffset + colorInit.g * greenOffset + colorInit.b * blueOffset;
    float2 offset = factors * intensity * blend;
    return tex2D(texSampler, initTexCoord + offset);
}

float Sigmoid(float n)
{
    return 1.0 / (1.0 + exp(-n));
}

float GetDropFactor(float4 drop, float2 pos)
{
    float xDist = pos.x - drop.x;
    float yDist = pos.y - drop.y;
    float trueDistance = sqrt(xDist * xDist + yDist * yDist);
    float targetDistance = (time - drop.z) * timeScaleFactor;
    float relativeDistance = trueDistance - targetDistance;
    return Sigmoid(-(relativeDistance + sigmoidOffset) * sigmoidSharpness) * drop.w;
}

float4 MainPS(MainPixelInput input) : COLOR0
{
    // first computing blend factor from drops
    float blendFactor = 0.0;
    for (int i = 0; i < MAX_DROPS; i++) {
        if (i < numDrops) {
            blendFactor += GetDropFactor(drops[i], input.Position);
        }
    }

    // then getting color based on blend factor
    float firstBlend = blendFactor;
    float4 firstColor = GetOffsetted(FirstTextureSampler, input.TexCoord, firstBlend);

    float secondBlend = 1.0 - blendFactor;
    float4 secondColor = GetOffsetted(SecondTextureSampler, input.TexCoord, secondBlend);

    return (firstColor * firstBlend) + (secondColor * secondBlend);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
}
