float2 pos;
float2 cpos;
float screenWidth = 1024;
float screenHeight = 768;

sampler ColorMap : register(s0);

struct VertexShaderOutput
{
	float4 color : COLOR0;
	float2 texCoord : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{  
	
    float4 base = tex2D(ColorMap, input.texCoord);

	float3 pixelPosition = float3(screenWidth * input.texCoord.x,
                            screenHeight * input.texCoord.y,0);
	float a = distance(cpos,pixelPosition)/screenHeight;

	float fll = float3(1-a,1-a,1-a);

    return float4(base.rgb * fll, base.a);
}


technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}