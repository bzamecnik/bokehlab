#version 120
#extension GL_EXT_texture_array : enable
#extension GL_EXT_gpu_shader4 : enable

// TODO:
// - N-buffer acceleration
// - use more than 4 layers

// sensor size in camera space (width, height)
uniform vec2 sensorSize;
// Z coordinate of the sensor center (usually in +z half-space)
//uniform float sensorZ;
uniform vec3 sensorShift;
// near and far plane distances (unsigned), the planes lie in -z half-space
uniform float near;
uniform float far;
uniform float lensFocalLength;
uniform float lensApertureRadius;
//uniform mat4 perspective;

uniform mat3 sensorTransform;

// left, right, bottom, top
uniform vec4 frustumBounds;

uniform sampler2DArray packedDepthTexture;
uniform sampler2DArray colorTexture;

// precomputed pseudo-random senzor and lens sample positions
// - a 2D vector - lens position (x,y) in camera space (unit disk [-1;1])
// - samples for one fragment stored in R dimension
// - samples for neighbor pixel tiled in S and T dimensions
// TODO: two 2D vectors could be packed into one 4D vector
uniform sampler3D lensSamplesTexture;
uniform sampler1D pixelSamplesTexture;
// number of samples - allowed values: [0; length(lensSamples)]
uniform int sampleCount;
uniform float sampleCountInv;

uniform vec2 screenSize;
uniform vec2 screenSizeInv;

uniform vec2 cameraShift;


uniform sampler2DArray nBuffersTexture;

// N-buffers size in pixels (width, height)
uniform vec2 nBuffersSize;

// get min/max values in a rectangular window (pos, pos + size) in [0.0; 1.0]^2
vec2 queryMinMaxNBuffers(vec2 position, vec2 size)
{
	vec2 rectSizeInPixels = size * nBuffersSize;
	float maxSize = max(rectSizeInPixels.x, rectSizeInPixels.y);
	int level = int(ceil(log2(maxSize)));
	vec2 minmax = texture2DArray(nBuffersTexture, vec3(position, level)).rg;
	return minmax;
	////return vec2(level / float(ceil(log(max(nBuffersSize.x, nBuffersSize.y)) * ln2Inv)));
	//return vec2(level / 8.0, 0);
	//return size;
	//return position;
}


// inverse frustum size (for a simplified frustum transform)
// 1 / ((right - left), (top - bottom))
vec2 frustumSizeInv = 1.0 / (frustumBounds.xz - frustumBounds.yw);
float nearOverFar = near / far;


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
        float layerDepth = texture2DArray(packedDepthTexture, vec3(currentPos.xy, 0)).r;
        //if (!isecFound && (currentPos.z >= layerDepth)) {
        if (currentPos.z >= layerDepth) {
            bestPos = currentPos;
            //isecFound = true;
            return texture2DArray(colorTexture, vec3(bestPos.xy, 0)).rgb;
        }
	}
	//if (isecFound) {
        //color = texture2DArray(colorTexture, vec3(bestPos.xy, 0)).rgb;
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
        float layerDepth = texture2DArray(packedDepthTexture, vec3(middlePos.xy, 0)).r;
        if (middlePos.z >= layerDepth) {
			endPos = middlePos;
            bestPos = middlePos;
            isecFound = true;
        } else {
			startPos = middlePos;
        }
	}
	if (isecFound) {
        color = texture2DArray(colorTexture, vec3(bestPos.xy, 0)).rgb;
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
        float layerDepth = texture2DArray(packedDepthTexture, vec3(currentPos.xy, 0)).r;
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
        float layerDepth = texture2DArray(packedDepthTexture, vec3(middlePos.xy, 0)).r;
        if (middlePos.z >= layerDepth) {
			endPos = middlePos;
            bestPos = middlePos;
        } else {
			startPos = middlePos;
        }
	}
	
	if (isecFound) {
        color = texture2DArray(colorTexture, vec3(bestPos.xy, 0)).rgb;
    }
    return color;
}

