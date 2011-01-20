#ifndef __DoFAccumListener_h_
#define __DoFAccumListener_h_

#include "stdafx.h"

#include "MovingAverage.h"

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

#endif // #ifndef __DoFAccumListener_h_
