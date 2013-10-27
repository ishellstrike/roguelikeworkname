float screenWidth;
float screenHeight;
 
float lightStrength;
float lightRadius;
float3 lightPosition;
float3 lightColor;
 
struct VertexToPixel
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};
 
struct PixelToFrame
{
    float4 Color : COLOR0;
};
 
VertexToPixel VertexToPixelShader(float4 inPos: POSITION0, float2 texCoord: TEXCOORD0)
{
    VertexToPixel Output = (VertexToPixel)0;
 
    Output.Position = inPos;
    Output.TexCoord = texCoord;
 
    return Output;
}
 
PixelToFrame PointLightShader(VertexToPixel PSIn) : COLOR0
{
    PixelToFrame Output = (PixelToFrame)0;
	float3 normal = float3(0,0,-1);
 
    float depth = 1;
 
    float3 pixelPosition;
    pixelPosition.x = screenWidth * PSIn.TexCoord.x;
    pixelPosition.y = screenHeight * PSIn.TexCoord.y;
    pixelPosition.z = 1;
    //pixelPosition.w = 1.0f;
 
    float3 shading;
    float3 lightDirection = lightPosition - pixelPosition;
    float distance = 1 / length(lightPosition - pixelPosition) * lightStrength;
    float amount = max(dot(normal + depth, normalize(distance)), 0);
 
    float coneAttenuation = saturate(1.0f - length(lightDirection) / lightRadius);
 
    shading = distance * amount * coneAttenuation * lightColor;
 
    Output.Color = float4(shading.r, shading.g, shading.b, 1);
    return Output;
}
 
technique DeferredPointLight
{
    pass Pass1
    {
		VertexShader = compile vs_2_0 VertexToPixelShader();
        PixelShader = compile ps_2_0 PointLightShader();
    }
}