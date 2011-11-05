#extension GL_EXT_gpu_shader4 : enable

uniform sampler2D noiseTexture;

float uniformRandom(vec2 pos, int index) {
	ivec2 size = textureSize2D(colorTexture, 0);
	int width = size.x;
	vec2 offset = vec2(index % width, index / width) / vec2(size);
	vec2 texPos = gl_TexCoord[0].st + offset;
	return texture2D(noiseTexture, texPos).r;
}

// just the noise
void main() {
	gl_FragColor.rgb = uniformRandom(gl_TexCoord[0].st, 0);
}

// averaged noise
void main() {
	vec3 colorSum = vec3(0,0,0);
	for (int i = 0; i < 32; i++) {
		colorSum.rgb += uniformRandom(gl_TexCoord[0].st, 802 * i);
	}
	gl_FragColor.rgb = colorSum / 32;
	gl_FragColor.a = 1.0;
}
