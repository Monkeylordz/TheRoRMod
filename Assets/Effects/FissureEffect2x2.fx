float uThreshold;
float uScale;
float uAlpha;
float3 uDefaultColor;

struct VSIn
{
    float4 Position : POSITION0;
};

struct VSOut
{
	float4 Position : POSITION0;
    float2 Position2 : TEXCOORD0;
};

struct FSIn
{
	float4 Position : TEXCOORD0;
};

// Pass vertex position to pixel shader
VSOut FissureVS(VSIn input)
{
    VSOut output;
	output.Position = input.Position;
    output.Position2 = input.Position.xy;
    return output;
}

// cellular2x2 Worley Noise code adapted from Stefan Gustavson: https://weber.itn.liu.se/~stegu/GLSL-cellular/

// Permutation polynomial: (34x^2 + x) mod 289
float4 permute(float4 x) {
	return ((34 * x + 1) * x) % 289;
}

// Cellular noise, returning F1 and F2 in a vec2.
// Sped up by using 2x2 search window instead of 3x3, at the expense of some strong pattern artifacts.
float2 cellular2x2(float2 P) {
	float K = 0.142857142857;
	float K2 = 0.0714285714285;
	float jitter = 0.8;
	float2 Pi = floor(P) % 289;
 	float2 Pf = frac(P);
	float4 Pfx = Pf.x + float4(-0.5, -1.5, -0.5, -1.5);
	float4 Pfy = Pf.y + float4(-0.5, -0.5, -1.5, -1.5);
	float4 p = permute(Pi.x + float4(0, 1, 0, 1));
	p = permute(p + Pi.y + float4(0, 0, 1, 1));
	float4 ox = (p % 7) * K + K2;
	float4 oy = (floor(p * K) % 7) * K + K2;
	float4 dx = Pfx + jitter * ox;
	float4 dy = Pfy + jitter * oy;
	float4 d = dx * dx + dy * dy; // d11, d12, d21 and d22, squared
	// Find both F1 and F2
	d.xy = (d.x < d.y) ? d.xy : d.yx; // Swap if smaller
	d.xz = (d.x < d.z) ? d.xz : d.zx;
	d.xw = (d.x < d.w) ? d.xw : d.wx;
	d.y = min(d.y, d.z);
	d.y = min(d.y, d.w);
	return sqrt(d.xy);
}

// Uses Worley Noise with a threshold to simulate to a starry background
float4 FissurePS(FSIn fsIn) : COLOR0
{
	float2 F = cellular2x2(fsIn.Position * uScale);
	float n = 1 - 1.5 * F.x;

	if (n < uThreshold)
	{
		return float4(uDefaultColor, uAlpha);
	}

    return float4(n, n, n, uAlpha);
}

technique FissureTechnique
{
    pass BackgroundPass
    {
        VertexShader = compile vs_2_0 FissureVS();
        PixelShader = compile ps_2_0 FissurePS();
    }
}