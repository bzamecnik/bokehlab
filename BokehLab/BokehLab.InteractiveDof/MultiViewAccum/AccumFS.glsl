uniform sampler2D currentFrameTexture;
uniform sampler2D averageTexture;
uniform float frameWeight;

void main() {
	vec2 texCoord = gl_TexCoord[0].st;
	vec3 frameColor = texture2D(currentFrameTexture, texCoord).rgb;
	vec3 averageColor = texture2D(averageTexture, texCoord).rgb;
	gl_FragColor = vec4(mix(averageColor, frameColor, frameWeight), 1.0);
}