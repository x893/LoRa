#include "rfm95w_bb.h"
#include "stm32l1xx_spi.h"

GPIO_TypeDef* GPIO_PORT[LEDn]	= {LED1_GPIO_PORT, LED2_GPIO_PORT, LED3_GPIO_PORT};
const uint16_t GPIO_PIN[LEDn]	= {LED1_PIN, LED2_PIN, LED3_PIN};
const uint32_t GPIO_CLK[LEDn]	= {LED1_GPIO_CLK, LED2_GPIO_CLK, LED3_GPIO_CLK};

/**
  * @brief  Configures LED GPIO.
  * @param  Led: Specifies the Led to be configured. 
  *   This parameter can be one of following parameters:
  *     @arg LED1
  *     @arg LED2
  *     @arg LED3
  * @retval None
  */
void STM_EVAL_LEDInit(Led_TypeDef Led)
{
	GPIO_InitTypeDef  GPIO_InitStructure;

	/* Enable the GPIO_LED Clock */
	RCC_AHBPeriphClockCmd(GPIO_CLK[Led], ENABLE);

	STM_EVAL_LEDOff(Led);	// Turn Off initially

	/* Configure the GPIO_LED pin */
	GPIO_InitStructure.GPIO_Pin = GPIO_PIN[Led];
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_400KHz;
	GPIO_Init(GPIO_PORT[Led], &GPIO_InitStructure);
}

/**
  * @brief  Turns selected LED On.
  * @param  Led: Specifies the Led to be set on. 
  *   This parameter can be one of following parameters:
  *     @arg LED1
  *     @arg LED2
  *     @arg LED3
  * @retval None
  */
void STM_EVAL_LEDOn(Led_TypeDef Led)
{
	GPIO_PORT[Led]->BSRRL = GPIO_PIN[Led];
}

/**
  * @brief  Turns selected LED Off.
  * @param  Led: Specifies the Led to be set off. 
  *   This parameter can be one of following parameters:
  *     @arg LED1
  *     @arg LED2
  *     @arg LED3
  * @retval None
  */
void STM_EVAL_LEDOff(Led_TypeDef Led)
{
	GPIO_PORT[Led]->BSRRH = GPIO_PIN[Led];  
}

/**
  * @brief  Toggles the selected LED.
  * @param  Led: Specifies the Led to be toggled. 
  *   This parameter can be one of following parameters:
  *     @arg LED1
  *     @arg LED2
  *     @arg LED3
  * @retval None
  */
void STM_EVAL_LEDToggle(Led_TypeDef Led)
{
	GPIO_PORT[Led]->ODR ^= GPIO_PIN[Led];
}

/**
  * @brief  Configures COM port.
  * @param  COM: Specifies the COM port to be configured.
  *   This parameter can be one of following parameters:    
  *     @arg COM1
  *     @arg COM2  
  * @param  USART_InitStruct: pointer to a USART_InitTypeDef structure that
  *   contains the configuration information for the specified USART peripheral.
  * @retval None
  */
void STM_EVAL_COMInit(COM_TypeDef COM, USART_InitTypeDef* USART_InitStruct)
{
	GPIO_InitTypeDef GPIO_InitStructure;

	/* Enable GPIO clock */
	RCC_AHBPeriphClockCmd(	0
							| RF95W_COM_TX_GPIO_CLK
							| RF95W_COM_RX_GPIO_CLK
							, ENABLE);

	/* Enable UART clock */
	RCC_APB1PeriphClockCmd(RF95W_COM_TX_GPIO_CLK | RF95W_COM_RX_GPIO_CLK, ENABLE);

	/* Connect PXx to USARTx_Tx */
	GPIO_PinAFConfig(RF95W_COM_TX_GPIO_PORT, RF95W_COM_TX_SOURCE, RF95W_COM_PIN_AF);

	/* Connect PXx to USARTx_Rx */
	GPIO_PinAFConfig(RF95W_COM_RX_GPIO_PORT, RF95W_COM_RX_SOURCE, RF95W_COM_PIN_AF);

	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_40MHz;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;

	/* Configure USART Tx as alternate function push-pull */
	GPIO_InitStructure.GPIO_Pin = RF95W_COM_TX_PIN;
	GPIO_Init(RF95W_COM_TX_GPIO_PORT, &GPIO_InitStructure);

	/* Configure USART Rx as alternate function push-pull */
	GPIO_InitStructure.GPIO_Pin = RF95W_COM_RX_PIN;
	GPIO_Init(RF95W_COM_RX_GPIO_PORT, &GPIO_InitStructure);

	/* USART configuration */
	USART_Init(EVAL_COM1, USART_InitStruct);

	/* Enable USART */
	USART_Cmd(EVAL_COM1, ENABLE);
}

/**
  * @brief  DeInitializes the SPI interface.
  * @param  None
  * @retval None
  */
