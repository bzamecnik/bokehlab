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
	virtual bool frameRenderingQueued(const Ogre::FrameEvent& evt);
 
	Ogre::MovablePlane* mPlane;
	Ogre::Entity* mPlaneEnt;
	Ogre::SceneNode* mPlaneNode;
 
    void setupCompositors(void);
};
 
#endif // #ifndef __MovingAverage_h_
