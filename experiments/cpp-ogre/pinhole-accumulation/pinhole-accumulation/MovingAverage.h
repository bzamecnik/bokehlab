#ifndef __MovingAverage_h_
#define __MovingAverage_h_

#include "stdafx.h"

#include "BaseApplication.h"
 
class MovingAverage : public BaseApplication, public Ogre::RenderTargetListener
{
public:
    MovingAverage(void);
    virtual ~MovingAverage(void);
    unsigned int getCurrentFrameIndex() const {
        return mCurrentFrameIndex;
    }
    Ogre::Vector2 getCurrentFrameOffset() const;
 
protected:
    virtual void createScene(void);
    virtual void createCamera(void);
	virtual bool frameRenderingQueued(const Ogre::FrameEvent& evt);
    //virtual bool frameStarted(const Ogre::FrameEvent& evt);
    // OIS::KeyListener
    virtual bool keyPressed( const OIS::KeyEvent &arg );
    virtual bool keyReleased( const OIS::KeyEvent &arg );
    // OIS::MouseListener
    virtual bool mouseMoved( const OIS::MouseEvent &arg );
    virtual bool mousePressed( const OIS::MouseEvent &arg, OIS::MouseButtonID id );
    virtual bool mouseReleased( const OIS::MouseEvent &arg, OIS::MouseButtonID id );
 
    void setupCompositors(void);
    void uniformSampleDisk(const Ogre::Vector2& randomNumbers, Ogre::Vector2* diskSamples);
    void resetCurrentFrameIndex();

	Ogre::MovablePlane* mPlane;
	Ogre::Entity* mPlaneEnt;
	Ogre::SceneNode* mPlaneNode;
 
    // index of the current frame since the scene and camera didn't change
    unsigned int mCurrentFrameIndex;

    Ogre::Real totalTime;
    Ogre::Vector3 mLastCameraOffset;

    // offset of the image in the near plane due to camera position perturbation
    Ogre::Vector2 mCurrentFrameOffset;

    Ogre::Real mLensRadius;
    Ogre::Real mFocusDistance;
};

#endif // #ifndef __MovingAverage_h_