void RF95W_LowLevel_DeInit(void)
{
	GPIO_InitTypeDef  GPIO_InitStructure;

	SPI_Cmd(RF95W_SPI, DISABLE);	/*!< SD_SPI disable */
	SPI_DeInit(RF95W_SPI);			/*!< DeInitializes the SD_SPI */

	/*!< SD_SPI Periph clock disable */
	RCC_APB1PeriphClockCmd(RF95W_SPI_CLK, DISABLE); 

	/*!< Configure SD_SPI pins: SCK */
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_NOPULL;

	GPIO_InitStructure.GPIO_Pin = RF95W_SCK_PIN;
	GPIO_Init(RF95W_SCK_GPIO_PORT, &GPIO_InitStructure);

	/*!< Configure SD_SPI pins: MISO */
	GPIO_InitStructure.GPIO_Pin = RF95W_MISO_PIN;
	GPIO_Init(RF95W_MISO_GPIO_PORT, &GPIO_InitStructure);

	/*!< Configure SD_SPI pins: MOSI */
	GPIO_InitStructure.GPIO_Pin = RF95W_MOSI_PIN;
	GPIO_Init(RF95W_MOSI_GPIO_PORT, &GPIO_InitStructure);

	/*!< Configure RF95W_CS_PIN pin: SD Card CS pin */
	GPIO_InitStructure.GPIO_Pin = RF95W_CS_PIN;
	GPIO_Init(RF95W_CS_GPIO_PORT, &GPIO_InitStructure);

	/*!< Configure RF95W_DIO0_PIN pin: SD Card detect pin */
	GPIO_InitStructure.GPIO_Pin = RF95W_DIO0_PIN;
	GPIO_Init(RF95W_DIO0_GPIO_PORT, &GPIO_InitStructure);
}

/**
  * @brief  Initializes the SD Card and put it into StandBy State (Ready for 
  *         data transfer).
  * @param  None
  * @retval None
  */
void RF95W_LowLevel_Init(void)
{
	GPIO_InitTypeDef  GPIO_InitStructure;
	SPI_InitTypeDef   SPI_InitStructure;

	RCC_AHBPeriphClockCmd(	0
							| RF95W_CS_GPIO_CLK
							| RF95W_MOSI_GPIO_CLK
							| RF95W_MISO_GPIO_CLK
							| RF95W_SCK_GPIO_CLK
							| RF95W_DIO0_GPIO_CLK
							, ENABLE);

	/*!< SD_SPI Periph clock enable */
	RCC_APB1PeriphClockCmd(RF95W_SPI_CLK, ENABLE); 

	/*!< Configure SD_SPI pins: SCK */
	GPIO_InitStructure.GPIO_Pin = RF95W_SCK_PIN;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_40MHz;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_PuPd  = GPIO_PuPd_UP;
	GPIO_Init(RF95W_SCK_GPIO_PORT, &GPIO_InitStructure);

	/*!< Configure SD_SPI pins: MISO */
	GPIO_InitStructure.GPIO_Pin = RF95W_MISO_PIN;
	GPIO_Init(RF95W_MISO_GPIO_PORT, &GPIO_InitStructure);

	/*!< Configure SD_SPI pins: MOSI */
	GPIO_InitStructure.GPIO_Pin = RF95W_MOSI_PIN;
	GPIO_Init(RF95W_MOSI_GPIO_PORT, &GPIO_InitStructure);

	/*!< Configure RF95W_CS_PIN pin: SD Card CS pin */
	GPIO_InitStructure.GPIO_Pin = RF95W_CS_PIN;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_OUT;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_40MHz;
	GPIO_Init(RF95W_CS_GPIO_PORT, &GPIO_InitStructure);

	/*!< Configure RF95W_DIO0_PIN pin: SD Card detect pin */
	GPIO_InitStructure.GPIO_Pin = RF95W_DIO0_PIN;
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN;
	GPIO_InitStructure.GPIO_PuPd = GPIO_PuPd_UP;
	GPIO_Init(RF95W_DIO0_GPIO_PORT, &GPIO_InitStructure);

	/* Connect PXx to RF95W_SCK */
	GPIO_PinAFConfig(RF95W_SCK_GPIO_PORT, RF95W_SCK_SOURCE, RF95W_PIN_AF);
	/* Connect PXx to RF95W_MISO */
	GPIO_PinAFConfig(RF95W_MISO_GPIO_PORT, RF95W_MISO_SOURCE, RF95W_PIN_AF); 
	/* Connect PXx to RF95W_MOSI */
	GPIO_PinAFConfig(RF95W_MOSI_GPIO_PORT, RF95W_MOSI_SOURCE, RF95W_PIN_AF);

	/*!< SD_SPI Config */
	SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex;
	SPI_InitStructure.SPI_Mode = SPI_Mode_Master;
	SPI_InitStructure.SPI_DataSize = SPI_DataSize_8b;
	SPI_InitStructure.SPI_CPOL = SPI_CPOL_High;
	SPI_InitStructure.SPI_CPHA = SPI_CPHA_2Edge;
	SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;
	SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_4;
	SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;
	SPI_InitStructure.SPI_CRCPolynomial = 7;
	SPI_Init(RF95W_SPI, &SPI_InitStructure);

	SPI_Cmd(RF95W_SPI, ENABLE);
}
