/*
-----------------------------------------------------------------------------
Filename:    FullscreenQuad.h
-----------------------------------------------------------------------------

This source file is part of the
   ___                 __    __ _ _    _ 
  /___\__ _ _ __ ___  / / /\ \ (_) | _(_)
 //  // _` | '__/ _ \ \ \/  \/ / | |/ / |
/ \_// (_| | | |  __/  \  /\  /| |   <| |
\___/ \__, |_|  \___|   \/  \/ |_|_|\_\_|
      |___/                              
      Tutorial Framework
      http://www.ogre3d.org/tikiwiki/
-----------------------------------------------------------------------------
*/
#ifndef __FullscreenQuad_h_
#define __FullscreenQuad_h_

#include "stdafx.h"

#include "BaseApplication.h"

class FullscreenQuad : public BaseApplication
{
public:
    FullscreenQuad(void);
    virtual ~FullscreenQuad(void);

protected:
    virtual void createScene(void);
    virtual bool FullscreenQuad::frameRenderingQueued(const Ogre::FrameEvent& evt);
private:
    Ogre::Rectangle2D* mScreenEnt;
    Ogre::ColourValue colorParameter;
    static Ogre::Vector4 colorToVector4(const Ogre::ColourValue& color);
};

#endif // #ifndef __FullscreenQuad_h_
