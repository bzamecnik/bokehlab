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
    def __init__(self, theta=0.0, phi=0.0):
        self.theta = theta
        self.phi = phi

    def fromCartesian(self, cartesian, normalized=True):
        x = cartesian[0]
        y = cartesian[1]
        z = cartesian[2]
        rInv = normalized and 1.0 or 1.0 / norm((x, y, z))
        theta = asin(z * rInv)
        phi = atan2(y, x)
        return SphericalCoords(theta, phi)

    def toCartesian(self):
        cosTheta = cos(self.theta)
        x = cosTheta * cos(self.phi)
        y = cosTheta * sin(self.phi)
        z = sin(self.theta)
        return (x, y, z)

    # Compute reflection of an incoming direction represented by this spherical
    # coordinates which gets reflected by the supporting plane.
    def reflect(self):
        return SphericalCoords(self.theta, self.phi + math.pi)

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
            return SphericalCoords(theta, self.phi + math.pi)
        else:
            # total internal reflection
            return self.reflect()
