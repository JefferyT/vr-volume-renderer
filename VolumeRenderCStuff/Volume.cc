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

int SaveVolume(const char *outputName, char *image, int width, int height) {
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

  cinfo.image_width = imageWidth;
  cinfo.image_height = imageHeight;
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
