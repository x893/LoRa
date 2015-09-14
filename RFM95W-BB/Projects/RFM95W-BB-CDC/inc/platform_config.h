#ifndef __PLATFORM_CONFIG_H
#define __PLATFORM_CONFIG_H

#if defined(STM32L1XX_MD) || defined(STM32L1XX_HD)|| defined(STM32L1XX_MD_PLUS)

	#include "stm32l1xx.h"

	#define		 ID1			(0x1FF80050)
	#define		 ID2			(0x1FF80054)
	#define		 ID3			(0x1FF80064)

	#if defined (USE_RFM95W_BB)
		#include "rfm95w_bb.h"
	#else
		#error "Missing board definition"
	#endif

#else
	#error "Missing CPU definition"
#endif

#endif /* __PLATFORM_CONFIG_H */
