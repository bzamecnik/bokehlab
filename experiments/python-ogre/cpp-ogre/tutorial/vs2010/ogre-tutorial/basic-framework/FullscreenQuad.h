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
};

#endif // #ifndef __FullscreenQuad_h_
