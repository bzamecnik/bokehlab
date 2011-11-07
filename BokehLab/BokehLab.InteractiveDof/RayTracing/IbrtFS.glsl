// TODO:
// - height field intersection
// - simple thin lens model
// - lens sampling
// - arrays of textures are possible

// sensor size in camera space (width, height)
uniform vec2 sensorSize;
// Z coordinate of the sensor center (usually in +z half-space)
uniform float sensorZ;
// near and far plane distances (unsigned), the planes lie in -z half-space
uniform float near;
uniform float far;
uniform float lensFocalLength;
uniform float lensApertureRadius;
uniform mat4 perspective;

uniform sampler2D colorTexture;
uniform sampler2D depthTexture;

// convert from [0;1]^3 to [-1;1]^3
vec3 smallToBigCube(vec3 vector) {
    return 2.0 * vector - vec3(1.0);
}

// convert from [-1;1]^3 to [0;1]^3
vec3 bigToSmallCube(vec3 vector) {
    return 0.5 * (vector + vec3(1.0));
}

// Intersect the height field defined by the depth texture and return the
// color at the intersection point as in the color textrere. In case of no
// interection return (0, 0, 0).
// The ray is given by its starting end ending points in the normalized
// frustum space. The start point usually can be on the near plane (z=0),
// the end point can be on the far plane (z=1).
vec3 intersectHeightField(vec3 start, vec3 end) {
    int steps = 5;
    vec3 rayStep = (end - start) / float(steps);
    vec3 currentPos = start;
    vec3 bestPos = start;
    bool isecFound = false;
    vec3 color = vec3(0, 0, 0);
    for (int i = 0; i < steps; i++) {
        currentPos += rayStep;
        vec2 texPos = currentPos.xy;
        float layerDepth = texture2D(depthTexture, texPos).r;
        if (currentPos.z >= layerDepth) {
            bestPos = currentPos;
            isecFound = true;
        }
	}
	if (isecFound) {
        color = texture2D(colorTexture, bestPos.xy).rgb;
    }
    return color;
}

vec3 transformPoint(mat4 matrix, vec3 point) {
	vec4 result = matrix * vec4(point, 1);
	return result.xyz / result.w;
}

void main() {
    // pixel postion in the normalized sensor space [0;1]^2
	vec2 texCoord = gl_TexCoord[0].st;

    // pixel corner in camera space
	// TODO: offset to pixel center or jitter the pixel area
	vec3 pixelPos = vec3((0.5 - texCoord) * sensorSize, sensorZ);

    vec3 colorSum = vec3(0.0, 0.0, 0.0);
    ivec2 steps = ivec2(3, 3);
    
    float apertureRadius = lensApertureRadius * 0.025;
    
    vec2 offsetStep = (2.0 * apertureRadius) * vec2(1.0 / vec2(steps - ivec2(1, 1)));
    for (int y = 0; y < steps.y; y++) {
        for (int x = 0; x < steps.x; x++) {
            vec3 lensOffset = vec3(
				float(x) * offsetStep.x - apertureRadius,
				float(y) * offsetStep.y - apertureRadius, 0.0);
            
            //vec3 lensOffset = vec3(0, 0, 0);
            
            vec3 rayDirection = lensOffset - pixelPos;
            vec3 unitZRayDir = rayDirection / rayDirection.z;
            
            vec3 startCamera = lensOffset + (-near) * unitZRayDir;
            // convert the start and end points to from [-1;1]^3 to [0;1]^3
            vec3 start = bigToSmallCube(transformPoint(perspective, startCamera));
            
            vec3 endCamera = lensOffset + (-far) * unitZRayDir;
            vec3 end = bigToSmallCube(transformPoint(perspective, endCamera));
            
			colorSum += intersectHeightField(start, end);
        }
	}
	
	gl_FragColor.rgb = colorSum / float(steps.x * steps.y);
	//gl_FragColor.rg = texture2D(colorTexture, start.xy).rgb;
	
	//if (texCoord.y > 0.5) {
		//gl_FragColor.r = (end.z - (1)) * 0.5 + 0.5;
		////gl_FragColor.r = sign(start.z) * 0.5 + 0.5;
	//} else {
		//gl_FragColor.r = texCoord.x;
	//}
	
    gl_FragColor.a = 1.0;
}
