// produces the next min/max N-buffer level either from the previous N-buffer level

uniform sampler2D prevLevelMinTexture; // outside clamped to 1
uniform sampler2D prevLevelMaxTexture; // outside clamped to 0
// offset for the current level in the texture coordinates (x, y, 0)
uniform vec3 offset;

void main() {
	vec2 coords = gl_TexCoord[0].st;
	vec4 depthAmin = texture2D(prevLevelMinTexture, coords);
	vec4 depthBmin = texture2D(prevLevelMinTexture, coords + offset.xz);
	vec4 depthCmin = texture2D(prevLevelMinTexture, coords + offset.zy);
	vec4 depthDmin = texture2D(prevLevelMinTexture, coords + offset.xy);
	
	float minValue = min(min(depthAmin, depthBmin), min(depthCmin, depthDmin));
	
	vec4 depthAmax = texture2D(prevLevelMaxTexture, coords);
	vec4 depthBmax = texture2D(prevLevelMaxTexture, coords + offset.xz);
	vec4 depthCmax = texture2D(prevLevelMaxTexture, coords + offset.zy);
	vec4 depthDmax = texture2D(prevLevelMaxTexture, coords + offset.xy);
	
	float maxValue = max(max(depthAmax, depthBmax), max(depthCmax, depthDmax));
	
	gl_FragColor = vec4(minValue, maxValue, 0, 0);
}