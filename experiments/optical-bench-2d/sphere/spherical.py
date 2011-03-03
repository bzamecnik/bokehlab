import math
from math import sin, cos, asin, acos, atan2

# Spherical coordinates represent a direction on a unit sphere.
#
# There are two parameters:
# - theta (elevation) [-PI/2; PI/2]
#   - goes from Z to -Z
# - phi (azimuth) [0; 2*PI]
#   - goes through: X, Y, -X, -Y
#
# The poles are on the Z axis: (0,0,1), (0,0,-1).
# The supporting plane is XY.
#
class SphericalCoords(object):
    def __init__(self, theta=0.0, phi=0.0, radius=1.0):
        self.theta = theta
        self.phi = phi
        self.radius = radius

    def fromCartesian(self, cartesian):
        x = cartesian[0]
        y = cartesian[1]
        z = cartesian[2]
        r = norm((x, y, z))
        theta = asin(z / r)
        phi = atan2(y, x)
        return SphericalCoords(theta, phi, r)

    def toCartesian(self):
        cosTheta = cos(self.theta)
        x = self.radius * cosTheta * cos(self.phi)
        y = self.radius * cosTheta * sin(self.phi)
        z = self.radius * sin(self.theta)
        return (x, y, z)

    def changeRadius(self, radius, centerDistance):
        theta = asin((centerDistance + self.radius * sin(self.theta)) / radius)
        return SphericalCoords(self.theta, self.phi, radius)

    # Compute reflection of an incoming direction represented by this spherical
    # coordinates which gets reflected by the supporting plane.
    def reflect(self):
        return SphericalCoords(self.theta, self.phi + math.pi, self.radius)

    # Compute refraction of an incoming direction represented by this spherical
    # coordinates which gets refrected by the supporting plane.
    #
    # etaIn
    #   represents the index of refraction of the material in the half-space
    #   where the incoming direction goes from
    # etaOut
    #   represents the index of refraction of the material in the other
    #   half-space than where the incoming direction goes from
    def refract(self, etaIn, etaOut):
        #print("theta: ", self.theta, ", phi: ", self.phi)
        eta = (self.theta < 0) and (etaOut / etaIn) or (etaIn / etaOut)
        cosTheta = cos(self.theta)
        if abs(eta * cosTheta) <= 1:
            theta = acos(eta * cosTheta)
            if (self.theta >= 0):
                theta = -theta
            return SphericalCoords(theta, self.phi + math.pi, self.radius)
        else:
            # total internal reflection
            return self.reflect()

def sphereParametrization():
    drawWorldCoordinates()
    
    sphere(radius=1.0, opacity=0.1, color=visual.color.white)
    newRadius = 3.0
    sphere(radius=newRadius, opacity=0.1, color=visual.color.green)

    thetaSampleCount = 10
    phiSampleCount = 2 * thetaSampleCount
    thetaStep = 1.0 / float(thetaSampleCount)
    phiStep = 1.0 / float(phiSampleCount)
    for i in range(0, thetaSampleCount + 1):
        for j in range(0, phiSampleCount + 1):
            spherical = SphericalCoords(
                theta = (i * thetaStep - 0.5) * math.pi,
                phi = j * phiStep * 2 * math.pi)
            color = visual.color.hsv_to_rgb((j * phiStep, i * thetaStep, 1.0))
            changedSpherical = spherical.changeRadius(newRadius, 0.0)
            points(pos=[spherical.toCartesian()], color=color)
            points(pos=[changedSpherical.toCartesian()], color=color)

if __name__ == "__main__":
    from visual import *
    from frame import *
    sphereParametrization()
