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

    return float4(0,0,0, 1);
}


technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}