vec3 intersectHeightFieldLinearDiscontinuousThenBinary(vec3 startPos, vec3 endPos) {
    int linearSteps = 10;
    int binarySteps = 5;
    
    vec3 rayStep = (endPos - startPos) / float(linearSteps);
    vec3 currentPos = startPos;
    vec3 bestPos = startPos;
    float lastHfDepth = texture2DArray(packedDepthTexture, vec3(startPos.xy, 0)).r;
    bool insideHeightField = lastHfDepth < startPos.z;
    float depthDerivativeLimit = 5.0;
    float rayStepLen = length(rayStep.xy);
    float dxyLenInv = 1.0 / rayStepLen;
    bool isecFound = false;
    float pixelDiagLen = length(1.0 / screenSize);
    
    for (int i = 0; !isecFound && (i < linearSteps); i++) {
        currentPos += rayStep;
        float layerDepth = texture2DArray(packedDepthTexture, vec3(currentPos.xy, 0)).r;
        // directional difference in the ray xy direction
        float hfDerivative = abs((layerDepth - lastHfDepth) * dxyLenInv);
		// after stepping over a discontinuity check for the inside/outside position again.
		if ((rayStepLen > pixelDiagLen) &&
		(hfDerivative > depthDerivativeLimit)) {
			insideHeightField = layerDepth < currentPos.z;
		}
        if (!isecFound && ((currentPos.z >= layerDepth) ^^ insideHeightField)) {
            bestPos = currentPos;
            isecFound = true;
        }
        lastHfDepth = layerDepth;
	}
	
	if (!isecFound) {
		binarySteps = 0;
    }
    
    startPos = bestPos;
    endPos = bestPos - rayStep;
    vec3 middlePos;
    
    for (int i = 0; i < binarySteps; i++) {    
        middlePos = 0.5 * (startPos + endPos);
        float layerDepth = texture2DArray(packedDepthTexture, vec3(middlePos.xy, 0)).r;
        if (middlePos.z >= layerDepth) {
			endPos = middlePos;
            bestPos = middlePos;
        } else {
			startPos = middlePos;
        }
	}
	
	vec3 color = vec3(0, 0, 0);
	if (isecFound) {
		color = texture2DArray(colorTexture, vec3(bestPos.xy, 0)).rgb;
	}
    return color;
}

bool testIntersection(vec2 currentPixel, vec3 entry, vec3 exit, inout vec3 color) {
	// the height field is sampled at pixel centers  
    vec2 depthTestPos = (vec2(0.5) + currentPixel) * screenSizeInv;
    //vec2 colorPos = currentPixel * screenSizeInv;
    vec2 colorPos = depthTestPos;
    
    vec4 layersZ = texture2DArray(packedDepthTexture, vec3(depthTestPos, 0));
    
    // this epsilon prevents artifact within objects arising from the
    // (virtual) nearest-neighbor interpolation of the depth layer
	float epsilonForDepthTest = 0.01;
    
    // compare up to 4 layers at once for intersection
    //bvec4 comparison = (vec4(entry.z) < layersZ) && (layersZ <= vec4(exit.z + epsilonForDepthTest));
    ivec4 comparison = ivec4(sign(layersZ - vec4(entry.z)) + sign(vec4(exit.z + epsilonForDepthTest) - layersZ));
    // intersection happens at the first layer with result value 2 (ie. both sign are positive)
    if (comparison.x == 2)
    {
		//color = vec3(1,1,1); // colors for a nice visualization of occlusion layers
        color = texture2DArray(colorTexture, vec3(colorPos, 0)).rgb;
        return true;
    }
    else if (comparison.y == 2)
    {
		//color = vec3(0,0,1);
        color = texture2DArray(colorTexture, vec3(colorPos, 1)).rgb;
        return true;
    }
    else if (comparison.z == 2)
    {
		//color = vec3(0,1,0);
        color = texture2DArray(colorTexture, vec3(colorPos, 2)).rgb;
        return true;
    }
    else if (comparison.w == 2)
    {
		//color = vec3(1,0,0);
        color = texture2DArray(colorTexture, vec3(colorPos, 3)).rgb;
        return true;
    }
    return false;
}


vec2 getPixelCorner(vec2 position, vec2 relDir)
{
    vec2 corner = floor(position);
    float epsilon = 0.00001;
    // in case the position lies on its floor edge and the ray goes
    // across the edge move the corner one more pixel in the ray direction
    if ((relDir.x < 0.0) && (position.x - corner.x < epsilon))	
    {
        corner.x -= 1.0;
    }
    if ((relDir.y < 0.0) && (position.y - corner.y < epsilon))
    {
        corner.y -= 1.0;
    }
    return corner;
}

