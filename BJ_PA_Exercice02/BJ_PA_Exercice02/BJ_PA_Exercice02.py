import struct
import os
from tinytag import TinyTag

FILENAME = "song.mp3"

def unsynchsafe(num):
        out = 0
        mask = 0x7f000000
        for i in range(4):
            out >>= 1
            out |= num & mask
            mask >>= 8
        return out

def humanbytes(B):
   'Return the given bytes as a human friendly KB, MB, GB, or TB string'
   B = float(B)
   KB = float(1024)
   MB = float(KB ** 2) # 1,048,576
   GB = float(KB ** 3) # 1,073,741,824
   TB = float(KB ** 4) # 1,099,511,627,776

   if B < KB:
      return '{0} {1}'.format(B,'Bytes' if 0 == B > 1 else 'Byte')
   elif KB <= B < MB:
      return '{0:.2f} KB'.format(B/KB)
   elif MB <= B < GB:
      return '{0:.2f} MB'.format(B/MB)
   elif GB <= B < TB:
      return '{0:.2f} GB'.format(B/GB)
   elif TB <= B:
      return '{0:.2f} TB'.format(B/TB)

def main():
    file = open(FILENAME, "rb")
    data = file.read()
    tag = TinyTag.get(FILENAME)
    # Read the file type
    file_type = data[:0x3].decode("utf-8")
    print(f"Signature du fichier : {file_type}")

    # Read the file version
    file_version_major = int.from_bytes(data[0x3:0x4], "little")
    file_version_minor = int.from_bytes(data[0x4:0x5], "little")
    print(f"Version du fichier : ID3v2.{file_version_major}.{file_version_minor}")

    # Read the file size
    file_size = unsynchsafe(int.from_bytes(data[0x06:0x0B], "big"))
    print(f"Taille du fichier : {humanbytes(file_size)}")

    # Read the file extanded header size
    file_extanded_size = unsynchsafe(int.from_bytes(data[0x0B:0x0F], "little"))
    print(f"Taille de l'entête du fichier : {humanbytes(file_extanded_size)}")


    # Read the file album
    file_album = data[0x0F:0x4B].decode("utf-8")
    print(f"nom de l'album du fichier : {file_album}")
    print("-----------------------------------------------------------")
    print("tinytag")
    print(tag.album)
    print(tag.artist)
    # Read the file year partition
    # file_year_parution = data[0x93:0x97].decode("utf-8")
    # print(f"Année de parution du fichier : {file_year_parution}")

    # Read the file commentary
    # file_commentary = data[0x97:0x127].decode("utf-8")
    # print(f"Commentaire sur la chanson : {file_commentary}")

    # Read the file musical genre
    # file_genre = data[0x127:0x128].decode("utf-8")
    # print(f"Genre musical : {file_genre}")


if __name__ == "__main__":
    main()
