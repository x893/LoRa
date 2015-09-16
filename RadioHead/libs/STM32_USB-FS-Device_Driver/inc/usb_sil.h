/**
  ******************************************************************************
  * @file    usb_sil.h
  * @author  MCD Application Team
  * @version V4.0.0
  * @date    28-August-2012
  * @brief   Simplified Interface Layer function prototypes.
  ******************************************************************************
  */
#ifndef __USB_SIL_H
#define __USB_SIL_H

#include <stdint.h>

uint32_t USB_SIL_Init(void);
uint32_t USB_SIL_Write(uint8_t bEpAddr, uint8_t * src, uint32_t length);
uint32_t USB_SIL_Read(uint8_t bEpAddr, uint8_t * dst);

#endif /* __USB_SIL_H */
