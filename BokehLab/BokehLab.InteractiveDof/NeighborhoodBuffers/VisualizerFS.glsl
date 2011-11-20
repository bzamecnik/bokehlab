#version 120
#extension GL_EXT_texture_array : enable
//#extension GL_EXT_gpu_shader4 : enable

uniform sampler2DArray nBufferLayerTexture;
uniform vec2 colorMask;
uniform int layer;

void main() {
	vec3 coords = vec3(gl_TexCoord[0].st, layer);
	
	vec2 value = texture2DArray(nBufferLayerTexture, coords).rg;
	
	gl_FragColor = vec4(value * colorMask, 0, 1);
	//gl_FragColor = vec4(1, 0, 1, 1);
}
