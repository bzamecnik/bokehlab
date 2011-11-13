
// produces the next min/max N-buffer level either from the original packed
// depth texture taking the extreme value from each vector component

uniform sampler2D prevLevelMinTexture; // outside clamped to 1
uniform sampler2D prevLevelMaxTexture; // outside clamped to 0
// offset for the current level in the texture coordinates (x, y, 0)
uniform vec3 offset;

float vecMin(vec4 vector) {
	min(min(vector.x, vector.y), min(vector.z, vector.w));
}

float vecMax(vec4 vector) {
	max(max(vector.x, vector.y), max(vector.z, vector.w));
}

void main() {
	vec2 coords = gl_TexCoord[0].st;
	vec4 depthAmin = texture2D(prevLevelMinTexture, coords);
	vec4 depthBmin = texture2D(prevLevelMinTexture, coords + offset.xz);
	vec4 depthCmin = texture2D(prevLevelMinTexture, coords + offset.zy);
	vec4 depthDmin = texture2D(prevLevelMinTexture, coords + offset.xy);
	
	float minValue = min(
		min(vecMin(depthAmin), vecMin(depthBmin)),
		min(vecMin(depthCmin), vecMin(depthDmin))
	);
	
	vec4 depthAmax = texture2D(prevLevelMaxTexture, coords);
	vec4 depthBmax = texture2D(prevLevelMaxTexture, coords + offset.xz);
	vec4 depthCmax = texture2D(prevLevelMaxTexture, coords + offset.zy);
	vec4 depthDmax = texture2D(prevLevelMaxTexture, coords + offset.xy);
	
	float maxValue = max(
		max(vecMax(depthAmax), vecMax(depthBmax)),
		max(vecMax(depthCmax), vecMax(depthDmax))
	);
	
	gl_FragColor = vec4(minValue, maxValue, 0, 0);
}