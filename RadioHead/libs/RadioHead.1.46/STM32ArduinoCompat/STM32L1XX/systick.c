#include <stm32l1xx.h>

volatile uint32_t systick_count = 0;

uint32_t millis(void)
{
	return systick_count;
}

// Called every 1 ms
void SysTick_Handler(void)
{
	systick_count++;
}
