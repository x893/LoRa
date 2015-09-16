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
	virtual void write(uint8_t) = NULL;

	void print(const char* s);
	void println(const char* s);
	void print(char ch);
	void println(char ch);
	void print(uint32_t n, uint8_t base = DEC);
	void print(int32_t n);
	void print(int8_t n) { print((int32_t)n); }
	void print(uint8_t ch, uint8_t base = DEC);
	void println(uint8_t ch, uint8_t base = DEC);
};

#endif
