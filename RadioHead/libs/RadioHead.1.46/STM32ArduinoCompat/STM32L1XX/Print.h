#ifndef __PRINT_H__
#define __PRINT_H__

#include <stdio.h>
#include <stdint.h>

class Print
{
public:
#define DEC 10
#define HEX 16
#define OCT 8
#define BIN 2
	virtual size_t write(uint8_t) = NULL;
	size_t print(const char* s);
	size_t println(const char* s);
	size_t print(char ch);
	size_t println(char ch);
	size_t print(uint32_t n, uint8_t base = DEC);
	size_t print(uint8_t ch, uint8_t base = DEC);
	size_t println(uint8_t ch, uint8_t base = DEC);
};

#endif
