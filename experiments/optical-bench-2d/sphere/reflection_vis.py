from visual import *
from visual.controls import *
from spherical import SphericalCoords
from frame import *


def updateArrow(incomingArrow, reflectedArrow, spherical, theta=None, phi=None):
    if theta != None:
        spherical.theta = theta * math.pi
    if phi != None:
        spherical.phi = phi * 2 * math.pi
    incomingDir = vector(spherical.toCartesian())
    incomingArrow.pos = incomingDir
    incomingArrow.axis = -vector(incomingDir)
    reflectedArrow.axis = spherical.reflect().toCartesian()

w = 500
display(x=w, y=0, width=w, height=w, range=1.5, forward=-vector(0,1,1), newzoom=1)
controls = controls(0, 0, width=w, height=w)
incomingThetaSlider = slider(pos=(-10,-50), width=7, length=50, axis=(0,1,0),
    action=lambda: updateArrow(incomingArrow, reflectedArrow, incoming,
        theta=incomingThetaSlider.value / 100.0))
incomingPhiSlider = slider(pos=(10,-50), width=7, length=50, axis=(0,1,0),
    action=lambda: updateArrow(incomingArrow, reflectedArrow, incoming,
        phi=incomingPhiSlider.value / 100.0))

drawWorldCoordinates()
box(pos=(0,0,0), axis=(0,0,1), length=0.1, height=1.0, width=1.0, opacity=0.1)

incoming = SphericalCoords(theta = 0.5 * math.pi, phi = 0.2 * 2 * math.pi)
reflected = incoming.reflect()

incomingDir = incoming.toCartesian()

incomingArrow = drawRayDirection(incomingDir, -vector(incomingDir))
reflectedArrow = drawRayDirection((0,0,0), reflected.toCartesian(), color=visual.color.green)

incomingThetaSlider.value = 100.0 * incoming.theta / math.pi
incomingPhiSlider.value = 100.0 * incoming.phi / (2 * math.pi)

while True:
    rate(100)
    controls.interact() # check for events, drive actions; must be executed repeatedly in a loop
