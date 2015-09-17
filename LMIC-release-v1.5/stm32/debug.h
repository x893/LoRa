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

// intialize debug library
void debug_init (void);

// set LED state
void debug_led (uint8_t val);

// write character to USART
void debug_char (uint8_t c);

// write byte as two hex digits to USART
void debug_hex (uint8_t b);

// write buffer as hex dump to USART
void debug_buf (const uint8_t* buf, uint16_t len);

// write 32-bit integer as eight hex digits to USART
void debug_uint (uint32_t v);

// write nul-terminated string to USART
void debug_str (const char * str);

// write LMiC event name to USART
void debug_event (int ev);

// write label and 32-bit value as hex to USART
void debug_val (const char * label, uint32_t val);
