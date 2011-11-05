uniform sampler2D depthTexture;

void main() {
	// TODO: provide a uniform precomputed vec2: 1 / textureSize(depthTexture, 0)
	float depth = texture2D(depthTexture, gl_FragCoord.xy / textureSize(depthTexture, 0));
	//
	////float depth = texture2D(depthTexture, gl_TexCoord[0].st);
	//
	//if ((depth == 1) || (gl_FragCoord.z <= depth)) {
	if (gl_FragCoord.z <= depth) {
		discard;
	}
	
	gl_FragColor = gl_Color;
	//gl_FragColor = vec4(1,0.4,0.5,1);
	//gl_FragColor = vec4(depth, 0.5, 0, 1);
}