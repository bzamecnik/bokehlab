uniform sampler2D depthTexture0;
uniform sampler2D depthTexture1;
uniform sampler2D depthTexture2;
uniform sampler2D depthTexture3;

void main() {
	vec2 coords = gl_TexCoord[0].st;
	//float undefinedValue = 0.0;
	//vec4 packedDepth = vec4(undefinedValue);
	vec4 packedDepth = vec4(
		texture2D(depthTexture0, coords).r,
		texture2D(depthTexture1, coords).r,
		texture2D(depthTexture2, coords).r,
		texture2D(depthTexture3, coords).r
	);
	gl_FragColor = packedDepth;
	
	// visualization of the first three layers
	//gl_FragColor = vec4(1.0 - packedDepth.rgb, 1.0);
}