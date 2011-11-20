#version 120
#extension GL_EXT_texture_array : enable

uniform sampler2DArray layerTexture;
uniform int layerIndex;

void main() {
	vec3 coords = vec3(gl_TexCoord[0].st, layerIndex);
	gl_FragColor = texture2DArray(layerTexture, coords);
}

