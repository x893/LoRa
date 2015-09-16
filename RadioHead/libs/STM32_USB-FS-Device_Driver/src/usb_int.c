/**
  ******************************************************************************
  * @file    usb_int.c
  * @author  MCD Application Team
  * @version V4.0.0
  * @date    28-August-2012
  * @brief   Endpoint CTR (Low and High) interrupt's service routines
  ******************************************************************************
  */
#include "usb_lib.h"

extern __IO uint16_t wIstr;			/* ISTR register last read value */
extern void (*pEpInt_IN[7])(void);	/*  Handles IN  interrupts   */
extern void (*pEpInt_OUT[7])(void);	/*  Handles OUT interrupts   */

/*******************************************************************************
* Function Name  : CTR_LP.
* Description    : Low priority Endpoint Correct Transfer interrupt's service
*                  routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CTR_LP(void)
{
	__IO uint16_t wEPVal;

	/* stay in loop while pending interrupts */
	while (((wIstr = _GetISTR()) & ISTR_CTR) != 0)
	{
		/* extract highest priority endpoint number */
		pInformation->EPindex = (uint8_t)(wIstr & ISTR_EP_ID);
		if (pInformation->EPindex == 0)
		{
			/* Decode and service control endpoint interrupt */
			/* calling related service routine */
			/* (Setup0_Process, In0_Process, Out0_Process) */

			/* save RX & TX status */
			/* and set both to NAK */
      
			pInformation->SaveRState = _GetENDPOINT(ENDP0);
			pInformation->SaveTState = pInformation->SaveRState & EPTX_STAT;
			pInformation->SaveRState &=  EPRX_STAT;	

			_SetEPRxTxStatus(ENDP0,EP_RX_NAK,EP_TX_NAK);

			/* DIR bit = origin of the interrupt */
			if ((wIstr & ISTR_DIR) == 0)
			{
				/* DIR = 0 */

				/* DIR = 0      => IN  int */
				/* DIR = 0 implies that (EP_CTR_TX = 1) always  */

				_ClearEP_CTR_TX(ENDP0);
				In0_Process();

				/* before terminate set Tx & Rx status */
				_SetEPRxTxStatus(ENDP0, pInformation->SaveRState, pInformation->SaveTState);
				return;
			}
			else
			{
				/* DIR = 1 */

				/* DIR = 1 & CTR_RX       => SETUP or OUT int */
				/* DIR = 1 & (CTR_TX | CTR_RX) => 2 int pending */

				wEPVal = _GetENDPOINT(ENDP0);

				if ((wEPVal &EP_SETUP) != 0)
				{
					_ClearEP_CTR_RX(ENDP0); /* SETUP bit kept frozen while CTR_RX = 1 */
					Setup0_Process();
					/* before terminate set Tx & Rx status */

					_SetEPRxTxStatus(ENDP0, pInformation->SaveRState, pInformation->SaveTState);
					return;
				}
				else if ((wEPVal & EP_CTR_RX) != 0)
				{
					_ClearEP_CTR_RX(ENDP0);
					Out0_Process();
					/* before terminate set Tx & Rx status */

					_SetEPRxTxStatus(ENDP0, pInformation->SaveRState, pInformation->SaveTState);
					return;
				}
			}
		}	/* if(EPindex == 0) */
		else
		{
			/* Decode and service non control endpoints interrupt  */
			/* process related endpoint register */
			wEPVal = _GetENDPOINT(pInformation->EPindex);
			if ((wEPVal & EP_CTR_RX) != 0)
			{
				/* clear int flag */
				_ClearEP_CTR_RX(pInformation->EPindex);

				/* call OUT service function */
				(*pEpInt_OUT[pInformation->EPindex-1])();
			}	/* if((wEPVal & EP_CTR_RX) */

			if ((wEPVal & EP_CTR_TX) != 0)
			{
				/* clear int flag */
				_ClearEP_CTR_TX(pInformation->EPindex);

				/* call IN service function */
				(*pEpInt_IN[pInformation->EPindex-1])();
			} /* if((wEPVal & EP_CTR_TX) != 0) */
		} /* if(EPindex == 0) else */
	} /* while(...) */
}

/*******************************************************************************
* Function Name  : CTR_HP.
* Description    : High Priority Endpoint Correct Transfer interrupt's service 
*                  routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CTR_HP(void)
{
	uint32_t wEPVal;

	while (((wIstr = _GetISTR()) & ISTR_CTR) != 0)
	{
		_SetISTR((uint16_t)CLR_CTR); /* clear CTR flag */
		/* extract highest priority endpoint number */
		pInformation->EPindex = (uint8_t)(wIstr & ISTR_EP_ID);
		/* process related endpoint register */
		wEPVal = _GetENDPOINT(pInformation->EPindex);
		if ((wEPVal & EP_CTR_RX) != 0)
		{	/* clear int flag */
			_ClearEP_CTR_RX(pInformation->EPindex);

			/* call OUT service function */
			(*pEpInt_OUT[pInformation->EPindex-1])();
		} /* if((wEPVal & EP_CTR_RX) */
		else if ((wEPVal & EP_CTR_TX) != 0)
		{	/* clear int flag */
			_ClearEP_CTR_TX(pInformation->EPindex);

			/* call IN service function */
			(*pEpInt_IN[pInformation->EPindex-1])();
		} /* if((wEPVal & EP_CTR_TX) != 0) */
	} /* while(...) */
}
