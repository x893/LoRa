#include "Print.h"

void Print::print(const char* s)
{
	while (*s != 0)
		write(*s++);
}

void Print::println(const char* s)
{
	print(s);
	print("\r\n");
}

void Print::print(char ch)
{
	write(ch);
}

void Print::println(char ch)
{
	write(ch);
	print("\r\n");
}

void Print::print(int32_t n)
{
	if (n < 0)
	{
		write('-');
		n = -n;
	}
	print((uint32_t)n, DEC);
}

void Print::print(uint32_t n, uint8_t base)
{
	char buf[20];
    int i = 0;

	if (n == 0)
		buf[i++] = 0;
	else
		while (n > 0)
		{
			buf[i++] = n % base;
			n /= base;
		}

	for (; i > 0; i--)
	{
		print((char)(
					buf[i - 1] < 10 ?
					buf[i - 1] + '0' :
					buf[i - 1] + ('A' - 10))
			);
	}
}

void Print::print(uint8_t ch, uint8_t base)
{
	print((uint32_t)ch, base);
}

void Print::println(uint8_t ch, uint8_t base)
{
	print((uint32_t)ch, base);
	print("\r\n");
}
