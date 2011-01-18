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
        return currentFrameIndex;
    }
 
protected:
    virtual void createScene(void);
    virtual void createCamera(void);
	virtual bool frameRenderingQueued(const Ogre::FrameEvent& evt);
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
    unsigned int currentFrameIndex;

    Ogre::Real totalTime;
    Ogre::Vector3 mLastCameraOffset;
};

class DoFAccumListener: public Ogre::CompositorInstance::Listener
{
public:
    DoFAccumListener(const MovingAverage& app)
    : movingAverageApp(app) {
        Ogre::CompositorInstance::Listener();
    }
    virtual ~DoFAccumListener() {}
    virtual void notifyMaterialSetup(Ogre::uint32 pass_id, Ogre::MaterialPtr &mat);
    virtual void notifyMaterialRender(Ogre::uint32 pass_id, Ogre::MaterialPtr &mat);
protected:
    Ogre::GpuProgramParametersSharedPtr fpParams;
    const MovingAverage& movingAverageApp;
};


#endif // #ifndef __MovingAverage_h_
