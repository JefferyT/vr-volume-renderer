#include <cstdlib>
#include <cstdio>


void LoadVolume(std::vector<short>& volumedata) {
  FILE *fp = fopen("body.dat", "rb");
  fread(&volumedata[0], sizeof(short), volumedata.size(), fp);
  fclose(fp);
}

