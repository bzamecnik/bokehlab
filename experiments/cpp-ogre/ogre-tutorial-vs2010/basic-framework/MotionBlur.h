#ifndef __MotionBlur_h_
#define __MotionBlur_h_

#include "stdafx.h"

#include "BaseApplication.h"
 
class MotionBlur : public BaseApplication, public Ogre::RenderTargetListener
{
public:
    MotionBlur(void);
    virtual ~MotionBlur(void);
 
protected:
    virtual void createScene(void);
	virtual bool frameRenderingQueued(const Ogre::FrameEvent& evt);
 
	Ogre::MovablePlane* mPlane;
	Ogre::Entity* mPlaneEnt;
	Ogre::SceneNode* mPlaneNode;
 
    void setupCompositors(void);
};
 
#endif // #ifndef __MotionBlur_h_
