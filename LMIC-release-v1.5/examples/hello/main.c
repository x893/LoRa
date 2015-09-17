/*******************************************************************************
 * Copyright (c) 2014-2015 IBM Corporation.
 * All rights reserved. This program and the accompanying materials
 * are made available under the terms of the Eclipse Public License v1.0
 * which accompanies this distribution, and is available at
 * http://www.eclipse.org/legal/epl-v10.html
 *
 * Contributors:
 *	IBM Zurich Research Lab - initial API, implementation and documentation
 *******************************************************************************/

#include "lmic.h"
#include "debug.h"

// LMIC application callbacks not used in his example
void os_getArtEui (uint8_t* buf) {
}

void os_getDevEui (uint8_t* buf) {
}

void os_getDevKey (uint8_t* buf) {
}

void onEvent (ev_t ev) {
}

// counter
static int cnt = 0;

// log text to USART and toggle LED
static void initfunc (osjob_t* job)
{
	debug_str("Hello World!\r\n");	// say hello
	debug_val("cnt = ", cnt);		// log counter
	debug_led(++cnt & 1);			// toggle LED
									// reschedule job every second
	os_setTimedCallback(job, os_getTime()+sec2osticks(1), initfunc);
}

// application entry point
int main ()
{
	osjob_t initjob;

	os_init();		// initialize runtime env
	debug_init();	// initialize debug library
					// setup initial job
	os_setCallback(&initjob, initfunc);
	os_runloop();	// execute scheduled jobs and events
	// (not reached)
	return 0;
}
