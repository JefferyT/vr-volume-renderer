CC=gcc -Wall -g -std=c11
PP=g++ -Wall -g -std=c++11
COMP=-c
LINK=-o

all: VolumeRender

VolumeRender: VolumeRender.o Volume.o
	$(PP) $(LINK) VolumeRender VolumeRender.o Volume.o -ljpeg

VolumeRender.o: VolumeRender.cc
	$(PP) $(COMP) VolumeRender.cc

Volume.o: Volume.cc
	$(PP) $(COMP) Volume.cc

clean:
	rm -f *.o *~ VolumeRender