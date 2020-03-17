void SaveVolume(std::shared_ptr<CSliceStackVolume> i_pVolume) {
  FILE *fp = fopen("body.dat", "wb");
  int dimX = i_pVolume->m_volumeMetaData.GetXResolution(),
      dimY = i_pVolume->m_volumeMetaData.GetYResolution(),
      dimZ = i_pVolume->m_volumeMetaData.GetZResolution();
  for (int i = 0; i < dimZ; i++) {
    const VoxelType *p = i_pVolume->Slice(i);
    fwrite(p, sizeof(VoxelType), dimX * dimY, fp);
  }
  fclose(fp);
}

void LoadVolume(std::vector<VoxelType>& volumedata) {
  FILE *fp = fopen("body.dat", "rb");
  fread(&volumedata[0], sizeof(VoxelType), volumedata.size(), fp);
  fclose(fp);
}

