// ---------- vertex shader ----------

varying out vec2 texcoord0;

void main() {
	texcoord0 = gl_MultiTexCoord0.st;
	gl_Position = ftransform();
}

// ---------- fragment shader ----------

#version 140
#extension GL_EXT_gpu_shader4 : enable

uniform sampler2DRect colorTexture;
uniform sampler2DRect depthTexture;
in vec2 texcoord0;

void main() {
	// integer texture size (width, height)
	ivec2 size = textureSize(colorTexture);
	vec2 sizeInv = 1.0 / vec2(size);
	// integer position of current pixel
	ivec2 pos = texcoord0;

	vec3 color = vec3(0,0,0);
	float depth = 1;
	vec3 resultColorSum = vec3(0,0,0);
	
	//for (int i = 0; i < 5; i++) {
		//vec2 fpos = pos * sizeInv;
		color = texture2DRect(colorTexture, vec2(pos)).rgb;
		//depth = texture2D(depthTexture, fpos).r;
		resultColorSum += vec3(1 - depth, color.g, color.b);
		//pos.x += i;
	//}
	gl_FragColor.rgb = resultColorSum;
	gl_FragColor.a = 1;
}
