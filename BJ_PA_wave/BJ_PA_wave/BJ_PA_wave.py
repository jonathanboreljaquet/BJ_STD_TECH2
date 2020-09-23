import struct
import os
import datetime

FILENAME = "elvis6.wav"

def main():
    file = open(FILENAME,"rb")
    data = file.read()
    riff = data[:4].decode("utf-8")
    print(f"riff = {riff}")
    file_size = int.from_bytes(data[4:8],"little")
    print(f"file size = {file_size}")
    file_type_header = data[8:12].decode("utf-8")
    print(f"file_type_header = {file_type_header}")

        
        

       

if __name__ == "__main__":
    main()