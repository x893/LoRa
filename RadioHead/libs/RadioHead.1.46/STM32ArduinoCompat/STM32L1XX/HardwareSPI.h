// ArduinoCompat/HardwareSPI.h
// STM32 implementattion of Arduino compatible SPI class

#ifndef _HardwareSPI_h
#define _HardwareSPI_h

#include <stdint.h>

typedef enum SPIFrequency {
    SPI_16_MHZ	= 0,
    SPI_8_MHZ,
    SPI_4_MHZ,
    SPI_2_MHZ,
    SPI_1_MHZ,
    SPI_500_KHZ,
    SPI_250_KHZ,
    SPI_125_KHZ
} SPIFrequency;

#define SPI_MODE0 0x00
#define SPI_MODE1 0x04
#define SPI_MODE2 0x08
#define SPI_MODE3 0x0C

class HardwareSPI
{
public:
    HardwareSPI(uint32_t spiPortNumber); // Only port SPI1 is currently supported
    void begin(SPIFrequency frequency, uint32_t bitOrder, uint32_t mode);
    void end(void);
    uint8_t transfer(uint8_t data);

private:
    uint32_t _spiPortNumber; // Not used yet.
};
extern HardwareSPI SPI;

#endif