// "A Fast Voxel Traversal Algorithm for Ray Tracing", John Amanatides and Andrew Woo [amanatides1999]
// - 2D implementation
// - in addition to pure pixels we need also entry and exit points where the
//   ray crosses pixel boundaries
// - axis-aligned directions are modified a bit to prevent handled
//   signularities differently
// - start and end pixel corners are computed differently to assure
//   the end pixel will not be missed
vec3 intersectHeightFieldPerPixel(vec3 startPos, vec3 endPos) {
	vec3 start = vec3((startPos.xy * screenSize), startPos.z);
	vec3 end = vec3((endPos.xy * screenSize), endPos.z);
	vec3 dir = end - start;
	float epsilonForRayDir = 0.0001;

	// avoid division by zero in rayDirInv for tMax and tDelta
	// and also avoid rayStep components being zero
	if (abs(dir.x) < epsilonForRayDir)
    {
        dir.x = epsilonForRayDir;
    }
    if (abs(dir.y) < epsilonForRayDir)
    {	
        dir.y = epsilonForRayDir;
    }
    
    vec2 rayStep = sign(dir.xy); // {-1,1}^2
    vec2 currentPixel = getPixelCorner(start.xy, rayStep);
    // use the modified ray end
    vec2 endPixel = getPixelCorner(start.xy + dir.xy, -rayStep);

	vec2 boundary = currentPixel + max(rayStep, 0.0); // {0,1}^2
    vec2 rayDirInv = 1.0 / dir.xy;

    vec2 tMax = (boundary - start.xy) * rayDirInv;
    vec2 tDelta = rayStep * rayDirInv;
    
    vec3 entry = start;
    vec3 exit = start;

	vec3 color = vec3(0.0);
	int iteration = 0;
	while ((iteration < 100) && (currentPixel != endPixel)) {
        if (tMax.x < tMax.y)
        {
            exit = start + tMax.x * dir;
            tMax.x += tDelta.x;
            currentPixel.x += rayStep.x;
        }
        else
        {
            exit = start + tMax.y * dir;
            tMax.y += tDelta.y;
            currentPixel.y += rayStep.y;
        }

		if (testIntersection(currentPixel, entry, exit, color)) {
			break;
		}

        entry = exit;
        iteration++;
	}

    // currentPixel == endPixel
	testIntersection(currentPixel, entry, endPos, color);

	return color;
}

// Intersect the height field with a ray starting at the
// position 'lensPos' going to the 'rayDirection' direction.
//
// Assuming that lensPos.z == 0.
vec3 intersectHeightField(vec3 lensPos, vec3 rayDirection) {
	rayDirection /= rayDirection.z; // normalize to a unit z step
    // ray start in camera space
    vec3 startCamera = lensPos + (-near) * rayDirection;
    // convert it to the frustum space [0;1]^3
    vec3 start = vec3((startCamera.xy - frustumBounds.yw) * frustumSizeInv, 0);
    
    // ray end in camera space
    vec3 endCamera = lensPos + (-far) * rayDirection;
    vec3 end = vec3((endCamera.xy * nearOverFar - frustumBounds.yw) * frustumSizeInv, 1);

	// TODO: clip the footprint (iteratively) by using N-buffers
	// - repeat several times:
	//   - find min/max depth in rectangle (start, end - start)
	//   - find new ray start and end points at depths min, max
	
	vec3 dir = end - start;
	vec3 clippedStart = start;
	vec3 clippedEnd = end;
	for (int i = 0; i < 1; i++) {
		// corner position and size of bounding rectangle of the ray footprint
		vec2 rectPosition = min(clippedStart.xy, clippedEnd.xy);
		vec2 rectSize = abs(clippedEnd.xy - clippedStart.xy);
		vec2 rectSizeInPixels = rectSize * nBuffersSize;
		// only accelerate when the footprint is large
		// length(rectSizeInPixels) > N <=> (length(rectSizeInPixels))^2 > N^2
		
		//return vec3(length(rectSizeInPixels) > 10, 0, 0);
		
		bool shouldClip = dot(rectSizeInPixels, rectSizeInPixels) > 10;
		//if (dot(rectSizeInPixels, rectSizeInPixels) > 20) {
		//if (length(rectSizeInPixels) > 10) {
		vec2 minMaxDepth = queryMinMaxNBuffers(rectPosition, rectSize);
			//return vec3(minMaxDepth, 0);
		if (shouldClip){
			clippedStart = start + (minMaxDepth.r - 0.005) * dir;
			clippedEnd = start + minMaxDepth.g * dir;
		}
		//}
	}

	//return intersectHeightFieldLinear(start, end);
	//return intersectHeightFieldLinearDiscontinuousThenBinary(start, end);
	//return intersectHeightFieldPerPixel(start, end);
	return intersectHeightFieldPerPixel(clippedStart, clippedEnd);
	//return clippedStart;
	//return clippedStart - start;
	//return end - clippedEnd;
	//return vec3(clippedEnd.xy - clippedStart.xy, 0);
	//return vec3(end.xy - start.xy, 0);
}

