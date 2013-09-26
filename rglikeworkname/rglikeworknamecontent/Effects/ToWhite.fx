float4x4 World;
float4x4 View;
float4x4 Projection;

sampler ColorMap : register(s0);

struct VertexShaderOutput
{
	float4 color : COLOR0;
	float2 texCoord : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 base = tex2D(ColorMap, input.texCoord);
    return float4(0, 0, 0, base.a);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
