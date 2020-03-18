/*
 * Copyright ©2019 Jeffery Tian.  All rights reserved.
 * Idk how to do this copyright stuff. Use it if you want.
 */

#ifndef VOLUME_H_
#define VOLUME_H_

#include <cstdlib>
#include <string>
#include <iostream>
#include <memory>
#include <vector>
#include <jpeglib.h>
#include <string.h>
#include <math.h>

using std::shared_ptr;
using std::string;
using std::vector;
using std::cerr;
using std::cout;
using std::endl;

typedef struct Vector3 {
  float x;
  float y;
  float z;
} vec3;

typedef struct Vector4 {
  float x;
  float y;
  float z;
  float w;
} vec4;

class Volume {
 public:
  Volume(const char *inputName, int width, int height, int depth);
  int RenderVolume(const char *outputName, int imageWidth, int imageHeight);
  void PrintSlice(int depth);
  
 private:
  // gets the index of the volume
  int Index(int x, int y, int z);
  // Interpolates the point at a vec3
  float TriLinearInterpolation(vec3 position);
  // gets the gradient at a point
  vec3 Gradient(vec3 position, float stepSize);
  // gets the color for a certain ray
  vec3 GetColor(vec3 position, vec3 direction);
  vec4 LookupTable(float value);
  int SaveVolume(const char *outputName, unsigned char *image, int width, int height);
  float Distance(vec3 u, vec3 v);
  vector<short> raw_volume_;
  vec3 volume_location_;
  vec3 size_;
};


#endif  // VOLUME_H_
