// extension for textureSize2D()
#extension GL_EXT_gpu_shader4 : enable

uniform sampler2D colorTexture;
uniform sampler2D depthTexture;

void main() {
	// integer texture size (width, height)
	ivec2 size = textureSize2D(colorTexture, 0);
	vec2 sizeInv = 1.0 / vec2(size);
	vec2 nPos = gl_TexCoord[0].st;
	vec2 iPos = gl_FragCoord;
	
	float radius = 10;
	// variable blur radius:
	//radius *= abs(texture2D(depthTexture, nPos).r - 0.5);
	
	vec2 iPosStart = iPos;
	int sqrtCount = 4;
	int count = 0;
	float xyStep = radius / float(sqrtCount);
	
	vec3 color = vec3(0,0,0);
	vec3 colorSum = vec3(0,0,0);

	for (float y = -radius; y <= radius; y += xyStep) {
		for (float x = -radius; x <= radius; x += xyStep) {
			vec2 offset = vec2(x, y);
			vec2 texpos = (iPos + offset) * sizeInv;
			color = texture2D(colorTexture, texpos).rgb;
			colorSum += color;
			count++;
		}
	}

	gl_FragColor.rgb = colorSum / float(count);
	gl_FragColor.a = 1.0;	
}
