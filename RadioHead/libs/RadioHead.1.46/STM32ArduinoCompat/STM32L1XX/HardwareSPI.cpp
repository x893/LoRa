// ArduinoCompat/HardwareSPI.cpp
//
// Interface between Arduino-like SPI interface and STM32F4 Discovery and similar
// using STM32F4xx_DSP_StdPeriph_Lib_V1.3.0

#include "RadioHead.h"
#include "stm32l1xx_spi.h"

extern "C"
{
	// #include "gdb_stdio.h"
}

// Symbolic definitions for the SPI pins we intend to use
// Currently we only support SPI1
#define SPIx                           SPI1
#define SPIx_CLK                       RCC_APB2ENR_SPI1EN
#define SPIx_CLK_INIT                  RCC_APB2PeriphClockCmd
#define SPIx_IRQn                      SPI1_IRQn
#define SPIx_IRQHANDLER                SPI1_IRQHandler

#define SPIx_SCK_PIN                   GPIO_Pin_3
#define SPIx_SCK_GPIO_PORT             GPIOB
#define SPIx_SCK_GPIO_CLK              RCC_AHBPeriph_GPIOB
#define SPIx_SCK_SOURCE                GPIO_PinSource3
#define SPIx_SCK_AF                    GPIO_AF_SPI1

#define SPIx_MISO_PIN                  GPIO_Pin_4
#define SPIx_MISO_GPIO_PORT            GPIOB
#define SPIx_MISO_GPIO_CLK             RCC_AHBPeriph_GPIOB
#define SPIx_MISO_SOURCE               GPIO_PinSource4
#define SPIx_MISO_AF                   GPIO_AF_SPI1

#define SPIx_MOSI_PIN                  GPIO_Pin_5
#define SPIx_MOSI_GPIO_PORT            GPIOB
#define SPIx_MOSI_GPIO_CLK             RCC_AHBPeriph_GPIOB
#define SPIx_MOSI_SOURCE               GPIO_PinSource5
#define SPIx_MOSI_AF                   GPIO_AF_SPI1

HardwareSPI::HardwareSPI(uint32_t spiPortNumber) :
    _spiPortNumber(spiPortNumber)
{
}

void HardwareSPI::begin(SPIFrequency frequency, uint32_t bitOrder, uint32_t mode)
{
	GPIO_InitTypeDef GPIO_InitStructure;
//	NVIC_InitTypeDef NVIC_InitStructure;
	SPI_InitTypeDef  SPI_InitStructure;

	/* Peripheral Clock Enable -------------------------------------------------*/
	/* Enable the SPI clock */
	RCC_APB2PeriphClockCmd(SPIx_CLK, ENABLE);
  
	/* Enable GPIO clocks */
	RCC_AHBPeriphClockCmd(SPIx_SCK_GPIO_CLK | SPIx_MISO_GPIO_CLK | SPIx_MOSI_GPIO_CLK, ENABLE);

	/* SPI GPIO Configuration --------------------------------------------------*/
	/* GPIO Deinitialisation */
//	GPIO_DeInit(SPIx_SCK_GPIO_PORT);
//	GPIO_DeInit(SPIx_MISO_GPIO_PORT);
//	GPIO_DeInit(SPIx_MOSI_GPIO_PORT);

	/* Connect SPI pins to AF1 */
	GPIO_PinAFConfig(SPIx_SCK_GPIO_PORT, SPIx_SCK_SOURCE, SPIx_SCK_AF);
	GPIO_PinAFConfig(SPIx_MISO_GPIO_PORT, SPIx_MISO_SOURCE, SPIx_MISO_AF);    
	GPIO_PinAFConfig(SPIx_MOSI_GPIO_PORT, SPIx_MOSI_SOURCE, SPIx_MOSI_AF);

	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF;
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_40MHz;
	GPIO_InitStructure.GPIO_OType = GPIO_OType_PP;
	GPIO_InitStructure.GPIO_PuPd  = GPIO_PuPd_DOWN;

	/* SPI SCK pin configuration */
	GPIO_InitStructure.GPIO_Pin = SPIx_SCK_PIN;
	GPIO_Init(SPIx_SCK_GPIO_PORT, &GPIO_InitStructure);
  
	/* SPI  MISO pin configuration */
	GPIO_InitStructure.GPIO_Pin =  SPIx_MISO_PIN;
	GPIO_Init(SPIx_MISO_GPIO_PORT, &GPIO_InitStructure);  

	/* SPI  MOSI pin configuration */
	GPIO_InitStructure.GPIO_Pin =  SPIx_MOSI_PIN;
	GPIO_Init(SPIx_MOSI_GPIO_PORT, &GPIO_InitStructure);
 
	/* SPI configuration -------------------------------------------------------*/
	SPI_I2S_DeInit(SPIx);
	SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex;
	SPI_InitStructure.SPI_DataSize = SPI_DataSize_8b;

	if (mode == SPI_MODE0)
	{
		SPI_InitStructure.SPI_CPOL = SPI_CPOL_Low;
		SPI_InitStructure.SPI_CPHA = SPI_CPHA_1Edge;
	}
	else if (mode == SPI_MODE1)
	{
		SPI_InitStructure.SPI_CPOL = SPI_CPOL_Low;
		SPI_InitStructure.SPI_CPHA = SPI_CPHA_2Edge;
	}
	else if (mode == SPI_MODE2)
	{
		SPI_InitStructure.SPI_CPOL = SPI_CPOL_High;
		SPI_InitStructure.SPI_CPHA = SPI_CPHA_1Edge;
	}
	else if (mode == SPI_MODE3)
	{
		SPI_InitStructure.SPI_CPOL = SPI_CPOL_Low;
		SPI_InitStructure.SPI_CPHA = SPI_CPHA_2Edge;
	}

	SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;

	// Prescaler is divided into PCLK2 (32 MHz) to get SPI baud rate/clock speed
	// 256 => 125 kHz
	// 128 => 250 kHz
	// 64 => 500 kHz
	// 32 => 1.0 MHz
	// 16 => 2.0 MHz
	// 8  => 4.0 MHz
	// 4  => 8.0 MHz
	switch (frequency)
	{
	case SPI_16_MHZ:
		SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_2;
		break;
	case SPI_8_MHZ:
		SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_4;
		break;
	case SPI_4_MHZ:
		SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_8;
		break;
	case SPI_2_MHZ:
		SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_16;
		break;
	case SPI_1_MHZ:
	default:
		SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_32;
		break;
	case SPI_500_KHZ:
		SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_64;
		break;
	case SPI_250_KHZ:
		SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_128;
		break;
	case SPI_125_KHZ:
		SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_256;
		break;
	}

	if (bitOrder == LSBFIRST)
		SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_LSB;
	else
		SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;

	SPI_InitStructure.SPI_CRCPolynomial = 7;
	SPI_InitStructure.SPI_Mode = SPI_Mode_Master;

	SPI_Init(SPIx, &SPI_InitStructure);
	SPI_Cmd(SPIx, ENABLE);
}

void HardwareSPI::end(void)
{
    SPI_DeInit(SPIx);
}

uint8_t HardwareSPI::transfer(uint8_t data)
{
    // Wait for TX empty
    while (SPI_I2S_GetFlagStatus(SPIx, SPI_I2S_FLAG_TXE) == RESET)
		;
    SPI_SendData(SPIx, data);
    // Wait for RX not empty
    while (SPI_I2S_GetFlagStatus(SPIx, SPI_I2S_FLAG_RXNE) == RESET)
		;
    return SPI_ReceiveData(SPIx);
}
