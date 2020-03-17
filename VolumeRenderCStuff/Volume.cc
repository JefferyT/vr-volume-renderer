/*
 * Copyright ©2019 Jeffery Tian.  All rights reserved.
 * Idk how to do this copyright stuff. Use it if you want.
 */

#include "Volume.h"

#define DEFAULT_NUM_COMPONENTS 3

using std::cout;
using std::endl;
using std::vector;

Volume::Volume(const char *inputName, int width, int height, int depth) {
  FILE *fp = fopen(inputName, "rb");
  this->raw_volume_.resize(width * height * depth);
  fread(&this->raw_volume_[0], sizeof(short), this->raw_volume_.size(), fp);
  fclose(fp);
  this->volume_location_.x = width / 2.0;
  this->volume_location_.y = height / 2.0;
  this->volume_location_.z = -100.0;
  this->size_.x = width;
  this->size_.y = height;
  this->size_.z = depth;
}

int Volume::RenderVolume(const char *outputName, int imageWidth, int imageHeight) {
  return 1;
}

float TriLinearInterpolation(vec3 point) {
  return 0.0;
}

vec3 Gradient(vec3 position, float stepSize) {
  return vec3(0.0, 0.0, 0.0);
}

void Volume::PrintSlice(int depth) {
  int start = this->Index(0, 0, depth);
  int end = this->Index(0, 0, depth + 1);
  int i = 0;
  for (auto it = &this->raw_volume_[start]; it != &this->raw_volume_[end]; it += 16) {
    cout << *it << " ";
    if (i % (int) size_.x == 0) {
      cout << endl;
    }
    i += 16;
  }
}

int Volume::Index(int x, int y, int z) {
  return x + y * this->size_.x * this->size_.z + z * this->size_.x;
}
