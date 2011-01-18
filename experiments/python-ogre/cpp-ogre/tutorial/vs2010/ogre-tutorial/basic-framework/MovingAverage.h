#ifndef __MovingAverage_h_
#define __MovingAverage_h_

#include "stdafx.h"

#include "BaseApplication.h"
 
class MovingAverage : public BaseApplication, public Ogre::RenderTargetListener
{
public:
    MovingAverage(void);
    virtual ~MovingAverage(void);
 
protected:
    virtual void createScene(void);
    virtual void createCamera(void);
	virtual bool frameRenderingQueued(const Ogre::FrameEvent& evt);
 
    void setupCompositors(void);

	Ogre::MovablePlane* mPlane;
	Ogre::Entity* mPlaneEnt;
	Ogre::SceneNode* mPlaneNode;
 
    Ogre::Real totalTime;
    Ogre::Vector3 mLastCameraOffset;
};
 
#endif // #ifndef __MovingAverage_h_
