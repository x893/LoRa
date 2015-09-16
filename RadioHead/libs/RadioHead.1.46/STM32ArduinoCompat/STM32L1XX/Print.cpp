#include "Print.h"

size_t Print::print(const char* s)
{
	while (*s != 0)
		write(*s++);
	return 0;
}

size_t Print::println(const char* s)
{
	print(s);
	print("\r\n");
	return 0;
}

size_t Print::print(char ch)
{
	write(ch);
	return 0;
}

size_t Print::println(char ch)
{
	write(ch);
	print("\r\n");
	return 0;
}

size_t Print::print(uint32_t n, uint8_t base)
{
	/*
	if (base == DEC)
		printf("%d", n);
	else if (base == HEX)
		printf("%02x", n);
	else if (base == OCT)
		printf("%o", n);
	else if (base == BIN)
		printf("%o", n);
	*/
	return 0;
}

size_t Print::print(uint8_t ch, uint8_t base)
{
	return print((uint32_t)ch, base);
}

size_t Print::println(uint8_t ch, uint8_t base)
{
	print((uint32_t)ch, base);
	print("\r\n");
	return 0;
}
