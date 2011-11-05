import visual

# Draw axes of a frames as color-coded arrows.
def drawFrameAxes(position, frame):
    visual.arrow(pos=position, axis=frame[0], shaftwidth=0.01,
        color=visual.color.blue)
    visual.arrow(pos=position, axis=frame[1], shaftwidth=0.01)
    visual.arrow(pos=position, axis=frame[2], shaftwidth=0.01,
        color=visual.color.red)

def drawRayDirection(position=(0,0,0), direction=(1,0,0), color=visual.color.yellow):
    return visual.arrow(pos=position, axis=direction, shaftwidth=0.025,
        color=color)

def drawWorldCoordinates():
    drawFrameAxes((0,0,0), ((1,0,0), (0,1,0), (0,0,1)))
