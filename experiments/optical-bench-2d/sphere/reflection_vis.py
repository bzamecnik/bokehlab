from visual import *
from visual.controls import *
from spherical import SphericalCoords
from frame import *

class TransmissionDemo(object):
    def __init__(self, incoming=SphericalCoords(0,0), etaIn=1.0, etaOut=1.0):
        self.etaIn = etaIn
        self.etaOut = etaOut
        
        self.incoming = incoming
        self.reflected = incoming.reflect()
        self.refracted = incoming.refract(etaIn, etaOut)
        
        self.incomingArrow = drawRayDirection()
        self.reflectedArrow = drawRayDirection(color=visual.color.green)
        self.refractedArrow = drawRayDirection(color=visual.color.orange)

    def update(self, theta=None, phi=None):
        if theta != None:
            self.incoming.theta = theta * math.pi
        if phi != None:
            self.incoming.phi = phi * 2 * math.pi
        self.reflected = self.incoming.reflect()
        self.refracted = self.incoming.refract(self.etaIn, self.etaOut)

        # set incoming arrow so that it point towards the transmission point
        incomingDir = vector(self.incoming.toCartesian())
        self.incomingArrow.pos = incomingDir
        self.incomingArrow.axis = -vector(incomingDir)
        
        self.reflectedArrow.axis = self.reflected.toCartesian()
        
        self.refractedArrow.axis = self.refracted.toCartesian()

w = 500
display(x=w, y=0, width=w, height=w, range=1.5, forward=-vector(0,1,1), newzoom=1)
controls = controls(0, 0, width=w, height=w)
incomingThetaSlider = slider(pos=(-10,-50), width=7, length=100, axis=(0,1,0),
    action=lambda: demo.update(theta=(incomingThetaSlider.value / 100.0)-0.5))
incomingPhiSlider = slider(pos=(10,-50), width=7, length=100, axis=(0,1,0),
    action=lambda: demo.update(phi=incomingPhiSlider.value / 100.0))

drawWorldCoordinates()
box(pos=(0,0,0), axis=(0,0,1), length=0.01, height=1.0, width=1.0, opacity=0.25)
box(pos=(0,0,-0.5), axis=(0,0,1), length=1, height=1.0, width=1.0, opacity=0.1)

demo = TransmissionDemo(SphericalCoords(0.25 * math.pi, 0.2 * 2 * math.pi),
    etaOut=1.5)

incomingThetaSlider.value = 100.0 * ((demo.incoming.theta / math.pi) + 0.5)
incomingPhiSlider.value = 100.0 * demo.incoming.phi / (2 * math.pi)

while True:
    rate(100)
    controls.interact() # check for events, drive actions; must be executed repeatedly in a loop
