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

	size_t print(const char* s)
	{
		printf(s);
		return 0;
	}

	size_t println(const char* s)
	{
		print(s);
		printf("\n");
		return 0;
	}

	size_t print(unsigned int n, uint8_t base = DEC)
	{
		if (base == DEC)
			printf("%d", n);
		else if (base == HEX)
			printf("%02x", n);
		else if (base == OCT)
			printf("%o", n);
		// TODO: BIN
		return 0;
	}

	size_t print(char ch)
	{
		printf("%c", ch);
		return 0;
	}

	size_t println(char ch)
	{
		printf("%c\n", ch);
		return 0;
	}

	size_t print(unsigned char ch, uint8_t base = DEC)
	{
		return print((unsigned int)ch, base);
	}

	size_t println(unsigned char ch, uint8_t base = DEC)
	{
		print((unsigned int)ch, base);
		printf("\n");
		return 0;
	}
};

#endif
