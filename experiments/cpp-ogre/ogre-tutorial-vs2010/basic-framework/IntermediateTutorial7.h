#ifndef __IntermediateTutorial7_h_
#define __IntermediateTutorial7_h_

#include "stdafx.h"

#include "BaseApplication.h"
 
class IntermediateTutorial7 : public BaseApplication, public Ogre::RenderTargetListener
{
public:
    IntermediateTutorial7(void);
    virtual ~IntermediateTutorial7(void);
 
protected:
    virtual void createScene(void);
	virtual bool frameRenderingQueued(const Ogre::FrameEvent& evt);
	virtual void preRenderTargetUpdate(const Ogre::RenderTargetEvent& evt);
	virtual void postRenderTargetUpdate(const Ogre::RenderTargetEvent& evt);
 
	Ogre::MovablePlane* mPlane;
	Ogre::Entity* mPlaneEnt;
	Ogre::SceneNode* mPlaneNode;
 
        //This should be taken out of the createScene member and brought here
	Ogre::Rectangle2D* mMiniScreen;
};
 
#endif // #ifndef __IntermediateTutorial7_h_
