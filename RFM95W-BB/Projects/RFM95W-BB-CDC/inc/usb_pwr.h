/**
 ******************************************************************************
 * @file    usb_pwr.h
 * @author  MCD Application Team
 * @version V4.0.0
 * @date    21-January-2013
 * @brief   Connection/disconnection & power management header
 ******************************************************************************
 */
#ifndef __USB_PWR_H
#define __USB_PWR_H

#include <stdbool.h>
#include "usb_core.h"

typedef enum _RESUME_STATE {
	RESUME_EXTERNAL,
	RESUME_INTERNAL,
	RESUME_LATER,
	RESUME_WAIT,
	RESUME_START,
	RESUME_ON,
	RESUME_OFF,
	RESUME_ESOF
} RESUME_STATE;

void Suspend(void);
void Resume_Init(void);
void Resume(RESUME_STATE eResumeSetVal);
USB_RESULT PowerOn(void);
USB_RESULT PowerOff(void);

#endif  /*__USB_PWR_H*/
