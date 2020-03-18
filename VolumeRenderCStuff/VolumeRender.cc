/*
 * Copyright ©2019 Jeffery Tian.  All rights reserved.
 * Idk how to do this copyright stuff. Use it if you want.
 */

#include <stdio.h>
#include <cstdlib>
#include <string>
#include "Volume.h"

using std::string;

int main(int argc, char** argv) {
  Volume *vol;
  if (argc == 2) {
    vol = new Volume(argv[1], 512, 512, 406);
  } else {
    vol = new Volume("body.dat", 512, 512, 406);
  }

  vol->RenderVolume("out.jpg", 1024, 1024);
  //vol->RenderDefault("default.jpg");
}
