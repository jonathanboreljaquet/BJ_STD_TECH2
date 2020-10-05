# Author : Borel-Jaquet Jonathan
# Date : 28.09.2020 - Ver 01
# Name : STD_01_BorelJaquet.py
# Desc.  : TIFF header reader and read the different IFDs for the lenna.tiff
import struct
import os

FILENAME = "Lenna.tiff"
LENGHT_IFD = 12
# table of different tags to display.
# https://www.awaresystems.be/imaging/tiff/tifftags/baseline.html
TAG_IFD = {  
    256:'ImageWidth', 257:'ImageLength',258:'BitsPerSample',
    259:'Compression',262:'PhotometricInterpretation',266:'FillOrder',
    273:'StripOffsets',274:'Orientation',277:'SamplesPerPixel',
    278:'RowsPerStrip',279:'StripByteCounts',284:'PlanarConfiguration',
    296:'ResolutionUnit',338:'ExtraSamples',339:'SampleFormat'
  }
# table of the different types of IFDs
TYPE_IFD = {  
    1: 'BYTE', 2: 'ASCII',3: 'SHORT',
    4: 'LONG',5: 'RATIONAL',6: 'SBYTE',
    7: 'UNDEFINED',8: 'SSHORT',9: 'SLONG',
    10: 'SRATIONAL',11: 'FLOAT',12: 'DOUBLE'
  }
# table of photometric interpretation.
# https://github.com/image-js/tiff/blob/master/TIFF6.pdf p.37
PHOTOMETRIC_INTERPRETATION = {
    0 : 'WhiteIsZero',1:'BlackIsZero',
    2:'RGB',3:'Palette color',4:'Transparency Mask'
    }

def main():
    # source file for reading (r)b
    file = open(FILENAME,"rb")
    data = file.read()
    print(f"File Name : {FILENAME}")
    file_byte_order = data[:2].decode("utf-8")
    print(f"Byte Order : {file_byte_order}")
    offset = int.from_bytes(data[5:8],"big")
    number_of_directory_entries = int.from_bytes(data[offset:offset + 2],"big")
    offset+=2
    # read all IFDs from the first IFD to the last (-1 to remove the first IFD
    # "Number of Directory Entries")
    for i in range(offset,offset + ((number_of_directory_entries - 1) * LENGHT_IFD),LENGHT_IFD):
        tag = int.from_bytes(data[i:i + 2],"big")
        type = int.from_bytes(data[i + 2:i + 4],"big")
        # if the type is long, the field size of the value is larger
        if TYPE_IFD[type] == 'LONG':
            value = int.from_bytes(data[i + 8:i + 12],"big")
        elif TYPE_IFD[type] == 'SHORT':
            if TAG_IFD[tag] == 'BitsPerSample' or TAG_IFD[tag] == 'SampleFormat':
                value = data[i + 8:i + 12]
            else:
                value = int.from_bytes(data[i + 8:i + 10],"big")
        
        print("----------------------------")
        print(f"tag : {TAG_IFD[tag]}")
        print(f"type : {TYPE_IFD[type]}")
        if TAG_IFD[tag] == 'PhotometricInterpretation':
            print(f"value : {PHOTOMETRIC_INTERPRETATION[value]}")
        else:
            print(f"value : {value}")

if __name__ == "__main__":
    main()