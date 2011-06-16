uniform sampler3D lrtf3dTexture;
uniform float zCoord;
uniform vec4 valueMask;

uniform sampler1D colorMapTexture;

void main() {
	float s = gl_TexCoord[0].s;
	float t = gl_TexCoord[0].t;

	// NOTE:
	//  Use the folling code for cartesian->polar coordinate conversion.
	//
	//s = 2 * s - 1;
	//t = 2 * t - 1;
	//float sMod = atan(s, t) / (2 * 3.1415926535) + 0.5;
	//float tMod = 1 - sqrt(s * s + t * t);
	//vec4 lrtfValue = texture3D(lrtf3dTexture, vec3(sMod, tMod, zCoord));
	
	vec4 lrtfValue = texture3D(lrtf3dTexture, vec3(s, t, zCoord));
	//vec4 lrtfValue = vec4(sMod, tMod, 0, 0);
	float intensity = dot(lrtfValue, valueMask);
	vec3 color = texture1D(colorMapTexture, intensity);
	gl_FragColor.rgb = color;
	gl_FragColor.a = 1;
}