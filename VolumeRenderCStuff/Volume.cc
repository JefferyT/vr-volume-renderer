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
  this->size_.y = depth;
  this->size_.z = height;
}

int Volume::RenderVolume(const char *outputName, int imageWidth, int imageHeight) {
  float xStep = this->size_.x / (float) imageWidth;
  float yStep = this->size_.y / (float) imageHeight;
  unsigned char *buf = new unsigned char[imageHeight * imageWidth * 3];
  for (int i = 0; i < imageWidth; i++) {
    for (int j = 0; j < imageHeight; j++) {
      vec3 curColor = GetColor({(float) i * xStep, (float) j * yStep, 0.0}, {0.0, 0.0, 1.0}, 1);
      buf[(i + (imageHeight - j) * imageWidth) * 3] = (unsigned char) curColor.x;
      buf[(i + (imageHeight - j) * imageWidth) * 3 + 1] = (unsigned char) curColor.y;
      buf[(i + (imageHeight - j) * imageWidth) * 3 + 2] = (unsigned char) curColor.z;
    }
  }
  return SaveVolume(outputName, buf, imageWidth, imageHeight);
}

int Volume::RenderDefault(const char *outputName) {
  unsigned char *buf = new unsigned char[512 * 406 * 3];
  for (int i = 0; i < 512; i++) {
    for (int j = 0; j < 406; j++) {
      vec3 curColor = TestLookupTable((float)(raw_volume_[Index(i, j, 256)]));
      buf[(i + j * 512) * 3] = (unsigned char) curColor.x;
      buf[(i + j * 512) * 3 + 1] = (unsigned char) curColor.z;
      buf[(i + j * 512) * 3 + 2] = (unsigned char) curColor.y;
    }
  }
  
  return SaveVolume(outputName, buf, 512, 406);
}

vec3 Volume::TestLookupTable(float val) {
  if (val < 0) {
    val = 0;
  }
  if (val > 3600.0) {
    val = 3600.0;
  }
  return {255.0f * (val / 3600.0f), 255.0f * (val / 3600.0f), 255.0f * (val / 3600.0f)};
}

vec3 Volume::GetColor(vec3 position, vec3 direction, float stepSize) {
  vec3 curPos = position;
  vec4 cur = {0.0, 0.0, 0.0, 0.0};
  int i = 0;
  while (cur.w <= 0.99 && curPos.x < this->size_.x && curPos.y < this->size_.y && curPos.z < this->size_.z ) {
    vec4 curColorOpacity = this->LookupTable(TriLinearInterpolation(curPos));
    curPos.x += direction.x * stepSize;
    curPos.y += direction.y * stepSize;
    curPos.z += direction.z * stepSize;
    

    // new color
    cur.x += curColorOpacity.x * curColorOpacity.w * (1.0 - cur.w);
    cur.y += curColorOpacity.y * curColorOpacity.w * (1.0 - cur.w);
    cur.z += curColorOpacity.z * curColorOpacity.w * (1.0 - cur.w);    

    // new opacity
    cur.w += curColorOpacity.w * (1.0 - cur.w);

    curPos.x += stepSize * (direction.x);
    curPos.y += stepSize * (direction.y);
    curPos.z += stepSize * (direction.z);
    i++;
  }
  // cout << i << endl;
  float val = TriLinearInterpolation({position.x, position.y, 200.0});
  vec3 color;
  vec4 lookup = LookupTable(val);
  color = {lookup.x * 255.0f, lookup.y * 255.0f, lookup.z * 255.0f};
  if (color.x > 255.0f) {
    color.x = 255.0f;
  }
  if (color.y > 255.0f) {
    color.y = 255.0f;
  }
  if (color.z > 255.0f) {
    color.z = 255.0f;
  }
  return color;
}

