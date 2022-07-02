float uCellSize;
float uThreshold;

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

// cellular2D Worley Noise code adapted from Stefan Gustavson: https://weber.itn.liu.se/~stegu/GLSL-cellular/
// Unfortunately, it uses too just a bit too many instruction slots for ps_2_0

// Permutation polynomial: (34x^2 + x) mod 289
float3 permute(float3 x) {
	return ((34 * x + 1) * x) % 289;
}

// Cellular noise, returning F1 and F2 in a vec2.
// Standard 3x3 search window for good F1 and F2 values
float2 cellular(float2 P) {
	float K = 0.142857142857; // 1/7
	float Ko = 0.428571428571; // 3/7
	float jitter = 1; // Less gives more regular pattern
	float2 Pi = floor(P) % 289;
 	float2 Pf = frac(P);
	float3 oi = float3(-1, 0, 1);
	float3 of = float3(-0.5, 0.5, 1.5);
	float3 px = permute(Pi.x + oi);
	float3 p = permute(px.x + Pi.y + oi); // p11, p12, p13
	float3 ox = frac(p * K) - Ko;
	float3 oy = (floor(p * K) % 7) * K - Ko;
	float3 dx = Pf.x + 0.5 + jitter * ox;
	float3 dy = Pf.y - of + jitter * oy;
	float3 d1 = dx * dx + dy * dy; // d11, d12 and d13, squared
	p = permute(px.y + Pi.y + oi); // p21, p22, p23
	ox = frac(p * K) - Ko;
	oy = (floor(p * K) % 7) * K - Ko;
	dx = Pf.x - 0.5 + jitter * ox;
	dy = Pf.y - of + jitter * oy;
	float3 d2 = dx * dx + dy * dy; // d21, d22 and d23, squared
	p = permute(px.z + Pi.y + oi); // p31, p32, p33
	ox = frac(p * K) - Ko;
	oy = (floor(p * K) % 7) * K - Ko;
	dx = Pf.x - 1.5 + jitter * ox;
	dy = Pf.y - of + jitter * oy;
	float3 d3 = dx * dx + dy * dy; // d31, d32 and d33, squared
	// Sort out the two smallest distances (F1, F2)
	float3 d1a = min(d1, d2);
	d2 = max(d1, d2); // Swap to keep candidates for F2
	d2 = min(d2, d3); // neither F1 nor F2 are now in d3
	d1 = min(d1a, d2); // F1 is now in d1
	d2 = max(d1a, d2); // Swap to keep candidates for F2
	d1.xy = (d1.x < d1.y) ? d1.xy : d1.yx; // Swap if smaller
	d1.xz = (d1.x < d1.z) ? d1.xz : d1.zx; // F1 is in d1.x
	d1.yz = min(d1.yz, d2.yz); // F2 is now not in d2.yz
	d1.y = min(d1.y, d1.z); // nor in  d1.z
	d1.y = min(d1.y, d2.x); // F2 is in d1.y, we're done.
	return sqrt(d1.xy);
}

float4 FissurePS(FSIn fsIn) : COLOR0
{
	float2 F = cellular(fsIn.Position);
	float facets = 0.1 + (F.y - F.x);
	float dots = smoothstep(0.05, 0.1, F.x);
	float n = facets * dots;
    return float4(n, n, n, 1);
}

technique FissureTechnique
{
    pass BackgroundPass
    {
        VertexShader = compile vs_2_0 FissureVS();
        PixelShader = compile ps_2_0 FissurePS();
    }
}