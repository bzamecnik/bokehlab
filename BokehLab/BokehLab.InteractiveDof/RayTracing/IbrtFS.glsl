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
//uniform mat4 perspective;

// left, right, bottom, top
uniform vec4 frustumBounds;

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
vec3 intersectHeightFieldLinear(vec3 start, vec3 end) {
    int steps = 10;
    vec3 rayStep = (end - start) / float(steps);
    vec3 currentPos = start;
    vec3 bestPos = start;
    // TODO: find out if it is better to terminate early or not
    //bool isecFound = false;
    vec3 color = vec3(0, 0, 0);
    for (int i = 0; i < steps; i++) {
        currentPos += rayStep;
        float layerDepth = texture2D(depthTexture, currentPos.xy).r;
        //if (!isecFound && (currentPos.z >= layerDepth)) {
        if (currentPos.z >= layerDepth) {
            bestPos = currentPos;
            //isecFound = true;
            
            //float diffZ= currentPos.z - layerDepth;
            //color = vec3(diffZ, diffZ, diffZ);
            //color = vec3(currentPos.z, currentPos.z, currentPos.z);
            //color = vec3(layerDepth, layerDepth, layerDepth);
            
            //float value =i/float(steps);
            //return vec3(value,value,value);
            
            //color = vec3(value,value,value);
            //color = vec3(layerDepth,layerDepth,layerDepth);
            //return texture2D(colorTexture, bestPos.xy).rgb;
            
            return texture2D(colorTexture, bestPos.xy).rgb;
        }
	}
	//if (isecFound) {
        //color = texture2D(colorTexture, bestPos.xy).rgb;
    //}
    return color;
}

vec3 intersectHeightFieldBinary(vec3 start, vec3 end) {
    int steps = 10;
    
    vec3 startPos = start;
    vec3 endPos = end;
    vec3 bestPos = start;
    vec3 middlePos;
    vec3 color = vec3(0, 0, 0);
    bool isecFound = false;
    
    for (int i = 0; i < steps; i++) {
        middlePos = 0.5 * (startPos + endPos);
        float layerDepth = texture2D(depthTexture, middlePos.xy).r;
        if (middlePos.z >= layerDepth) {
			endPos = middlePos;
            bestPos = middlePos;
            isecFound = true;
        } else {
			startPos = middlePos;
        }
	}
	if (isecFound) {
        color = texture2D(colorTexture, bestPos.xy).rgb;
    }
    return color;
}

vec3 intersectHeightFieldLinearThenBinary(vec3 start, vec3 end) {
    int linearSteps = 10;
    int binarySteps = 5;
    
    vec3 startPos = start;
    vec3 endPos = end;
    vec3 bestPos = start;
    vec3 color = vec3(0, 0, 0);
    bool isecFound = false;
    
    vec3 rayStep = (end - start) / float(linearSteps);
    vec3 currentPos = start;
    
    //for (int i = 0; (i < linearSteps) && !isecFound; i++) {
    for (int i = 0; (i < linearSteps); i++) {
        currentPos += rayStep;
        float layerDepth = texture2D(depthTexture, currentPos.xy).r;
        //if (currentPos.z >= layerDepth) {
        if (!isecFound && (currentPos.z >= layerDepth)) {
            bestPos = currentPos;
            isecFound = true;
        }
	}
    
    if (!isecFound) {
		binarySteps = 0;
    }
    
    startPos = bestPos;
    endPos = bestPos - rayStep;
    vec3 middlePos;
    
    for (int i = 0; i < binarySteps; i++) {    
        middlePos = 0.5 * (startPos + endPos);
        float layerDepth = texture2D(depthTexture, middlePos.xy).r;
        if (middlePos.z >= layerDepth) {
			endPos = middlePos;
            bestPos = middlePos;
        } else {
			startPos = middlePos;
        }
	}
	
	if (isecFound) {
        color = texture2D(colorTexture, bestPos.xy).rgb;
    }
    return color;
}

vec3 intersectHeightField(vec3 start, vec3 end) {
	return intersectHeightFieldLinearThenBinary(start, end);
}

//vec3 transformPoint(mat4 matrix, vec3 point) {
	//vec4 result = matrix * vec4(point, 1);
	//return result.xyz / result.w;
//}

vec3 thinLensTransformPoint(vec3 point) {
	return point / (1.0 - (abs(point.z) / lensFocalLength));
}

void main() {
    // pixel postion in the normalized sensor space [0;1]^2
	vec2 texCoord = gl_TexCoord[0].st;

    // pixel corner in camera space
	// TODO: offset to pixel center or jitter the pixel area
	vec3 pixelPos = vec3((0.5 - texCoord) * sensorSize, sensorZ);

    vec3 colorSum = vec3(0.0, 0.0, 0.0);
    ivec2 steps = ivec2(4, 4);
    
    float apertureRadius = lensApertureRadius;
    
    // inverse frustum size (for a simplified frustum transform)
    // 1 / ((right - left), (top - bottom))
    vec2 frustumSizeInv = 1.0 / (frustumBounds.xz - frustumBounds.yw);
    float nearOverFar = near / far;
    
    vec2 offsetStep = (2.0 * apertureRadius) * vec2(1.0 / vec2(steps - ivec2(1, 1)));
    for (int y = 0; y < steps.y; y++) {
        for (int x = 0; x < steps.x; x++) {	
            vec3 lensOffset = vec3(
				float(x) * offsetStep.x - apertureRadius,
				float(y) * offsetStep.y - apertureRadius, 0.0);
            
            // create lens ray (lensOffset, rayDirection)
            // TODO: take care of lensOffset.z != 0
            // - pinhole
            //vec3 rayDirection = lensOffset - pixelPos;
            // - thin lens
            vec3 rayDirection = thinLensTransformPoint(pixelPos) - lensOffset;
            rayDirection /= rayDirection.z; // normalize to a unit z step
            
            // ray start in camera space
            vec3 startCamera = lensOffset + (-near) * rayDirection;
            // convert it to the frustum space [0;1]^3
            vec3 start = vec3((startCamera.xy - frustumBounds.yw) * frustumSizeInv, 0);
            
            // ray end in camera space
            vec3 endCamera = lensOffset + (-far) * rayDirection;
            vec3 end = vec3((endCamera.xy * nearOverFar - frustumBounds.yw) * frustumSizeInv, 1);
            
			colorSum += intersectHeightField(start, end);
        }
	}
	
	gl_FragColor = vec4(colorSum / float(steps.x * steps.y), 1.0);
}