//vec3 transformPoint(mat4 matrix, vec3 point) {
	//vec4 result = matrix * vec4(point, 1);
	//return result.xyz / result.w;
//}

vec3 thinLensTransformPoint(vec3 point) {
	return point / (1.0 - (abs(point.z) / lensFocalLength));
}

// trace a ray going from sensorPos to lensPos
// evaluate the incoming color
vec3 traceRay(vec3 senzorPos, vec3 lensPos) {
	// create lens ray (lensPos, rayDirection)
    // TODO: take care of lensPos.z != 0
    // - pinhole
    //vec3 rayDirection = lensPos - senzorPos;
    // - thin lens
    vec3 rayDirection = thinLensTransformPoint(senzorPos) - lensPos;
    
	return intersectHeightField(lensPos, rayDirection);
}

vec3 estimateRadianceNonJittered(vec3 pixelPos) {
	vec3 colorSum = vec3(0.0, 0.0, 0.0);
    ivec2 steps = ivec2(4, 4);
    
    vec2 offsetStep = (2.0 * lensApertureRadius) * vec2(1.0 / vec2(steps - ivec2(1, 1)));
    for (int y = 0; y < steps.y; y++) {
        for (int x = 0; x < steps.x; x++) {	
            vec3 lensOffset = vec3(
				float(x) * offsetStep.x - lensApertureRadius,
				float(y) * offsetStep.y - lensApertureRadius, 0.0);
            colorSum += traceRay(pixelPos, lensOffset);
        }
	}
	
	return colorSum / float(steps.x * steps.y);
}

vec3 estimateRadianceJittered(vec3 pixelPos) {
	vec3 colorAccum = vec3(0, 0, 0);
	ivec3 lensJitterSize = textureSize3D(lensSamplesTexture, 0);
	vec2 jitterCoords = gl_FragCoord.st;
	jitterCoords.t = screenSize.y - jitterCoords.t;
	vec3 samplesIndex = vec3(jitterCoords / vec2(lensJitterSize.st), 0.0);
	// TODO: check dFdx(texcoord.x) out
	vec3 samplesIndexStep = vec3(0, 0, 1.0 / (float(sampleCount) - 1.0));
	// [0;1]^2 pixel sample to camera space
	//vec2 pixelSampleToCamera = sensorSize * screenSizeInv;
	//float pixelSampleTexIndex = 0.0;
	for (int i = 0; i < sampleCount; i += 1) {
		vec2 lensPos = texture3D(lensSamplesTexture, samplesIndex).st;
		// TODO: The offset should be also rotated using the sensorTransform
		// for a tilt-shift sensor! At best we should precompute transformed
		// unit pixel vectors and multiply them with the offset.
		//vec2 pixelOffset = pixelSampleToCamera * texture1D(pixelSamplesTexture, pixelSampleTexIndex).st;
		lensPos *= lensApertureRadius;
		//colorAccum += traceRay(pixelPos + vec3(pixelOffset, 0), vec3(lensPos, 0));
		colorAccum += traceRay(pixelPos, vec3(lensPos, 0));
		samplesIndex += samplesIndexStep;
		//pixelSampleTexIndex += sampleCountInv;
	}
	//return vec4(samplesIndex.b, 0, 0, 1);
	return colorAccum * sampleCountInv;
}

void main() {
    // pixel postion in the normalized sensor space [0;1]^2
	vec2 texCoord = gl_TexCoord[0].st;

    // pixel corner in camera space
    //vec3 pixelPos = vec3((0.5 - texCoord) * sensorSize, sensorZ);
	//vec3 pixelPos = vec3((0.5 - texCoord) * sensorSize, sensorShift.z);
	vec3 pixelPos = vec3((0.5 - texCoord) * sensorSize, 0);
	pixelPos = sensorShift + sensorTransform * pixelPos;

	//gl_FragColor = vec4(estimateRadianceNonJittered(pixelPos), 1.0);
	gl_FragColor = vec4(estimateRadianceJittered(pixelPos), 1.0);
	//gl_FragColor = vec4(traceRay(pixelPos, vec3(lensApertureRadius * cameraShift, 0.0)), 1.0);
}
