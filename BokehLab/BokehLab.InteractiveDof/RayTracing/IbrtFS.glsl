// TODO:
// - height field intersection
// - simple thin lens model
// - lens sampling
// - arrays of textures are possible

uniform sampler2D colorTexture;
uniform sampler2D depthTexture;

void main() {
	// pixel postion in the normalized sensor space [0;1]^2
	vec2 texCoord = gl_TexCoord[0].st;
	//vec3 frameColor = texture2D(depthTexture, texCoord).r;
	//vec3 averageColor = texture2D(averageTexture, texCoord).rgb;
	//gl_FragColor = vec4(mix(averageColor, frameColor, frameWeight), 1.0);
	
	//gl_FragColor.rgb = texture2D(depthTexture, texCoord).rrr;
	gl_FragColor.rgb = vec3(1, 1, 0);
	gl_FragColor.a = 1;
}