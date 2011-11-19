// produces the next min/max N-buffer level either from the previous N-buffer level

uniform sampler2D prevLevelMinTexture; // outside clamped to 1
uniform sampler2D prevLevelMaxTexture; // outside clamped to 0
// offset for the current level in the texture coordinates (x, y, 0)
uniform vec3 offset;

void main() {
	vec2 coords = gl_TexCoord[0].st;
	float depthAmin = texture2D(prevLevelMinTexture, coords).r;
	float depthBmin = texture2D(prevLevelMinTexture, coords + offset.xz).r;
	float depthCmin = texture2D(prevLevelMinTexture, coords + offset.zy).r;
	float depthDmin = texture2D(prevLevelMinTexture, coords + offset.xy).r;
	
	float minValue = min(min(depthAmin, depthBmin), min(depthCmin, depthDmin));
	
	float depthAmax = texture2D(prevLevelMaxTexture, coords).g;
	float depthBmax = texture2D(prevLevelMaxTexture, coords + offset.xz).g;
	float depthCmax = texture2D(prevLevelMaxTexture, coords + offset.zy).g;
	float depthDmax = texture2D(prevLevelMaxTexture, coords + offset.xy).g;
	
	float maxValue = max(max(depthAmax, depthBmax), max(depthCmax, depthDmax));
	
	gl_FragColor = vec4(minValue, maxValue, 0, 0);
	//gl_FragColor = vec4(minValue, 0, 0, 0);
	//gl_FragColor = vec4(0, maxValue, 0, 0);
	//gl_FragColor = vec4(depthDmin, depthDmax, 0, 0);
}