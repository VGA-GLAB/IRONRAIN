#ifndef CUSTOM_MACRO_INCLUDED
#define CUSTOM_MACRO_INCLUDED

#define remap(value, inMin, inMax, outMin, outMax) (value - inMin) * ((outMax - outMin) / (inMax - inMin)) + outMin

#endif