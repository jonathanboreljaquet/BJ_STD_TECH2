"""
Author  :   Francisco Garcia
Date    :   12.03.2020 - Ver 03
Name    :   invertbmp.py
Desc.   :   BMP Header Reader and inversion bmp color
"""

import struct
import os

# Read the header information from BMP files
# input : databytes
# return : tupple with header BPM info
def readHeader(databytes):   
    data  = struct.unpack('<cciHHiiiihhiiiii', databytes[0:0x32]) # https://docs.python.org/2/library/struct.html
    tmp = "Signature du fichier : " + (data[0]).decode("utf-8") + (data[1]).decode("utf-8")    
    print(tmp)
    tmp = "Taille totale du fichier en octets: " + str(data[2])
    print(tmp)
    print("Taille en-tête : " + str(data[5]))
    calc = hex(data[5])
    print(str(calc))
    calc = int("0x32", 16) 
    print("0x32 en base 10 : " + str(calc))
    print("Largeur image : " + str(data[7]))
    print("Hauteur image : " + str(data[8]))
    print("Nombre de bits par pixel : " + str(data[10]))
    print("Taille en octets des données de l’image : " + str(data[12]))
    calc = (data[5] + data[12])
    print("Calcul => (Taille en-tête + Taille en octets des données de l’image) * 8 => " + str(calc))
    return data


# source file for reading (r)b
src = open("logo_sav.bmp", "rb")

# destination file for writing (w)b
dst = open("invert.bmp", "wb")

src_data = src.read()
dst_data_lst = list(src_data) # Conversion from databytes to list
header = readHeader(src_data) 

for i in range(header[5], header[2]):
    dst_data_lst[i] = 255-src_data[i] # inverting pixel values

for rec in dst_data_lst:
    dst.write(rec.to_bytes(1, 'little')) # Writing list into file

# source and dest closing
src.close() 
dst.close()