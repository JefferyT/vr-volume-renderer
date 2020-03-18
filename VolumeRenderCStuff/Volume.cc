/*
 * Copyright ©2019 Jeffery Tian.  All rights reserved.
 * Idk how to do this copyright stuff. Use it if you want.
 */

#include "Volume.h"

#define DEFAULT_NUM_COMPONENTS 3


Volume::Volume(const char *inputName, int width, int height, int depth) {
  FILE *fp = fopen(inputName, "rb");
  this->raw_volume_.resize(width * height * depth);
  fread(&this->raw_volume_[0], sizeof(short), this->raw_volume_.size(), fp);
  fclose(fp);
  this->volume_location_.x = 0;
  this->volume_location_.y = 0;
  this->volume_location_.z = 100.0;
  this->size_.x = width;
  this->size_.y = height;
  this->size_.z = depth;
}

int Volume::RenderVolume(const char *outputName, int imageWidth, int imageHeight) {
  float xStep = this->size_.x / (float) imageWidth;
  float yStep = this->size_.y / (float) imageHeight;
  unsigned char *buf = new unsigned char[imageHeight * imageWidth * 3];
  for (int i = 0; i < imageWidth; i++) {
    for (int j = 0; j < imageHeight; j++) {
      vec3 curColor = GetColor({(float) i * xStep, (float) j * yStep, 0.0}, {0.0, 0.0, 1.0});
      buf[(i + j * imageWidth) * 3] = (unsigned char) curColor.x;
      buf[(i + j * imageWidth) * 3 + 1] = (unsigned char) curColor.y;
      buf[(i + j * imageWidth) * 3 + 2] = (unsigned char) curColor.z;
    }
  }
  return SaveVolume(outputName, buf, imageWidth, imageHeight);
}

vec3 Volume::GetColor(vec3 position, vec3 direction) {
  float val = TriLinearInterpolation({position.x, position.y, 200.0});
  vec3 color;
  vec4 lookup = LookupTable(val);
  color = {lookup.x * 255.0, lookup.y * 255.0, lookup.z * 255.0};
  return color;
}

vec4 Volume::LookupTable(float value) {
  vec4 ret = {0, 0, 0, 0};
  if (value <= 142.677) {
    return ret;
  } else if (value <= 145.016) {
    float percent = ((value - 142.677) / (145.016 - 142.677));
    ret.w = (percent * (0.116071 - 0.0) + 0.0);

    ret.x = percent * ();
    ret.y = 0;
    ret.z = ;

  }
}

float Volume::TriLinearInterpolation(vec3 point) {
  int x = (int) point.x;
  int y = (int) point.y;
  int z = (int) point.z;
  float vals[8];

  // +0
  vals[0] = (float) this->raw_volume_[Index(x, y, z)] / Distance(point, {(float) x, (float)y, (float)z});
  // +x
  vals[1] = (float) this->raw_volume_[Index(x + 1, y, z)] / Distance(point, {(float) x + 1, (float)y, (float)z});
  // +y
  vals[2] = (float) this->raw_volume_[Index(x, y + 1, z)] / Distance(point, {(float) x, (float)y + 1, (float)z});
  // +z
  vals[3] = (float) this->raw_volume_[Index(x, y, z + 1)] / Distance(point, {(float) x, (float)y, (float)z + 1});
  // +x + y
  vals[4] = (float) this->raw_volume_[Index(x + 1, y + 1, z)] / Distance(point, {(float) x + 1, (float)y + 1, (float)z});
  // +x +z
  vals[5] = (float) this->raw_volume_[Index(x + 1, y, z + 1)] / Distance(point, {(float) x + 1, (float)y, (float)z + 1});
  // +y +z
  vals[6] = (float) this->raw_volume_[Index(x, y + 1, z + 1)] / Distance(point, {(float) x, (float)y + 1, (float)z + 1});
  // +x +y +z
  vals[7] = (float) this->raw_volume_[Index(x + 1, y + 1, z + 1)] / Distance(point, {(float) x + 1, (float)y + 1, (float)z + 1});
  float ret = 0.0;
  for (int i = 0; i < 8; i++) {
    ret += vals[i];
  }
  return ret;
}

vec3 Volume::Gradient(vec3 position, float stepSize) {
  vec3 gradient;
  gradient.x = TriLinearInterpolation({position.x + stepSize, position.y, position.z})
              + TriLinearInterpolation({position.x - stepSize, position.y, position.z});
  gradient.x /= 2.0;

  gradient.y = TriLinearInterpolation({position.x, position.y + stepSize, position.z})
              + TriLinearInterpolation({position.x, position.y - stepSize, position.z});
  gradient.y /= 2.0;

  gradient.z = TriLinearInterpolation({position.x, position.y, position.z - stepSize})
              + TriLinearInterpolation({position.x, position.y, position.z - stepSize});
  gradient.z /= 2.0;

  
  return gradient;
}

float Volume::Distance(vec3 u, vec3 v) {
  return sqrt(pow(u.x - v.x, 2.0) + pow(u.y - v.y, 2.0) + pow(u.y - v.y, 2.0));
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

int Volume::SaveVolume(const char *outputName, unsigned char *image, int width, int height) {
  struct jpeg_compress_struct cinfo;
	struct jpeg_error_mgr jerr;
  JSAMPROW row_pointer[1];
  FILE *outfile = fopen(outputName, "wb");
  if (!outfile) {
    cerr << "Error opening output jpeg file " << outputName << endl;
    return -1;
  }

  cinfo.err = jpeg_std_error(&jerr);
  jpeg_create_compress(&cinfo);
  jpeg_stdio_dest(&cinfo, outfile);

  cinfo.image_width = width;
  cinfo.image_height = height;
  cinfo.input_components = 3;
  cinfo.in_color_space = JCS_RGB;

  jpeg_set_defaults(&cinfo);
  cinfo.num_components = 3;

  cinfo.dct_method = JDCT_FLOAT;
  jpeg_set_quality(&cinfo, 100, true);

  jpeg_start_compress(&cinfo, TRUE);

  while (cinfo.next_scanline < cinfo.image_height) {
    row_pointer[0] = &(image[cinfo.next_scanline
            * cinfo.image_width * cinfo.input_components]);
    jpeg_write_scanlines(&cinfo, row_pointer, 1);
  }

  jpeg_finish_compress(&cinfo);
  jpeg_destroy_compress(&cinfo);
  fclose(outfile);

  return 1;
}