vec4 Volume::LookupTable(float value) {
  vec4 ret = {0, 0, 0, 0};
  vector<float> scalarOpacity =
  {-2048, 0,
    142.677, 0,
    145.016, 0.116071,
    192.174, 0.5625,
    217.24, 0.776786,
    384.347, 0.830357,
    3661, 0.830357};
  vector<float> colorTransfer = 
  {-2048, 0, 0, 0,
  142.677, 0, 0, 0,
  145.016, 0.615686, 0, 0.0156863,
  192.174, 0.909804, 0.454902, 0,
  217.24, 0.972549, 0.807843, 0.611765,
  384.347, 0.909804, 0.909804, 1,
  3661, 1, 1, 1};
  int count = 0;
  while (value > scalarOpacity[count * 2] && count < 7) {
    count++;
  }
  if (count == 7) {
    ret = {1, 1, 1, 1};
  } else if (count == 0) {
    ret = {0, 0, 0, 0};
  } else {
    float percent = (value - scalarOpacity[(count - 1) * 2]) / (scalarOpacity[(count) * 2])
                  - scalarOpacity[(count - 1) * 2];
    ret.w = (scalarOpacity[(count) * 2 + 1] - scalarOpacity[(count - 1) * 2 + 1])
            * percent + (scalarOpacity[(count) * 2 + 1] - scalarOpacity[(count - 1) * 2 + 1]);
    
    ret.x = (colorTransfer[(count) * 4 + 1] - colorTransfer[(count - 1) * 4 + 1])
            * percent + (colorTransfer[(count) * 4 + 1] - colorTransfer[(count - 1) * 4 + 1]);
    
    ret.y = (colorTransfer[(count) * 4 + 2] - colorTransfer[(count - 1) * 4 + 2])
            * percent + (colorTransfer[(count) * 4 + 2] - colorTransfer[(count - 1) * 4 + 2]);
    
    ret.z = (colorTransfer[(count) * 4 + 3] - colorTransfer[(count - 1) * 4 + 3])
            * percent + (colorTransfer[(count) * 4 + 3] - colorTransfer[(count - 1) * 4 + 3]);
  }
  return ret;
}

float Volume::TriLinearInterpolation(vec3 point) {
  float xd = (int) (point.x);
  float yd = (int) (point.y);
  float zd = (int) (point.z);

  
  
  float cXXX[8];

  // c000
  cXXX[0] = (float) this->raw_volume_[Index(xd, yd, zd)];
  // c100
  cXXX[1] = (float) this->raw_volume_[Index(xd + 1, yd, zd)];
  // c010
  cXXX[2] = (float) this->raw_volume_[Index(xd, yd + 1, zd)];
  // c001
  cXXX[3] = (float) this->raw_volume_[Index(xd, yd, zd + 1)];
  // c110
  cXXX[4] = (float) this->raw_volume_[Index(xd + 1, yd + 1, zd)];
  // c101
  cXXX[5] = (float) this->raw_volume_[Index(xd + 1, yd, zd + 1)];
  // c011
  cXXX[6] = (float) this->raw_volume_[Index(xd, yd + 1, zd + 1)];
  // c111
  cXXX[7] = (float) this->raw_volume_[Index(xd + 1, yd + 1, zd + 1)];
  
  xd = point.x - xd;
  yd = point.y - yd;
  zd = point.z - zd;

  xd /= 1.0;
  yd /= 1.0;
  zd /= 1.0;

  float cXX[4];
  // c00
  cXX[0] = cXXX[0] *(1.0 - xd) + cXXX[1];
  // c01
  cXX[1] = cXXX[3] *(1.0 - xd) + cXXX[5];
  // c10
  cXX[2] = cXXX[2] *(1.0 - xd) + cXXX[4];
  // c11
  cXX[3] = cXXX[6] *(1.0 - xd) + cXXX[7];

  float cX[2];
  // c0
  cX[0] = cXX[0] *(1.0 - zd) + cXX[1];
  // c1
  cX[1] = cXX[2] *(1.0 - zd) + cXX[2];

  float c;
  c = cX[0] *(1.0 - yd) + cX[1];

  return c;
}

vec3 Volume::Gradient(vec3 position, float stepSize) {
  vec3 gradient;
  gradient.x = TriLinearInterpolation({position.x + stepSize, position.y, position.z})
              + TriLinearInterpolation({position.x - stepSize, position.y, position.z});
  gradient.x /= 2.0;

  gradient.y = TriLinearInterpolation({position.x, position.y + stepSize, position.z})
              + TriLinearInterpolation({position.x, position.y - stepSize, position.z});
  gradient.y /= 2.0;

  gradient.z = TriLinearInterpolation({position.x, position.y, position.z + stepSize})
              + TriLinearInterpolation({position.x, position.y, position.z - stepSize});
  gradient.z /= 2.0;

  
  return gradient;
}

float Volume::Distance(vec3 u, vec3 v) {
  return sqrt(pow(u.x - v.x, 2.0) + pow(u.y - v.y, 2.0) + pow(u.z - v.z, 2.0));
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
  return x + y * (int) this->size_.x * (int) this->size_.z + z * (int) this->size_.x;
}

int Volume::SaveVolume(const char *outputName, unsigned char *image, int width, int height) {
/*  FILE *fp = fopen("image.dat", "wb");
  fwrite(image, sizeof(char), width * height * 3, fp);
  fclose(fp);
*/
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
