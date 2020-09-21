import struct
import os
import datetime

FILENAME = "test.zip"

def main():
    file = open(FILENAME,"rb")
    data = file.read()

    #file_signature = int.from_bytes(data[:4],"little")
    #print(f"Signature du fichier : {hex(file_signature)}")
    #
    #file_version = int.from_bytes(data[4:6],"little")
    #print(f"Signature du fichier : {file_version}")
    #
    #file_bitflag = int.from_bytes(data[6:8],"little")
    #print(f"Bit flag du fichier : {file_bitflag}")
    #
    #file_compresed_method = int.from_bytes(data[8:10],"little")
    #print(f"Methode de compression : {file_compresed_method}")
    #
    #file_last_modification_time = int.from_bytes(data[10:12],"little")
    #print(f"Temps de la dernière modification  : {file_last_modification_time}")
    #
    #file_last_modification_date = int.from_bytes(data[12:14],"little")
    #print(f"Date de la dernière modification  : {file_last_modification_date}")
    #
    #file_compressed_size = int.from_bytes(data[18:22],"little")
    #print(f"Taille du fichier compressé  : {file_compressed_size}")
    #
    #file_name_length = int.from_bytes(data[26:28],"little")
    #print(f"Taille du nom de fichier : {file_name_length}")
    #
    #file_extra_field_length = int.from_bytes(data[28:30],"little")
    #print(f"Taille du Extra field : {file_extra_field_length}")
    #
    #file_name = data[30:30+file_name_length].decode("utf-8")
    #print(f"Nom du fichier : {file_name}")
    #
    #file_extra_field = data[30+file_name_length:30+file_name_length+file_extra_field_length].decode("utf-8")
    #print(f"Extra field du fichier : {file_extra_field}")
    #
    #dasda = int.from_bytes(data[30+file_name_length+file_compressed_size:30+file_name_length+file_compressed_size+4],"little")
    #print(hex(dasda))
    x=0
    while True:
        x+=4
        file_signature = hex(int.from_bytes(data[x-4:x],"little"))
        if file_signature != "0x4034b50":
            break
        x+=4
        file_compresed_method = int.from_bytes(data[x:x+2],"little")
        x+=10
        file_compressed_size = int.from_bytes(data[x:x+4],"little")
        x+=4
        file_size = int.from_bytes(data[x:x+4],"little")
        x+=4
        file_name_length = int.from_bytes(data[x:x+2],"little")
        x+=2
        file_extra_field_length = int.from_bytes(data[x:x+2],"little")
        x+=2
        file_name = data[x:x+file_name_length].decode("utf-8")
        x+= (file_name_length+file_compressed_size)

        print(f"filename : {file_name}")
        print(f'{"compress_type : Deflate" if file_compresed_method==8 else ""}')
        print(f"size          : {file_size}")
        print(f"compress_size : {file_compressed_size}")
        print("-----------------------------------------")
        
        

       

if __name__ == "__main__":
    main()