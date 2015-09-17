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

// RUNTIME STATE
static struct {
	osjob_t* scheduledjobs;
	osjob_t* runnablejobs;
} OS;

void os_init ()
{
	memset(&OS, 0x00, sizeof(OS));
	hal_init();
	radio_init();
	LMIC_init();
}

ostime_t os_getTime ()
{
	return hal_ticks();
}

static uint8_t unlinkjob (osjob_t** pnext, osjob_t* job)
{
	for ( ; *pnext; pnext = &((*pnext)->next))
	{
		if (*pnext == job) { // unlink
			*pnext = job->next;
			return 1;
		}
	}
	return 0;
}

// clear scheduled job
void os_clearCallback (osjob_t* job) {
	hal_disableIRQs();
	unlinkjob(&OS.scheduledjobs, job) || unlinkjob(&OS.runnablejobs, job);
	hal_enableIRQs();
}

// schedule immediately runnable job
void os_setCallback (osjob_t* job, osjobcb_t cb)
{
	osjob_t** pnext;
	hal_disableIRQs();

	os_clearCallback(job);	// remove if job was already queued
	job->func = cb;			// fill-in job
	job->next = NULL;
							// add to end of run queue
	for (pnext = &OS.runnablejobs; *pnext; pnext = &((*pnext)->next))
		;
	*pnext = job;

	hal_enableIRQs();
}

// schedule timed job
void os_setTimedCallback (osjob_t* job, ostime_t time, osjobcb_t cb)
{
	osjob_t** pnext;
	hal_disableIRQs();

	os_clearCallback(job);	// remove if job was already queued
	job->deadline = time;	// fill-in job
	job->func = cb;
	job->next = NULL;
							// insert into schedule
	for (pnext = &OS.scheduledjobs; *pnext; pnext = &((*pnext)->next))
	{
		if ((*pnext)->deadline - time > 0)
		{	// (cmp diff, not abs!)
			// enqueue before next element and stop
			job->next = *pnext;
			break;
		}
	}
	*pnext = job;

	hal_enableIRQs();
}

// execute jobs from timer and from run queue
void os_runloop ()
{
	osjob_t* job;
	while (1)
	{
		job = NULL;
		hal_disableIRQs();
		// check for runnable jobs
		if (OS.runnablejobs)
		{
			job = OS.runnablejobs;
			OS.runnablejobs = job->next;
		} else if (OS.scheduledjobs && hal_checkTimer(OS.scheduledjobs->deadline))
		{	// check for expired timed jobs
			job = OS.scheduledjobs;
			OS.scheduledjobs = job->next;
		} else { // nothing pending
			hal_sleep(); // wake by irq (timer already restarted)
		}
		hal_enableIRQs();

		if (job != NULL)
		{	// run job callback
			job->func(job);
		}
	}
}
