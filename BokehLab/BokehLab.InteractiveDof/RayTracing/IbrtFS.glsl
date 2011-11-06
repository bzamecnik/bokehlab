// TODO:
// - height field intersection
// - simple thin lens model
// - lens sampling
// - arrays of textures are possible

uniform sampler2D colorTexture;
uniform sampler2D depthTexture;

vec3 ndcToTexture(vec3 vector) {
	return vec3(0.5 * (vector.x + 1.0), 0.5 * (vector.y + 1.0), vector.z);
}

vec3 textureToNdc(vec3 vector) {
	return vec3((2.0 * vector.x) - 1.0, (2.0 * vector.y) - 1.0, vector.z);
}

// Intersect the height field defined by the depth texture and return the
// color at the intersection point as in the color textrere. In case of no
// interection return (0, 0, 0).
// The ray is given by its starting end ending points in the normalized
// frustum space. The start point usually can be on the near plane (z=0),
// the end point can be on the far plane (z=1).
vec3 intersectHeightField(vec3 start, vec3 end) {
	int steps = 10;
	vec3 rayStep = (end - start) / float(steps);
	vec3 currentPos = start;
	vec3 bestPos = start;
	bool isecFound = false;
	for (int i = 0; i < steps; i++) {
		currentPos += rayStep;
		vec2 texPos = ndcToTexture(currentPos).xy;
		float layerDepth = texture2D(depthTexture, texPos).r;
		if (currentPos.z >= layerDepth) {
			bestPos = currentPos;
			isecFound = true;
		}
	}
	if (isecFound) {
		return texture2D(colorTexture, ndcToTexture(bestPos).xy).rgb;
	} else {
		return vec3(0, 0, 0);
	}
}

void main() {
	// pixel postion in the normalized sensor space [0;1]^2
	vec2 texCoord = gl_TexCoord[0].st;
	
	//gl_FragColor.rgb = texture2D(colorTexture, texCoord).bgr;
	//gl_FragColor.rgb = texture2D(depthTexture, texCoord).rgb;
	
	vec3 centerOfProjection = vec3(0.5, 0.5, 3);
	vec3 pixelPos = textureToNdc(vec3(texCoord, 0));
	
	vec3 colorSum = vec3(0, 0, 0);
	float apertureRadius = 0.02;
	int stepsX = 3;
	int stepsY = 3;
	vec2 offsetStep = (2 * apertureRadius) * vec2(1 / float(stepsX - 1), 1 / float(stepsX - 1));
	for (int y = 0; y < stepsY; y++) {
		for (int x = 0; x < stepsX; x++) {
			vec3 lensOffset = vec3(x * offsetStep.x - apertureRadius, y * offsetStep.y - apertureRadius, 0);
			
			vec3 rayDirection = pixelPos - (centerOfProjection + lensOffset);
			//rayDirection = normalize(rayDirection);
			float imageZ = -1.0;
			// intersection with the image plane at z = -1
			vec3 isec = rayDirection * imageZ / rayDirection.z;
			
			// intersection converted to [0; 1]^2 texture coords
			//vec2 isecTex = ndcToTexture(isec).xy;
			//gl_FragColor.rgb = texture2D(colorTexture, isecTex).rgb;
			
			vec3 start = centerOfProjection + lensOffset;
			// the height field depth in in [0; 1] interval while the ray
			// goes in the -z half-space
			vec3 end = vec3(isec.x, isec.y, -isec.z);
			
			colorSum += intersectHeightField(start, end);
		}
	}
	
	gl_FragColor.rgb = colorSum / float(stepsX * stepsY);
	
	gl_FragColor.a = 1;
}